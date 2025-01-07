#import "UnityAppController.h"
#import "UnityAppController+ViewHandling.h"
#import "UnityAppController+Rendering.h"
#import "iPhone_Sensors.h"

#import <CoreGraphics/CoreGraphics.h>
#import <QuartzCore/QuartzCore.h>
#import <QuartzCore/CADisplayLink.h>
#import <Availability.h>
#import <AVFoundation/AVFoundation.h>

#include <mach/mach_time.h>

// MSAA_DEFAULT_SAMPLE_COUNT was removed
// ENABLE_INTERNAL_PROFILER and related defines were moved to iPhone_Profiler.h
// kFPS define for removed: you can use Application.targetFrameRate (30 fps by default)
// DisplayLink is the only run loop mode now - all others were removed

#include "CrashReporter.h"

#include "UI/OrientationSupport.h"
#include "UI/UnityView.h"
#include "UI/Keyboard.h"
#include "UI/UnityViewControllerBase.h"
#include "Unity/InternalProfiler.h"
#include "Unity/DisplayManager.h"
#include "Unity/ObjCRuntime.h"
#include "PluginBase/AppDelegateListener.h"

#include <assert.h>
#include <stdbool.h>
#include <sys/types.h>
#include <unistd.h>
#include <sys/sysctl.h>

// we assume that app delegate is never changed and we can cache it, instead of re-query UIApplication every time
UnityAppController* _UnityAppController = nil;
UnityAppController* GetAppController()
{
    return _UnityAppController;
}

// we keep old bools around to support "old" code that might have used them
bool _ios81orNewer = false, _ios82orNewer = false, _ios83orNewer = false, _ios90orNewer = false, _ios91orNewer = false;
bool _ios100orNewer = false, _ios101orNewer = false, _ios102orNewer = false, _ios103orNewer = false;
bool _ios110orNewer = false, _ios111orNewer = false, _ios112orNewer = false;
bool _ios130orNewer = false, _ios140orNewer = false, _ios150orNewer = false, _ios160orNewer = false;

// was core of Unity loaded (non-graphics part prior to loading first scene)
bool _unityEngineInitialized = false;
// was unity rendering already inited: we should not touch rendering while this is false
bool    _renderingInited        = false;
// was unity inited: we should not touch unity api while this is false
bool    _unityAppReady          = false;
// see if there's a need to do internal player pause/resume handling
//
// Typically the trampoline code should manage this internally, but
// there are use cases, videoplayer, plugin code, etc where the player
// is paused before the internal handling comes relevant. Avoid
// overriding externally managed player pause/resume handling by
// caching the state
bool    _wasPausedExternal      = false;
// should we skip present on next draw: used in corner cases (like rotation) to fill both draw-buffers with some content
bool    _skipPresent            = false;
// was app "resigned active": some operations do not make sense while app is in background
bool    _didResignActive        = false;

#if UNITY_SUPPORT_ROTATION
// Required to enable specific orientation for some presentation controllers: see supportedInterfaceOrientationsForWindow below for details
NSInteger _forceInterfaceOrientationMask = 0;
#endif

@implementation UnityAppController

@synthesize unityView               = _unityView;
@synthesize unityDisplayLink        = _displayLink;

@synthesize rootView                = _rootView;
@synthesize rootViewController      = _rootController;
@synthesize mainDisplay             = _mainDisplay;
@synthesize renderDelegate          = _renderDelegate;
@synthesize quitHandler             = _quitHandler;

#if UNITY_SUPPORT_ROTATION
@synthesize interfaceOrientation    = _curOrientation;
#endif

- (id)init
{
    if ((self = _UnityAppController = [super init]))
    {
        // due to clang issues with generating warning for overriding deprecated methods
        // we will simply assert if deprecated methods are present
        // NB: methods table is initied at load (before this call), so it is ok to check for override
        NSAssert(![self respondsToSelector: @selector(createUnityViewImpl)],
            @"createUnityViewImpl is deprecated and will not be called. Override createUnityView"
        );
        NSAssert(![self respondsToSelector: @selector(createViewHierarchyImpl)],
            @"createViewHierarchyImpl is deprecated and will not be called. Override willStartWithViewController"
        );
        NSAssert(![self respondsToSelector: @selector(createViewHierarchy)],
            @"createViewHierarchy is deprecated and will not be implemented. Use createUI"
        );
    }
    return self;
}

