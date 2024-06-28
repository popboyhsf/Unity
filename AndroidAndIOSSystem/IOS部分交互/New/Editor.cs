using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using System.Text.RegularExpressions;

#if UNITY_IOS || UNITY_EDITOR
public class XcodeBuildPostprocessor
{

    private static string unityiPhoneStr = "/Unity-iPhone.xcodeproj/project.pbxproj";

    // [PostProcessBuildAttribute(88)]
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            UnityEngine.Debug.Log("XCodePostProcess: Starting to perform post build tasks for iOS platform.");
            ReplaceNativeCodeFile(path);
            ProjectSetting(path);
            InfoPlist(path);
            EditNativeCode(path);

        }
    }
    private static void ProjectSetting(string path)
    {
        // 主要官方文档  https://docs.unity3d.com/cn/2018.4/ScriptReference/iOS.Xcode.PBXProject.html
        string projPath = path + unityiPhoneStr;
        PBXProject proj = new PBXProject();
        proj.ReadFromFile(projPath);

        string mainTarget = proj.GetUnityMainTargetGuid();
        string target = proj.GetUnityFrameworkTargetGuid();
        proj.AddFrameworkToProject(target, "AdSupport.framework", false);//true optional false required
        proj.AddFrameworkToProject(target, "iAd.framework", false);
        proj.AddFrameworkToProject(target, "AdServices.framework", true);

        proj.AddFrameworkToProject(target, "CoreMotion.framework", false);
        proj.AddFrameworkToProject(target, "SafariServices.framework", false);
        proj.AddFrameworkToProject(target, "AppTrackingTransparency.framework", true);
        proj.AddFrameworkToProject(target, "CoreTelephony.framework", false);
        proj.AddFrameworkToProject(target, "StoreKit.framework", true);
        proj.AddFrameworkToProject(target, "AudioToolbox.framework", false);
        proj.AddFrameworkToProject(target, "Foundation.framework", false);
        proj.AddFrameworkToProject(target, "SystemConfiguration.framework", false);
        proj.AddFrameworkToProject(target, "AVFoundation.framework", false);
        proj.AddFrameworkToProject(target, "MessageUI.framework", false);
        proj.AddFrameworkToProject(target, "UIKit.framework", false);
        proj.AddFrameworkToProject(target, "CoreGraphics.framework", false);
        proj.AddFrameworkToProject(target, "WebKit.framework", true);
        proj.AddFrameworkToProject(target, "CoreMedia.framework", false);
        proj.AddFrameworkToProject(target, "JavaScriptCore.framework", false);
        proj.AddFrameworkToProject(target, "CoreServices.framework", false);
        proj.AddFrameworkToProject(target, "Social.framework", true);
        proj.AddFrameworkToProject(target, "WatchConnectivity.framework", true);
        proj.AddFrameworkToProject(target, "QuartzCore.framework", false);
        proj.AddFrameworkToProject(target, "MobileCoreServices.framework", false);
        proj.AddFrameworkToProject(target, "MediaPlayer.framework", false);
        proj.AddFrameworkToProject(target, "Accelerate.framework", false);
        proj.AddFrameworkToProject(target, "Security.framework", false);
        proj.AddFrameworkToProject(target, "DeviceCheck.framework", false);
        proj.AddFrameworkToProject(target, "ImageIO.framework", false);
        proj.AddFrameworkToProject(target, "CoreFoundation.framework", false);
        proj.AddFrameworkToProject(target, "CoreLocation.framework", false);
        proj.AddFrameworkToProject(target, "CFNetwork.framework", false);
        proj.AddFrameworkToProject(target, "PassKit.framework", false);
        proj.AddFrameworkToProject(target, "Network.framework", true);

        proj.AddFrameworkToProject(target, "CoreImage.framework", true);//这是新加的
        proj.AddFrameworkToProject(target, "CoreText.framework", true);//这是新加的
        proj.AddFrameworkToProject(target, "MapKit.framework", true);//这是新加的
        proj.SetBuildProperty(target, "ENABLE_BITCODE", "false");

        proj.SetBuildProperty(target, "CLANG_MODULES_AUTOLINK", "YES");
        proj.AddBuildProperty(mainTarget, "FRAMEWORK_SEARCH_PATHS", "$(SRCROOT)/DropIn1");
        proj.AddBuildProperty(mainTarget, "FRAMEWORK_SEARCH_PATHS", "$(SRCROOT)/DropIn1/DependFramework");
        proj.AddBuildProperty(target, "FRAMEWORK_SEARCH_PATHS", "$(SRCROOT)/DropIn1/DependFramework");
        proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");
        proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-fobjc-arc");
        proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-l\"c++\"");
        proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-l\"c++abi\"");
        proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-l\"sqlite3\"");
        proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-l\"z\"");
        proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-l\"xml2\"");//这是新加的
        AddLibToProject(proj, target, "libz.tbd");
        AddLibToProject(proj, target, "libsqlite3.0.tbd");
        AddLibToProject(proj, target, "libSwiftWebKit.tbd");
        AddLibToProject(proj, target, "libxml2.2.tbd");
        AddLibToProject(proj, target, "libsqlite3.tbd");
        AddLibToProject(proj, target, "libresolv.9.tbd");
        AddLibToProject(proj, target, "libc++.tbd");
        AddLibToProject(proj, target, "libbz2.tbd");
        AddLibToProject(proj, target, "libxml2.tbd");
        AddLibToProject(proj, target, "libiconv.tbd");
        AddLibToProject(proj, target, "libc++abi.tbd");
        AddLibToProject(proj, target, "libz.1.2.5.tbd");

        // 保存
        File.WriteAllText(projPath, proj.WriteToString());
    }
    // 添加lib方法
    private static void AddLibToProject(PBXProject proj, string target, string lib)
    {
        string file = proj.AddFile("usr/lib/" + lib, "Frameworks/" + lib, PBXSourceTree.Sdk);
        proj.AddFileToBuild(target, file);
    }
    // 修改Info.Plist
    private static void InfoPlist(string path)
    {
        string plistPath = path + "/Info.plist";
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));
        // Get root
        PlistElementDict infoDict = plist.root;

        infoDict.SetString("NSUserTrackingUsageDescription", "This only uses device info for less annoying, more relevant ads");


        var appTransportSecurityKey = "NSAppTransportSecurity";
        PlistElementDict appTransportSecurityDic = infoDict.CreateDict(appTransportSecurityKey);
        appTransportSecurityDic.SetBoolean("NSAllowsArbitraryLoads", true);
        //添加applovin配置
        var appLovinConsentFlowInfoKey = "AppLovinConsentFlowInfo";
        PlistElementDict appLovinConsentFlowInfoDic = infoDict.CreateDict(appLovinConsentFlowInfoKey);
        appLovinConsentFlowInfoDic.SetString("AppLovinConsentFlowPrivacyPolicy", "https://www.applovin.com/privacy/");
        appLovinConsentFlowInfoDic.SetBoolean("AppLovinConsentFlowEnabled", true);

        //添加google id
        infoDict.SetString("GADApplicationIdentifier", "TODO");

        //添加adcolony的schemes配置
        var applicationQueriesSchemesKey = "LSApplicationQueriesSchemes";
        PlistElementArray applicationQueriesSchemesArr = infoDict.CreateArray(applicationQueriesSchemesKey);
        applicationQueriesSchemesArr.AddString("fb");
        applicationQueriesSchemesArr.AddString("instagram");
        applicationQueriesSchemesArr.AddString("tumblr");
        applicationQueriesSchemesArr.AddString("twitter");

        //添加NSAdvertisingAttributionReportEndpoint
        infoDict.SetString("NSAdvertisingAttributionReportEndpoint", "https://postbacks-app.com");

        //SKAdNetworkItems
        var adNetworkItemsKey = "SKAdNetworkItems";
        PlistElementArray adNetworkItemsArr = infoDict.CreateArray(adNetworkItemsKey);
        string[] list = { "TODO" };
        for(int i = 0; i < list.Length; i++)
        {
            PlistElementDict dictemp = adNetworkItemsArr.AddDict();
            dictemp.SetString("SKAdNetworkIdentifier", list[i]);
        }

        infoDict.SetString("NSPhotoLibraryUsageDescription", "Please allow the use of photo albums to interact with you when displaying ads");
        infoDict.SetString("NSCameraUsageDescription", "Please allow the camera to interact with you when displaying ads");
        infoDict.SetString("NSMotionUsageDescription", "Some ad content may require access to accelerometer for interactive ad experience.");
        infoDict.SetString("Privacy - Photo Library Additions Usage Description", "Please allow the use of photo albums to interact with you when displaying ads");

        File.WriteAllText(plistPath, plist.WriteToString());
    }
    private static void ReplaceNativeCodeFile(string path)
    {
        string projPath = path + unityiPhoneStr;
        PBXProject proj = new PBXProject();
        proj.ReadFromFile(projPath);
        string mainTarget = proj.GetUnityMainTargetGuid();
        string unityTarget = proj.GetUnityFrameworkTargetGuid();

        string serviceSetupPath = Application.dataPath + "/Editor/AppLovinQualityServiceSetup-ios.rb";
        string infoPath = Application.dataPath + "/Editor/adInfo.dat";
        string messageFilePath = Application.dataPath + "/Editor/MessageFile.m";
        string serviceSetupToPath = path + "/AppLovinQualityServiceSetup-ios.rb";
        string infoToPath = path + "/adInfo.dat";
        string messageFileToPath = path + "/Classes/MessageFile.m";
        if (!File.Exists(serviceSetupToPath))
        {
            //File.Delete(serviceSetupToPath);
            File.Copy(serviceSetupPath, serviceSetupToPath);
        }
        if (!File.Exists(infoToPath))
        {
            //File.Delete(infoToPath);
            File.Copy(infoPath, infoToPath);

            string adinfoGuid = proj.AddFile("adInfo.dat", "adInfo.dat", PBXSourceTree.Source);
            proj.AddFileToBuild(mainTarget, adinfoGuid);
        }
        if (!File.Exists(messageFileToPath))
        {
            //File.Delete(messageFileToPath);
            File.Copy(messageFilePath, messageFileToPath);

            string messageCombinePath = Path.Combine("Classes", "MessageFile.m");
            string messageGuid = proj.AddFile(messageCombinePath, messageCombinePath, PBXSourceTree.Source);
            var sourcesBuildPhase = proj.GetSourcesBuildPhaseByTarget(unityTarget);
            proj.AddFileToBuildSection(unityTarget, sourcesBuildPhase, messageGuid);
        }
        
        File.WriteAllText(projPath, proj.WriteToString());
    }
    private static void EditNativeCode(string filePath)
    {
        //Preprocessor.h文件
        XClass controllerH = new XClass(filePath + "/Classes/UnityAppController.h");
        controllerH.WriteBelowCode("@class DisplayConnection;", "#import <OSAttributionStatistics/OSAttributionStatistics.h>\n#import <AppLovinSDK/AppLovinSDK.h>\n#import <AdMediationMax/AdMediationMax.h>\n#import <SafariServices/SafariServices.h>");
        controllerH.ReplaceString("@interface UnityAppController : NSObject<UIApplicationDelegate>", "@interface UnityAppController : NSObject<UIApplicationDelegate, AppsFlyerConversionListener, AdManagerDelegate>");

        XClass controllerM = new XClass(filePath + "/Classes/UnityAppController.mm");
        string str = "\n    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(attRequestNotification:) name:ATTRequestNotification object:NULL];\n";
        str = str + "   [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(webPageNotification:) name:@\"iOSWebPageShow\" object:NULL];\n\n";
        str = str + "   CLogShowLog(true);\n";
        str = str + "   //带vc的打点事件，如果需要的话suffixCode每次加1\n";
        str = str + "   [AppsFlyerProxy versionSuffixList:@[@\"ad_request\",@\"ad_show\",@\"af_succ\",@\"luck_balance\",@\"interstitial_show\",@\"video_show\",@\"balance_display\",@\"loading_finish\",@\"loading_timeout\"] prefixList:@[@\"lang\"] suffixCode:1];\n";
        str = str + "   //appId是应用的id，gameName是统计前缀\n";
        str = str + "   [AppsFlyerProxy instanceInitNoATTWithKey:@\"TODO\" appId: @\"\" gameName: @\"\" delegate:self];\n";
        str = str + "   [AppsFlyerProxy refrenceWindow:self.window];\n";
        str = str + "   [AppsFlyerProxy firstCheckSystemIdfaSet];\n\n";
        str = str + "   //获取国家，通知unity\n";
        str = str + "   NSString *countryStr = [AppsFlyerProxy getPhoneCountryEnum];\n";
        str = str + "   const char* charStr = countryStr.UTF8String;\n";
        str = str + "   UnitySendMessage(\"\", \"\", charStr);\n\n";
        str = str + "   //确保idfa提示后初始化\n";
        str = str + "   if ([AppsFlyerProxy attDialogCanShow] == true) {\n";
        str = str + "       dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(2.0 * NSEC_PER_SEC)), dispatch_get_main_queue(), ^{\n";
        str = str + "           [AdManager instancePrepareGDPRWithDelegate:self rootCon:self.window.rootViewController];\n";
        str = str + "       });\n";
        str = str + "   }\n\n";
        controllerM.WriteBelowCode("[KeyboardDelegate Initialize];", str);

        str = "- (void)showDebuge {\n";
        str = str + "   [[ALSdk shared] showMediationDebugger];\n";
        str = str + "}\n";
        str = str + "- (void)webPageNotification:(NSNotification *)notification {\n";
        str = str + "   NSDictionary *userInfo = notification.userInfo;\n";
        str = str + "   NSString *urlStr = [userInfo objectForKey:@\"url\"];\n";
        str = str + "   SFSafariViewController *vc = [[SFSafariViewController alloc] initWithURL:[NSURL URLWithString:urlStr]];\n";
        str = str + "   [_window.rootViewController presentViewController:vc animated:YES completion:nil];\n";
        str = str + "}\n";
        str = str + "#pragma mark - AppsFlyerConversionListener\n";
        str = str + "- (void)onAppsFlyerReturn:(BOOL)isOrganic channel:(BOOL)isSafeChannel isSafeCounty:(BOOL)isSafeCounty;\n{\n";
        str = str + "   //isOrganic true自然用户，false 买量用户\n";
        str = str + "   UnitySendMessage(\"\", \"\", \"\");\n";
        str = str + "}\n";
        str = str + "#pragma mark - AdManagerDelegate\n";
        str = str + "//插屏广告即将展示\n";
        str = str + "- (void)splashAdWillOpen {\n\n";
        str = str + "}\n";
        str = str + "//插屏广告展示结束。\n";
        str = str + "- (void)splashAdDidClose {\n\n";
        str = str + "}\n";
        str = str + "//激励视频即将显示\n";
        str = str + "- (void)rewardVideoWillShow {\n\n";
        str = str + "}\n";
        str = str + "//激励视频关闭时回调，同时回调激励是否成功参数\n";
        str = str + "- (void)rewardVideoReward:(NSNumber*)obj {\n";
        str = str + "   BOOL result = [(NSNumber *)obj boolValue];\n";
        str = str + "   if (result) {//成功给奖励\n\n";
        str = str + "   }else {//失败不给\n\n";
        str = str + "   }\n";
        str = str + "}\n";
        str = str + "//GDPR流程结束开始初始化广告\n";
        str = str + "-(void)googleGDPRFinishBeginLoad {\n";
        str = str + "   dispatch_async(dispatch_get_main_queue(), ^{\n";
        str = str + "       [AdManager instanceInitWithTestMode:NO delegate:self];\n";
        str = str + "       [AdManager initBanner:AdBannerPosBottom];\n";
        str = str + "       [AdManager showBannerAd:YES];\n";
        str = str + "       //这行代码是查看sdk是否集成成功，会弹出一个界面，如果都在completed sdk integrations下面代表成功，提交时不要调用\n";
        str = str + "//     [self performSelector:@selector(showDebuge) withObject:nil afterDelay:10];\n";
        str = str + "   });\n";
        str = str + "}\n";
        str = str + "//GDPR重新展示结果\n";
        str = str + "-(void)onPrivacyOptionsFormShow:(BOOL)isSucceed {\n";
        str = str + "   //展示结束，通知unity\n";
        str = str + "   UnitySendMessage(\"\", \"\", isSucceed?\"YES\":\"NO\");\n";
        str = str + "}\n";
        str = str + "#pragma mark - NSNotification\n";
        str = str + "//这个是AFSDK里的，\n";
        str = str + "- (void)attRequestNotification:(NSNotification *)notification {\n";
        str = str + "   //弹窗结束后继续loading加载\n";
        str = str + "   UnitySendMessage(\"\", \"\",  \"\");\n";
        str = str + "   //确保idfa弹窗之后初始化sdk\n";
        str = str + "   dispatch_async(dispatch_get_main_queue(), ^{\n";
        str = str + "       [AdManager instancePrepareGDPRWithDelegate:self rootCon:self.window.rootViewController];\n";
        str = str + "   });\n";
        str = str + "}\n";
        str = str + "@end";
        controllerM.ReplaceString("@end", str);

        controllerM.WriteBelowCode("::printf(\"-> applicationDidEnterBackground()\\n\");", "  [AppsFlyerProxy applicationDidEnterBackground];");

        controllerM.WriteBelowCode("::printf(\"-> applicationDidBecomeActive()\\n\");", " [AppsFlyerProxy applicationDidBecomeActive];");
    }


    // 定义文件更新类
    public partial class XClass : System.IDisposable
    {

        private string filePath;

        public XClass(string fPath) //通过文件路径初始化对象
        {
            filePath = fPath;
            if (!System.IO.File.Exists(filePath))
            {
                Debug.LogError(filePath + "该文件不存在,请检查路径!");
                return;
            }
        }
        // 替换某些字符串
        public void ReplaceString(string oldStr, string newStr, string method = "")
        {
            if (!File.Exists(filePath))
            {

                return;
            }
            bool getMethod = false;
            string[] codes = File.ReadAllLines(filePath);
            for (int i = 0; i < codes.Length; i++)
            {
                string str = codes[i].ToString();
                if (string.IsNullOrEmpty(method))
                {
                    if (str.Contains(oldStr)) codes.SetValue(newStr, i);
                }
                else
                {
                    if (!getMethod)
                    {
                        getMethod = str.Contains(method);
                    }
                    if (!getMethod) continue;
                    if (str.Contains(oldStr))
                    {
                        codes.SetValue(newStr, i);
                        break;
                    }
                }
            }
            File.WriteAllLines(filePath, codes);
        }

        // 在某一行后面插入代码
        public void WriteBelowCode(string below, string text)
        {
            StreamReader streamReader = new StreamReader(filePath);
            string text_all = streamReader.ReadToEnd();
            streamReader.Close();

            int beginIndex = text_all.IndexOf(below);
            if (beginIndex == -1)
            {

                return;
            }

            int endIndex = text_all.LastIndexOf("\n", beginIndex + below.Length);

            text_all = text_all.Substring(0, endIndex) + "\n" + text + "\n" + text_all.Substring(endIndex);

            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(text_all);
            streamWriter.Close();
        }
        public void Dispose()
        {

        }
    }
}
#endif