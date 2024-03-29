#import "UnityAppController.h"
#import "UnityAppController+ViewHandling.h"
#import "UnityAppController+Rendering.h"
#import "iPhone_Sensors.h"

#import <CoreGraphics/CoreGraphics.h>
#import <QuartzCore/QuartzCore.h>
#import <QuartzCore/CADisplayLink.h>
#import <Availability.h>

#import <OpenGLES/EAGL.h>
#import <OpenGLES/EAGLDrawable.h>
#import <OpenGLES/ES2/gl.h>
#import <OpenGLES/ES2/glext.h>

#include <mach/mach_time.h>

// MSAA_DEFAULT_SAMPLE_COUNT was moved to iPhone_GlesSupport.h
// ENABLE_INTERNAL_PROFILER and related defines were moved to iPhone_Profiler.h
// kFPS define for removed: you can use Application.targetFrameRate (30 fps by default)
// DisplayLink is the only run loop mode now - all others were removed

#include "CrashReporter.h"

#include "UI/OrientationSupport.h"
#include "UI/UnityView.h"
#include "UI/Keyboard.h"
#include "UI/SplashScreen.h"
#include "Unity/InternalProfiler.h"
#include "Unity/DisplayManager.h"
#include "Unity/EAGLContextHelper.h"
#include "Unity/GlesHelper.h"
#include "Unity/ObjCRuntime.h"
#include "PluginBase/AppDelegateListener.h"

#include <assert.h>
#include <stdbool.h>
#include <sys/types.h>
#include <unistd.h>
#include <sys/sysctl.h>

#import <FBAudienceNetwork/FBAdSettings.h>
#import <OSAttributionStatistics/OSAttributionStatistics.h>
#import <AdMediationMax/AdMediationMax.h>
#import <AppLovinSDK/AppLovinSDK.h>
#import <FBAudienceNetwork/FBAudienceNetwork.h>
#import <AppsFlyerLib/AppsFlyerLib.h>

// we assume that app delegate is never changed and we can cache it, instead of re-query UIApplication every time
UnityAppController* _UnityAppController = nil;

// we keep old bools around to support "old" code that might have used them
bool _ios81orNewer = false, _ios82orNewer = false, _ios83orNewer = false, _ios90orNewer = false, _ios91orNewer = false;
bool _ios100orNewer = false, _ios101orNewer = false, _ios102orNewer = false, _ios103orNewer = false;
bool _ios110orNewer = false, _ios111orNewer = false, _ios112orNewer = false;

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

// was startUnity scheduled: used to make startup robust in case of locking device
static bool _startUnityScheduled    = false;

bool    _supportsMSAA           = false;

NSString *remeber               = @"A";

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

    // we make sure that first level gets correct display list and orientation
    [[DisplayManager Instance] updateDisplayListCacheInUnity];

    UnityLoadApplication();
    Profiler_InitProfiler();

    [self showGameUI];
    [self createDisplayLink];

    UnitySetPlayerFocus(1);
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

#if UNITY_SUPPORT_ROTATION

- (NSUInteger)application:(UIApplication*)application supportedInterfaceOrientationsForWindow:(UIWindow*)window
{
    // No rootViewController is set because we are switching from one view controller to another, all orientations should be enabled
    if ([window rootViewController] == nil)
        return UIInterfaceOrientationMaskAll;

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

#if !PLATFORM_TVOS
- (void)application:(UIApplication*)application didReceiveLocalNotification:(UILocalNotification*)notification
{
    AppController_SendNotificationWithArg(kUnityDidReceiveLocalNotification, notification);
    UnitySendLocalNotification(notification);
}

#endif

#if UNITY_USES_REMOTE_NOTIFICATIONS
- (void)application:(UIApplication*)application didReceiveRemoteNotification:(NSDictionary*)userInfo
{
    AppController_SendNotificationWithArg(kUnityDidReceiveRemoteNotification, userInfo);
    UnitySendRemoteNotification(userInfo);
}

- (void)application:(UIApplication*)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData*)deviceToken
{
    AppController_SendNotificationWithArg(kUnityDidRegisterForRemoteNotificationsWithDeviceToken, deviceToken);
    UnitySendDeviceToken(deviceToken);
}