- (void)setWindow:(id)object        {}
- (UIWindow*)window                 { return _window; }


- (void)shouldAttachRenderDelegate  {}
- (void)preStartUnity               {}


- (void)startUnity:(UIApplication*)application
{
    NSAssert(_unityAppReady == NO, @"[UnityAppController startUnity:] called after Unity has been initialized");

    UnityInitApplicationGraphics();

#if !PLATFORM_VISIONOS
    // we make sure that first level gets correct display list and orientation
    [[DisplayManager Instance] updateDisplayListCacheInUnity];
#endif

    UnityLoadApplication();
    Profiler_InitProfiler();

    [self showGameUI];
    [self createDisplayLink];

    UnitySetPlayerFocus(1);

    AVAudioSession* audioSession = [AVAudioSession sharedInstance];
    // If Unity audio is disabled, we set the category to ambient to make sure we don't mute other app's audio. We set the audio session
    // to active so we can get outputVolume callbacks. If Unity audio is enabled, FMOD should have already handled all of this AVAudioSession init.
    if (!UnityIsAudioManagerAvailableAndEnabled())
    {
        [audioSession setCategory: AVAudioSessionCategoryAmbient error: nil];
        [audioSession setActive: YES error: nil];
    }
    [audioSession addObserver: self forKeyPath: @"outputVolume" options: 0 context: nil];
    UnityUpdateMuteState([audioSession outputVolume] < 0.01f ? 1 : 0);

#if UNITY_REPLAY_KIT_AVAILABLE
    void InitUnityReplayKit();  // Classes/Unity/UnityReplayKit.mm

    InitUnityReplayKit();
#endif
}

extern "C" void UnityDestroyDisplayLink()
{
    [GetAppController() destroyDisplayLink];
}

extern "C" void UnityRequestQuit()
{
    _didResignActive = true;
    if (GetAppController().quitHandler)
        GetAppController().quitHandler();
    else
        exit(0);
}

extern void SensorsCleanup();
extern "C" void UnityCleanupTrampoline()
{
    // Unity view and viewController will not necessary be destroyed right after this function execution.
    // We need to ensure that these objects will not receive any callbacks from system during that time.
    [_UnityAppController window].rootViewController = nil;
    [[_UnityAppController unityView] removeFromSuperview];

    // Prevent multiple cleanups
    if (_UnityAppController == nil)
        return;

    [KeyboardDelegate Destroy];

    SensorsCleanup();

    Profiler_UninitProfiler();

    [DisplayManager Destroy];

    UnityDestroyDisplayLink();

    _UnityAppController = nil;
}

#if UNITY_SUPPORT_ROTATION

- (NSUInteger)application:(UIApplication*)application supportedInterfaceOrientationsForWindow:(UIWindow*)window
{
    // No rootViewController is set because we are switching from one view controller to another, all orientations should be enabled
    if ([window rootViewController] == nil)
        return UIInterfaceOrientationMaskAll;

    // During splash screen show phase no forced orientations should be allowed.
    // This will prevent unwanted rotation while splash screen is on and application is not yet ready to present (Ex. Fogbugz cases: 1190428, 1269547).
    if (!_unityAppReady)
        return [_rootController supportedInterfaceOrientations];

    // Some presentation controllers (e.g. UIImagePickerController) require portrait orientation and will throw exception if it is not supported.
    // At the same time enabling all orientations by returning UIInterfaceOrientationMaskAll might cause unwanted orientation change
    // (e.g. when using UIActivityViewController to "share to" another application, iOS will use supportedInterfaceOrientations to possibly reorient).
    // So to avoid exception we are returning combination of constraints for root view controller and orientation requested by iOS.
    // _forceInterfaceOrientationMask is updated in willChangeStatusBarOrientation, which is called if some presentation controller insists on orientation change.
    return [[window rootViewController] supportedInterfaceOrientations] | _forceInterfaceOrientationMask;
}

- (void)application:(UIApplication*)application willChangeStatusBarOrientation:(UIInterfaceOrientation)newStatusBarOrientation duration:(NSTimeInterval)duration
{
    // Setting orientation mask which is requested by iOS: see supportedInterfaceOrientationsForWindow above for details
    _forceInterfaceOrientationMask = 1 << newStatusBarOrientation;
}

#endif

#if UNITY_USES_REMOTE_NOTIFICATIONS

- (void)application:(UIApplication*)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData*)deviceToken
{
    AppController_SendNotificationWithArg(kUnityDidRegisterForRemoteNotificationsWithDeviceToken, deviceToken);
}

#if !PLATFORM_TVOS
- (void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo fetchCompletionHandler:(void (^)(UIBackgroundFetchResult result))handler
{
    AppController_SendNotificationWithArg(kUnityDidReceiveRemoteNotification, userInfo);

    if (handler)
    {
        handler(UIBackgroundFetchResultNoData);
    }
}

#endif

- (void)application:(UIApplication*)application didFailToRegisterForRemoteNotificationsWithError:(NSError*)error
{
    AppController_SendNotificationWithArg(kUnityDidFailToRegisterForRemoteNotificationsWithError, error);

    // alas people do not check remote notification error through api (which is clunky, i agree) so log here to have at least some visibility
    ::printf("\nFailed to register for remote notifications:\n%s\n\n", [[error localizedDescription] UTF8String]);
}

#endif

// UIApplicationOpenURLOptionsKey was added only in ios10 sdk, while we still support ios9 sdk
- (BOOL)application:(UIApplication*)app openURL:(NSURL*)url options:(NSDictionary<NSString*, id>*)options
{
    id sourceApplication = options[UIApplicationOpenURLOptionsSourceApplicationKey], annotation = options[UIApplicationOpenURLOptionsAnnotationKey];

    NSMutableDictionary<NSString*, id>* notifData = [NSMutableDictionary dictionaryWithCapacity: 3];
    if (url)
    {
        notifData[@"url"] = url;
        UnitySetAbsoluteURL(url.absoluteString.UTF8String);
    }
    if (sourceApplication) notifData[@"sourceApplication"] = sourceApplication;
    if (annotation) notifData[@"annotation"] = annotation;

    AppController_SendNotificationWithArg(kUnityOnOpenURL, notifData);
    return YES;
}

- (BOOL)application:(UIApplication *)application continueUserActivity:(NSUserActivity *)userActivity restorationHandler:(void (^)(NSArray<id<UIUserActivityRestoring> > * _Nullable restorableObjects))restorationHandler
{
    NSURL* url = userActivity.webpageURL;
    if (url)
        UnitySetAbsoluteURL(url.absoluteString.UTF8String);
    return YES;
}

- (BOOL)application:(UIApplication*)application willFinishLaunchingWithOptions:(NSDictionary*)launchOptions
{
    AppController_SendNotificationWithArg(kUnityWillFinishLaunchingWithOptions, launchOptions);
    return YES;
}

- (UIWindowScene*)pickStartupWindowScene:(NSSet<UIScene*>*)scenes API_AVAILABLE(ios(13.0), tvos(13.0))
{
    // if we have scene with UISceneActivationStateForegroundActive - pick it
    // otherwise UISceneActivationStateForegroundInactive will work
    //   it will be the scene going into active state
    // if there were no active/inactive scenes (only background) we should allow background scene
    //   this might happen in some cases with native plugins doing "things"
    UIWindowScene *foregroundScene = nil, *backgroundScene = nil;
    for (UIScene* scene in scenes)
    {
        if (![scene isKindOfClass: [UIWindowScene class]])
            continue;
        UIWindowScene* windowScene = (UIWindowScene*)scene;

        if (scene.activationState == UISceneActivationStateForegroundActive)
            return windowScene;
        if (scene.activationState == UISceneActivationStateForegroundInactive)
            foregroundScene = windowScene;
        else if (scene.activationState == UISceneActivationStateBackground)
            backgroundScene = windowScene;
    }

    return foregroundScene ? foregroundScene : backgroundScene;
}

- (BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions
{
    ::printf("-> applicationDidFinishLaunching()\n");

    // send notfications
#if !PLATFORM_TVOS && !PLATFORM_VISIONOS
    if ([UIDevice currentDevice].generatesDeviceOrientationNotifications == NO)
        [[UIDevice currentDevice] beginGeneratingDeviceOrientationNotifications];
#endif

    if ([self isBackgroundLaunchOptions: launchOptions])
        return YES;

    [self initUnityWithApplication: application];
    return YES;
}

- (BOOL)isBackgroundLaunchOptions:(NSDictionary*)launchOptions
{
    if (launchOptions.count == 0)
        return NO;

    // launch due to location event, the app likely will stay in background
    BOOL locationLaunch = [[launchOptions valueForKey: UIApplicationLaunchOptionsLocationKey] boolValue];
    if (locationLaunch)
        return YES;
    return NO;
}

- (void)initUnityWithApplication:(UIApplication*)application
{
    if (_unityEngineInitialized)
        return;
    _unityEngineInitialized = true;

    // basic unity init
    UnityInitApplicationNoGraphics(UnityDataBundleDir());

    [self selectRenderingAPI];
    [UnityRenderingView InitializeForAPI: self.renderingAPI];

#if !PLATFORM_VISIONOS
    if (@available(iOS 13, tvOS 13, *))
        _window = [[UIWindow alloc] initWithWindowScene: [self pickStartupWindowScene: application.connectedScenes]];
    else
        _window = [[UIWindow alloc] initWithFrame: [UIScreen mainScreen].bounds];
#else
    _window = [[UIWindow alloc] init]; 
#endif

    _window         = [[UIWindow alloc] initWithFrame: [UIScreen mainScreen].bounds];
//    _unityView      = [self createUnityView];

    
    if(_window.frame.size.height / _window.frame.size.width > 2)
    {
        CGRect rect = _window.frame;
        rect.size.height -= 32;
        rect.origin.y = 32;
        rect.size.height -= 28;
        _window.frame = rect;
    }
    
    _unityView      = [self createUnityView];


    [DisplayManager Initialize];
    _mainDisplay = [DisplayManager Instance].mainDisplay;
    [_mainDisplay createWithWindow: _window andView: _unityView];

    [self createUI];
    [self preStartUnity];

    // if you wont use keyboard you may comment it out at save some memory
    [KeyboardDelegate Initialize];


    //初始化 统计和买量SDK。
    /// @param key AppsFlyer 账号提供的key
    /// @param appId App Store 上app上架地址的 appId
    /// @param gameName AppsFlyer使用的统计前缀，需跟SDK统一。
    /// @param delegate id
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(attRequestNotification:) name:ATTRequestNotification object:NULL];
	[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(webPageNotification:) name:@"iOSWebPageShow" object:NULL];


	
	//打开SDK输出log
   CLogShowLog(false);
   
   //带vc的打点事件 suffixCode 每次根据版本号变动
   [AppsFlyerProxy versionSuffixList:@[@"ad_request",@"ad_show",@"af_succ",@"luck_balance",@"interstitial_show",@"video_show",@"balance_display",@"loading_finish",@"loading_timeout"] prefixList:@[@"lang"] suffixCode:2];
   
   //appId是应用的id，gameName是统计前缀 WorQjrKdXrWjRvCSwVXcTP 6479337814
   [AppsFlyerProxy instanceInitNoATTWithKey:@"SFmpoiTG8y4MzCSPePPpsP" appId: @"1611139549" gameName: @"" delegate:self];
   [AppsFlyerProxy refrenceWindow:self.window];

   [AppsFlyerProxy firstCheckSystemIdfaSet];

   //确保idfa提示后初始化
   if ([AppsFlyerProxy attDialogCanShow] == true) {
       dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(2.0 * NSEC_PER_SEC)), dispatch_get_main_queue(), ^{
           [AdManager instancePrepareGDPRWithDelegate:self rootCon:self.window.rootViewController];
       });
   }

    [self performSelector:@selector(changeLanguage) withObject:nil afterDelay:2];
    
	#if UNITY_DEVELOPER_BUILD
    // Causes a black screen after splash screen, but would deadlock if waiting for manged debugger otherwise
    [self performSelector: @selector(startUnity:) withObject: application afterDelay: 0];