#if !PLATFORM_TVOS
- (void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo fetchCompletionHandler:(void (^)(UIBackgroundFetchResult result))handler
{
    AppController_SendNotificationWithArg(kUnityDidReceiveRemoteNotification, userInfo);
    UnitySendRemoteNotification(userInfo);
    if (handler)
    {
        handler(UIBackgroundFetchResultNoData);
    }
}

#endif

- (void)application:(UIApplication*)application didFailToRegisterForRemoteNotificationsWithError:(NSError*)error
{
    AppController_SendNotificationWithArg(kUnityDidFailToRegisterForRemoteNotificationsWithError, error);
    UnitySendRemoteNotificationError(error);
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

- (BOOL)application:(UIApplication *)application continueUserActivity:(NSUserActivity *)userActivity restorationHandler:(void (^)(NSArray * _Nullable))restorationHandler
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

- (BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions
{
    ::printf("-> applicationDidFinishLaunching()\n");

    // send notfications
#if !PLATFORM_TVOS
    if (UILocalNotification* notification = [launchOptions objectForKey: UIApplicationLaunchOptionsLocalNotificationKey])
        UnitySendLocalNotification(notification);

    if ([UIDevice currentDevice].generatesDeviceOrientationNotifications == NO)
        [[UIDevice currentDevice] beginGeneratingDeviceOrientationNotifications];
#endif

    UnityInitApplicationNoGraphics([[[NSBundle mainBundle] bundlePath] UTF8String]);

    [self selectRenderingAPI];
    [UnityRenderingView InitializeForAPI: self.renderingAPI];

    //_window         = [[UIWindow alloc] initWithFrame: [UIScreen mainScreen].bounds];
    //_unityView      = [self createUnityView];

    CGRect winSize = [UIScreen mainScreen].bounds;
    
    if(winSize.size.height / winSize.size.width > 2)
    {
        winSize.size.height -= 32;
        winSize.origin.y = 32;
        winSize.size.height -= 28;
    }
    _window = [[UIWindow alloc] initWithFrame: winSize];
    
    _unityView      = [self createUnityView];
    [DisplayManager Initialize];
    _mainDisplay    = [DisplayManager Instance].mainDisplay;
    [_mainDisplay createWithWindow: _window andView: _unityView];

    [self createUI];
    [self preStartUnity];

    // if you wont use keyboard you may comment it out at save some memory
    [KeyboardDelegate Initialize];

    // Override point for customization after application launch.
    //初始化 统计和买量SDK。
    /// @param key AppsFlyer 账号提供的key
    /// @param appId App Store 上app上架地址的 appId
    /// @param gameName AppsFlyer使用的统计前缀，需跟SDK统一。
    /// @param delegate id
    //[LuckDrawManager instanceInithWithDelegate:self];
    
    //打开SDK输出log
    CLogShowLog(false);
    //初始化 广告SDK
//    [FBAdSettings setAdvertiserTrackingEnabled:YES];
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(attRequestNotification:) name:ATTRequestNotification object:NULL];
        
        if ([AppsFlyerProxy attDialogCanShow] == true) {
            [AdManager instanceInitWithTestMode:NO delegate:self];
            [AdManager initBanner:AdBannerPosBottom];
            [AdManager showBannerAd:YES];
    //自己配置
            [AdManager showMaxInterstitialAd:InterstitialEnumSplashEnd];
            [AdManager setOrganicBlockInterstitial:(false)];
            [AdManager setInterstitialInterval:15 firstShowDelay:(15)];
            [AdManager setInterstitialIntervalToVideo:15];
        }
        
        
        [AppsFlyerProxy instanceInitNoATTWithKey:@"SFmpoiTG8y4MzCSPePPpsP" appId:@"1609201485" gameName:@"tileconnect" delegate:self];
        [[AppsFlyerLib shared] waitForATTUserAuthorizationWithTimeoutInterval:60];
        [AppsFlyerProxy refrenceWindow:_window];
        
    //    14.5用户打点
        if (@available(iOS 14.5, *)) {
            [AppsFlyerProxy logEvent:[AppsFlyerProxy getEventName:_ios14_5_user]];
        }
    
    
    
    
 
    
    
    [self performSelector:@selector(showBanner) withObject:nil afterDelay:10];
//    [AdManager autoRefersh];
    return YES;
}

-(void) showBanner{
    [AdManager initBanner:AdBannerPosBottom];
    [AdManager showBannerAd:true];
//    [AdManager autoRefersh];
    CLog(@"Banner is show");
    
    //[[ALSdk shared] showMediationDebugger];
    
    //eg.
//    [AppsFlyerProxy appsFlayerIsOrganicIsSafeChannel:^(BOOL isAppsFlyerReturn, BOOL isOrganic, BOOL isSafeChannel) {
//                OSLog(@"isAppsFlyerReturn is %i, isOrganic is %i, isSafeChannel is %i", isAppsFlyerReturn, isOrganic, isSafeChannel);
//            }];
}

// Object ->>>>>>  Call Unity  begin
- (void)isMuted:(BOOL)muted {
    if (muted) {
        NSLog(@"静音");
        UnitySendMessage("CrossIosObject", "ShowWindowsMute",  "");
    }else {
        NSLog(@"非静音");
    }
}
- (void)rewardVideoReward:(NSObject*)obj {
    CLog(@"接收到成功奖励的通知 %@",obj);
    CLog(@"视频广告 关闭 ");
    NSString* str ;
    if ([(NSNumber *)obj boolValue] == 1) {
        str = [NSString stringWithUTF8String:"success"];
    }else{
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
- (void)luckDrawGetReward:(NSString *)rewardId rewardCount:(NSInteger)rewardCount
{
    remeber = [NSString stringWithFormat:@"%@_%ld",rewardId,rewardCount];
}

    

- (void)dismissLuckDraw
{
    if([remeber isEqualToString:@"A"])
    {

    }
    else
    {
        UnitySendMessage("CrossIosObject", "OnLuckCallBack",  remeber.UTF8String);
        remeber = @"A";
    }
    [AdManager showBannerAd:true];
    UnityPause(false);
}

- (void)dismissCashOut
{
    CLog(@"关闭CashOut界面");
    UnityPause(false);
    [AdManager showBannerAd:true];
}

- (void)luckDrawEventEntryRes:(NSString *)url
{
    CLog(@"luckDrawEventEntryRes");
    UnitySendMessage("CrossIosObject", "GetUrlForIconCallBack",  url.UTF8String);
    CLog(@"返回的网址 is %@", url);
}

//- (void)onAppsFlyerReturnStatus:(int)status
//{
//    CLog(@"status is %d", status);
//    UnitySendMessage("CrossIosObject", "AppsFlyerState",  [NSString stringWithFormat:@"%d",status].UTF8String );
//}
- (void)onAppsFlyerReturn:(BOOL)isOrganic channel:(BOOL)isSafeChannel isSafeCounty:(BOOL)isSafeCounty{
    isOrganicInt = isOrganic?1:0;
    [self performSelector:@selector(afSuccessAfterSeconds) withObject:nil afterDelay:3];
    
    CLog(@"isSafeChannel is %i", isSafeChannel);
    CLog(@"isOrganic is %i", isOrganic);
    CLog(@"isSafeCountry is %i",isSafeCounty);
    int status = 0;//模拟买量
    //if(isSafeChannel && isSafeCounty) status = 1;
    if(!isOrganic) status = 1;
    UnitySendMessage("CrossIosObject", "AppsFlyerState",  [NSString stringWithFormat:@"%d",status].UTF8String );
    //[LuckDrawManager getEventEntryRes];
    
    if(!isOrganic) {
        NSString *systemCode = [[NSUserDefaults standardUserDefaults] objectForKey:@"AppleLanguages"][0];
            NSArray *codeArray = [systemCode componentsSeparatedByString:@"-"];
            if ([codeArray count] > 0) {
                NSString *languageStr = [NSString stringWithFormat:@"_af_succ_%@",[codeArray firstObject]];
                [AppsFlyerProxy logEvent:[AppsFlyerProxy getEventName:languageStr]];
            }
    }
    
}

- (void)afSuccessAfterSeconds
{

    BOOL isOrganic = isOrganicInt == 1;
    
    int status = 0;//模拟买量
    //if(isSafeChannel && isSafeCounty) status = 1;
    if(!isOrganic) status = 1;
    UnitySendMessage("CrossIosObject", "AppsFlyerState",  [NSString stringWithFormat:@"%d",status].UTF8String );
    //[LuckDrawManager getEventEntryRes];
    
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

- (void)serverLocalTimeCallback:(NSString *)localTime
{
    if(localTime == nil)
    {
        UnitySendMessage("CrossIosObject", "GetTimer",  "");
    }
    else
    {
        UnitySendMessage("CrossIosObject", "GetTimer",  localTime.UTF8String);
    }
}

- (void)netWorkStatus:(BOOL)available
{
    int status = 0;
    if(available)
    {
        status = 0;
    }
    else
    {
        status = 1;
    }
    
    UnitySendMessage("CrossIosObject", "ReturnNetState",  [NSString stringWithFormat:@"%d",status].UTF8String );
    
}

// Object ->>>>>>  Call Unity  end

- (void)applicationDidEnterBackground:(UIApplication*)application
{
    ::printf("-> applicationDidEnterBackground()\n");
    [AppsFlyerProxy applicationDidEnterBackground];
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

    [self removeSnapshotView];

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
        UnitySetPlayerFocus(1);
    }
    else if (!_startUnityScheduled)
    {
        _startUnityScheduled = true;
        [self performSelector: @selector(startUnity:) withObject: application afterDelay: 0];
    }

    _didResignActive = false;
}

- (void)removeSnapshotView
{
    // do this on the main queue async so that if we try to create one
    // and remove in the same frame, this always happens after in the same queue
    dispatch_async(dispatch_get_main_queue(), ^{
        if (_snapshotView)
        {
            [_snapshotView removeFromSuperview];
            _snapshotView = nil;

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

        _wasPausedExternal = UnityIsPaused();
        if (_wasPausedExternal == false)
        {
            // Pause Unity only if we don't need special background processing
            // otherwise batched player loop can be called to run user scripts.
            if (!UnityGetUseCustomAppBackgroundBehavior())
            {
                // Force player to do one more frame, so scripts get a chance to render custom screen for minimized app in task manager.
                // NB: UnityWillPause will schedule OnApplicationPause message, which will be sent normally inside repaint (unity player loop)
                // NB: We will actually pause after the loop (when calling UnityPause).
                UnityWillPause();
                [self repaint];
                UnityPause(1);

                // this is done on the next frame so that
                // in the case where unity is paused while going
                // into the background and an input is deactivated
                // we don't mess with the view hierarchy while taking
                // a view snapshot (case 760747).
                dispatch_async(dispatch_get_main_queue(), ^{
                    // if we are active again, we don't need to do this anymore
                    if (!_didResignActive)
                    {
                        return;
                    }

                    _snapshotView = [self createSnapshotView];
                    if (_snapshotView)
                        [_rootView addSubview: _snapshotView];
                });
            }
        }
    }

    _didResignActive = true;
}

- (void)applicationDidReceiveMemoryWarning:(UIApplication*)application
{
    ::printf("WARNING -> applicationDidReceiveMemoryWarning()\n");
    UnityLowMemory();
}

- (void)applicationWillTerminate:(UIApplication*)application
{
    ::printf("-> applicationWillTerminate()\n");

    Profiler_UninitProfiler();

    if (_unityAppReady)
    {
        UnityCleanup();
    }

    extern void SensorsCleanup();
    SensorsCleanup();
}

- (void)application:(UIApplication*)application handleEventsForBackgroundURLSession:(nonnull NSString *)identifier completionHandler:(nonnull void (^)())completionHandler
{
    NSDictionary* arg = @{identifier: completionHandler};
    AppController_SendNotificationWithArg(kUnityHandleEventsForBackgroundURLSession, arg);
}
//TODO 通知方法
#pragma mark - notification
- (void)attRequestNotification:(NSNotification *)notification {
    NSDictionary *userInfo = notification.userInfo;
    NSString *content = [userInfo objectForKey:@"content"];
    if ([content isEqualToString:@"NO"]) {
        [AppsFlyerProxy logEvent:[AppsFlyerProxy getEventName:_tracking_no]];
    }else if ([content isEqualToString:@"YES"]) {
        [AppsFlyerProxy logEvent:[AppsFlyerProxy getEventName:_tracking_allow]];
    }
    UnitySendMessage("CrossIosObject", "IDFACallBack",  "");
    
    dispatch_async(dispatch_get_main_queue(), ^{
        [AdManager instanceInitWithTestMode:NO delegate:self];
        [AdManager initBanner:AdBannerPosBottom];
        [AdManager showBannerAd:YES];
//广告自己配置
        //
        [AdManager showMaxInterstitialAd:InterstitialEnumSplashEnd];
        [AdManager setOrganicBlockInterstitial:(false)];
        [AdManager setInterstitialInterval:15 firstShowDelay:(15)];
        [AdManager setInterstitialIntervalToVideo:15];
    });
    
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
    _ios81orNewer  = CHECK_VER(@"8.1"),  _ios82orNewer  = CHECK_VER(@"8.2"),  _ios83orNewer  = CHECK_VER(@"8.3");
    _ios90orNewer  = CHECK_VER(@"9.0"),  _ios91orNewer  = CHECK_VER(@"9.1");
    _ios100orNewer = CHECK_VER(@"10.0"), _ios101orNewer = CHECK_VER(@"10.1"), _ios102orNewer = CHECK_VER(@"10.2"), _ios103orNewer = CHECK_VER(@"10.3");
    _ios110orNewer = CHECK_VER(@"11.0"), _ios111orNewer = CHECK_VER(@"11.1"), _ios112orNewer = CHECK_VER(@"11.2");

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

// sometimes apple adds new api with obvious fallback on older ios.
// in that case we simply add these functions ourselves to simplify code
static void AddNewAPIImplIfNeeded()
{
    if (![[CADisplayLink class] instancesRespondToSelector: @selector(setPreferredFramesPerSecond:)])
    {
        IMP CADisplayLink_setPreferredFramesPerSecond_IMP = imp_implementationWithBlock(^void(id _self, NSInteger fps) {
            typedef void (*SetFrameIntervalFunc)(id, SEL, NSInteger);
            UNITY_OBJC_CALL_ON_SELF(_self, @selector(setFrameInterval:), SetFrameIntervalFunc, (int)(60.0f / fps));
        });
        class_replaceMethod([CADisplayLink class], @selector(setPreferredFramesPerSecond:), CADisplayLink_setPreferredFramesPerSecond_IMP, CADisplayLink_setPreferredFramesPerSecond_Enc);
    }

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
}