#else
    [self startUnity: application];
#endif
}

// Object ->>>>>>  Call Unity  begin

-(void) changeLanguage{
    
    NSString* resStr = [AppsFlyerProxy getPhoneCountryCodeStr];
    CLog(@"LanguageGetStr === %@",resStr);
    UnitySendMessage("CrossIosObject","ReturnContry", resStr.UTF8String);

}

//GDPR流程结束开始初始化广告
-(void)googleGDPRFinishBeginLoad {
   dispatch_async(dispatch_get_main_queue(), ^{
	   
       [AdManager instanceInitWithTestMode:NO delegate:self applovinKey:@"NBeatPCakspf0wVTSoEPWJpSNeqcosc1EOlVDDJ2HIE_Lyg3f86WM3gLMOANXE2_fXWmuPoDFsxC59tFqGd6ZG"];
	   [AdManager mtgROASWork:YES];
       [AdManager initBanner:AdBannerPosBottom];
       [AdManager showBannerAd:YES];
	   
	       //自己配置
            [AdManager showMaxInterstitialAd:InterstitialEnumSplashEnd];
            [AdManager setOrganicBlockInterstitial:(false)];
            [AdManager setInterstitialInterval:15 firstShowDelay:(15)];
            [AdManager setInterstitialIntervalToVideo:15];
			
	//Debug 这行代码是查看sdk是否集成成功，会弹出一个界面，如果都在completed sdk integrations下面代表成功，提交时不要调用
    //[self performSelector:@selector(showDebuge) withObject:nil afterDelay:2];
			
   });
   

}

-(void) showBanner{
    [AdManager initBanner:AdBannerPosBottom];
    [AdManager showBannerAd:true];

    CLog(@"Banner is show");

}


//激励视频关闭时回调，同时回调激励是否成功参数
- (void)rewardVideoReward:(NSNumber*)obj {
   BOOL result = [(NSNumber *)obj boolValue];
   
   CLog(@"接收到成功奖励的通知 %@",obj);
   CLog(@"视频广告 关闭 ");
    NSString* str;
   if (result) {
	   //成功给奖励
		str = [NSString stringWithUTF8String:"success"];
   }else {
	   //失败不给
		str = [NSString stringWithUTF8String:"failed"];
   }
   
    UnitySendMessage("CrossIosObject", "WatchRewardVideoComplete",  str.UTF8String);
    
    UnityPause(false);
}

- (void)rewardVideoWillShow{
    CLog(@"视频广告 展示 ");
    UnitySendMessage("CrossIosObject", "RewardVideoOpen", "");
    UnityPause(true);
}
- (void)showLoadingRewardVideoWindow{
    UnitySendMessage("CrossIosObject", "ShowLoadingRewardVideoWindow",  "");
}
- (void)hideLoadingRewardVideoWindow {
    CLog(@"loading 完成 展示广告");
    UnitySendMessage("CrossIosObject", "HideLoadingRewardVideoWindow",  "");
}
// ad
- (void)splashAdWillOpen{
    CLog(@"插屏广告 展示 ");
    UnitySendMessage("CrossIosObject", "InterstitialAdOpen",  "");
    UnityPause(true);
}
- (void)splashAdDidClose {
    CLog(@"插屏广告 关闭 ");
    UnitySendMessage("CrossIosObject", "InterstitialAdClose",  "");
    UnityPause(false);
}
- (void)onAppsFlayerReturnIsBuyFb:(BOOL)isBuyFb
{
    CLog(@"isBuyFb is %d", isBuyFb);
}
-(void)onAppsFlyerReturnAdSet:(NSString *)adSet
{
    CLog(@"adSet is %@", adSet);
}


- (void)onAppsFlyerReturn:(BOOL)isOrganic channel:(BOOL)isSafeChannel isSafeCounty:(BOOL)isSafeCounty{
	//isOrganic true自然用户，false 买量用户
	
    isOrganicInt = isOrganic?1:0;
    
    CLog(@"isSafeChannel is %i", isSafeChannel);
    CLog(@"isOrganic is %i", isOrganic);
    CLog(@"isSafeCountry is %i",isSafeCounty);
	
    int status = 0;//模拟买量
	
    //if(isSafeChannel && isSafeCounty) status = 1;
    if(!isOrganic) status = 1;
	
    UnitySendMessage("CrossIosObject", "AppsFlyerState",  [NSString stringWithFormat:@"%d",status].UTF8String );

    if(!isOrganic) {
        NSString *systemCode = [[NSUserDefaults standardUserDefaults] objectForKey:@"AppleLanguages"][0];
            NSArray *codeArray = [systemCode componentsSeparatedByString:@"-"];
            if ([codeArray count] > 0) {
                NSString *languageStr = [NSString stringWithFormat:@"_af_succ_%@",[codeArray firstObject]];
                [AppsFlyerProxy logEvent:[AppsFlyerProxy getEventName:languageStr]];
            }
    }
    
}


- (void)onAppsFlyerReturnFailure:(NSString *)failure
{
    CLog(@"failure is %@", failure);
}
- (void)onAppsFlyerReturnSuccess:(NSDictionary<NSString *,NSString *> *)map
{
    CLog(@"map is %@", map);
}

- (void)showDebuge {
   [[ALSdk shared] showMediationDebugger];
}

- (void)webPageNotification:(NSNotification *)notification {
   NSDictionary *userInfo = notification.userInfo;
   NSString *urlStr = [userInfo objectForKey:@"url"];
   SFSafariViewController *vc = [[SFSafariViewController alloc] initWithURL:[NSURL URLWithString:urlStr]];
   [_window.rootViewController presentViewController:vc animated:YES completion:nil];
}

//GDPR重新展示结果
-(void)onPrivacyOptionsFormShow:(BOOL)isSucceed {
   //展示结束，通知unity
   int status = 1;
	
   if(isSucceed) status = 1;
	
   UnitySendMessage("CrossIosObject", "OnPrivacyOptionsFormShow",  [NSString stringWithFormat:@"%d",status].UTF8String );
}

//这个是AFSDK里的，
- (void)attRequestNotification:(NSNotification *)notification {
   //弹窗结束后继续loading加载
    NSString *content = [notification.userInfo objectForKey:@"content"];
    if([content isEqualToString:@"YES"])
    {
        //同意
    }
    else
    {
        //不同意
    }
   UnitySendMessage("CrossIosObject", "IDFACallBack",  "");
   
   //确保idfa弹窗之后初始化sdk
   dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(2.0 * NSEC_PER_SEC)), dispatch_get_main_queue(), ^{
       [AdManager instancePrepareGDPRWithDelegate:self rootCon:self.window.rootViewController];
   });
}

// Object ->>>>>>  Call Unity  end

- (void)observeValueForKeyPath:(NSString *)keyPath ofObject:(id)object change:(NSDictionary<NSKeyValueChangeKey, id> *)change context:(void *)context
{
    if ([keyPath isEqual: @"outputVolume"])
    {
        UnityUpdateMuteState([[AVAudioSession sharedInstance] outputVolume] < 0.01f ? 1 : 0);
    }
}

- (void)applicationDidEnterBackground:(UIApplication*)application
{
  [AppsFlyerProxy applicationDidEnterBackground];

    ::printf("-> applicationDidEnterBackground()\n");
}

- (void)applicationWillEnterForeground:(UIApplication*)application
{
    ::printf("-> applicationWillEnterForeground()\n");

    // applicationWillEnterForeground: might sometimes arrive *before* actually initing unity (e.g. locking on startup)
    if (_unityAppReady)
    {
        // if we were showing video before going to background - the view size may be changed while we are in background
        [GetAppController().unityView recreateRenderingSurfaceIfNeeded];
    }
}

- (void)applicationDidBecomeActive:(UIApplication*)application
{
	 [AppsFlyerProxy applicationDidBecomeActive];
    ::printf("-> applicationDidBecomeActive()\n");

    [self removeSnapshotViewController];

    if (_unityAppReady)
    {
        if (UnityIsPaused() && _wasPausedExternal == false)
        {
            UnityWillResume();
            UnityPause(0);
        }
        if (_wasPausedExternal)
        {
            if (UnityIsFullScreenPlaying())
                TryResumeFullScreenVideo();
        }
        // need to do this with delay because FMOD restarts audio in AVAudioSessionInterruptionNotification handler
        [self performSelector: @selector(updateUnityAudioOutput) withObject: nil afterDelay: 0.1];
        UnitySetPlayerFocus(1);
    }
    else
    {
        [self initUnityWithApplication: application];
    }

    _didResignActive = false;
}

- (void)updateUnityAudioOutput
{
    UnityUpdateMuteState([[AVAudioSession sharedInstance] outputVolume] < 0.01f ? 1 : 0);
}

- (void)addSnapshotViewController
{
    if (!_didResignActive || self->_snapshotViewController)
    {
        return;
    }

    UIView* snapshotView = [self createSnapshotView];

    if (snapshotView != nil)
    {
        UIViewController* snapshotViewController = [AllocUnityViewController() init];
        snapshotViewController.modalPresentationStyle = UIModalPresentationFullScreen;
        snapshotViewController.view = snapshotView;

        [self->_rootController presentViewController: snapshotViewController animated: false completion: nil];
        self->_snapshotViewController = snapshotViewController;
    }
}

- (void)removeSnapshotViewController
{
    // do this on the main queue async so that if we try to create one
    // and remove in the same frame, this always happens after in the same queue
    dispatch_async(dispatch_get_main_queue(), ^{
        if (self->_snapshotViewController)
        {
            // we've got a view on top of the snapshot view (3rd party plugin/social media login etc).
            if (self->_snapshotViewController.presentedViewController)
            {
                [self performSelector: @selector(removeSnapshotViewController) withObject: nil afterDelay: 0.05];
                return;
            }

            [self->_snapshotViewController dismissViewControllerAnimated: NO completion: nil];
            self->_snapshotViewController = nil;

            // Make sure that the keyboard input field regains focus after the application becomes active.
            [[KeyboardDelegate Instance] becomeFirstResponder];
        }
    });
}

- (void)applicationWillResignActive:(UIApplication*)application
{
    ::printf("-> applicationWillResignActive()\n");

    if (_unityAppReady)
    {
        UnitySetPlayerFocus(0);

        // signal unity that the frame rendering have ended
        // as we will not get the callback from the display link current frame
        UnityDisplayLinkCallback(0);

        _wasPausedExternal = UnityIsPaused();
        if (_wasPausedExternal == false)
        {
            // Pause Unity only if we don't need special background processing
            // otherwise batched player loop can be called to run user scripts.
            if (!UnityGetUseCustomAppBackgroundBehavior())
            {
#if UNITY_SNAPSHOT_VIEW_ON_APPLICATION_PAUSE
                // Force player to do one more frame, so scripts get a chance to render custom screen for minimized app in task manager.
                // NB: UnityWillPause will schedule OnApplicationPause message, which will be sent normally inside repaint (unity player loop)
                // NB: We will actually pause after the loop (when calling UnityPause).
                UnityWillPause();
                [self repaint];
                UnityWaitForFrame();
                [self addSnapshotViewController];
#endif
                UnityPause(1);
            }
        }
    }

    _didResignActive = true;
}

- (void)applicationDidReceiveMemoryWarning:(UIApplication*)application
{
    UnityLowMemory();
}

- (void)applicationWillTerminate:(UIApplication*)application
{
    ::printf("-> applicationWillTerminate()\n");

    // Only clean up if Unity has finished initializing, else the clean up process will crash,
    // this happens if the app is force closed immediately after opening it.
    if (_unityAppReady)
    {
        UnityCleanup();
        UnityCleanupTrampoline();
    }
}

- (void)application:(UIApplication*)application handleEventsForBackgroundURLSession:(nonnull NSString *)identifier completionHandler:(nonnull void (^)())completionHandler
{
    NSDictionary* arg = @{identifier: completionHandler};
    AppController_SendNotificationWithArg(kUnityHandleEventsForBackgroundURLSession, arg);
}

@end

void AppController_SendNotification(NSString* name)
{
    [[NSNotificationCenter defaultCenter] postNotificationName: name object: GetAppController()];
}

void AppController_SendNotificationWithArg(NSString* name, id arg)
{
    [[NSNotificationCenter defaultCenter] postNotificationName: name object: GetAppController() userInfo: arg];
}

void AppController_SendUnityViewControllerNotification(NSString* name)
{
    [[NSNotificationCenter defaultCenter] postNotificationName: name object: UnityGetGLViewController()];
}

extern "C" UIWindow*            UnityGetMainWindow()
{
    return GetAppController().mainDisplay.window;
}

extern "C" UIViewController*    UnityGetGLViewController()
{
    return GetAppController().rootViewController;
}

extern "C" UIView*              UnityGetGLView()
{
    return GetAppController().unityView;
}

extern "C" ScreenOrientation    UnityCurrentOrientation()   { return GetAppController().unityView.contentOrientation; }


bool LogToNSLogHandler(LogType logType, const char* log, va_list list)
{
    NSLogv([NSString stringWithUTF8String: log], list);
    return true;
}

static void AddNewAPIImplIfNeeded();

// From https://stackoverflow.com/questions/4744826/detecting-if-ios-app-is-run-in-debugger
static bool isDebuggerAttachedToConsole(void)
// Returns true if the current process is being debugged (either
// running under the debugger or has a debugger attached post facto).
{
    int                 junk;
    int                 mib[4];
    struct kinfo_proc   info;
    size_t              size;

    // Initialize the flags so that, if sysctl fails for some bizarre
    // reason, we get a predictable result.

    info.kp_proc.p_flag = 0;

    // Initialize mib, which tells sysctl the info we want, in this case
    // we're looking for information about a specific process ID.

    mib[0] = CTL_KERN;
    mib[1] = KERN_PROC;
    mib[2] = KERN_PROC_PID;
    mib[3] = getpid();

    // Call sysctl.

    size = sizeof(info);
    junk = sysctl(mib, sizeof(mib) / sizeof(*mib), &info, &size, NULL, 0);
    assert(junk == 0);

    // We're being debugged if the P_TRACED flag is set.

    return ((info.kp_proc.p_flag & P_TRACED) != 0);
}

void UnityInitTrampoline()
{
    InitCrashHandling();

    NSString* version = [[UIDevice currentDevice] systemVersion];
#define CHECK_VER(s) [version compare: s options: NSNumericSearch] != NSOrderedAscending
    _ios81orNewer  = CHECK_VER(@"8.1");  _ios82orNewer  = CHECK_VER(@"8.2");  _ios83orNewer  = CHECK_VER(@"8.3");
    _ios90orNewer  = CHECK_VER(@"9.0");  _ios91orNewer  = CHECK_VER(@"9.1");
    _ios100orNewer = CHECK_VER(@"10.0"); _ios101orNewer = CHECK_VER(@"10.1"); _ios102orNewer = CHECK_VER(@"10.2"); _ios103orNewer = CHECK_VER(@"10.3");
    _ios110orNewer = CHECK_VER(@"11.0"); _ios111orNewer = CHECK_VER(@"11.1"); _ios112orNewer = CHECK_VER(@"11.2");
    _ios130orNewer  = CHECK_VER(@"13.0"); _ios140orNewer = CHECK_VER(@"14.0"); _ios150orNewer = CHECK_VER(@"15.0");
    _ios160orNewer = CHECK_VER(@"16.0");
#undef CHECK_VER

    AddNewAPIImplIfNeeded();

#if !TARGET_IPHONE_SIMULATOR
    // Use NSLog logging if a debugger is not attached, otherwise we write to stdout.
    if (!isDebuggerAttachedToConsole())
        UnitySetLogEntryHandler(LogToNSLogHandler);
#endif
}

extern "C" bool UnityiOS81orNewer() { return _ios81orNewer; }
extern "C" bool UnityiOS82orNewer() { return _ios82orNewer; }
extern "C" bool UnityiOS90orNewer() { return _ios90orNewer; }
extern "C" bool UnityiOS91orNewer() { return _ios91orNewer; }
extern "C" bool UnityiOS100orNewer() { return _ios100orNewer; }
extern "C" bool UnityiOS101orNewer() { return _ios101orNewer; }
extern "C" bool UnityiOS102orNewer() { return _ios102orNewer; }
extern "C" bool UnityiOS103orNewer() { return _ios103orNewer; }
extern "C" bool UnityiOS110orNewer() { return _ios110orNewer; }
extern "C" bool UnityiOS111orNewer() { return _ios111orNewer; }
extern "C" bool UnityiOS112orNewer() { return _ios112orNewer; }
extern "C" bool UnityiOS130orNewer() { return _ios130orNewer; }
extern "C" bool UnityiOS140orNewer() { return _ios140orNewer; }
extern "C" bool UnityiOS150orNewer() { return _ios150orNewer; }
extern "C" bool UnityiOS160orNewer() { return _ios160orNewer; }

// sometimes apple adds new api with obvious fallback on older ios.
// in that case we simply add these functions ourselves to simplify code
static void AddNewAPIImplIfNeeded()
{
#if !PLATFORM_VISIONOS
    if (![[UIScreen class] instancesRespondToSelector: @selector(maximumFramesPerSecond)])
    {
        IMP UIScreen_MaximumFramesPerSecond_IMP = imp_implementationWithBlock(^NSInteger(id _self) {
            return 60;
        });
        class_replaceMethod([UIScreen class], @selector(maximumFramesPerSecond), UIScreen_MaximumFramesPerSecond_IMP, UIScreen_maximumFramesPerSecond_Enc);
    }

    if (![[UIView class] instancesRespondToSelector: @selector(safeAreaInsets)])
    {
        IMP UIView_SafeAreaInsets_IMP = imp_implementationWithBlock(^UIEdgeInsets(id _self) {
            return UIEdgeInsetsZero;
        });
        class_replaceMethod([UIView class], @selector(safeAreaInsets), UIView_SafeAreaInsets_IMP, UIView_safeAreaInsets_Enc);
    }
#endif
}
