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

        proj.SetBuildProperty(target, "ENABLE_BITCODE", "false");
        proj.SetBuildProperty(mainTarget, "ENABLE_BITCODE", "false");
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

        //添加applovin id
        infoDict.SetString("AppLovinSdkKey", "TODO");

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
		//TODO
        string[] list = { "22mmun2rn5.skadnetwork","238da6jt44.skadnetwork","24t9a8vw3c.skadnetwork","24zw6aqk47.skadnetwork",
        "252b5q8x7y.skadnetwork","275upjj5gd.skadnetwork","294l99pt4k.skadnetwork","2fnua5tdw4.skadnetwork",
        "2q884k2j68.skadnetwork","2rq3zucswp.skadnetwork","2tdux39lx8.skadnetwork","2u9pt9hc89.skadnetwork",
        "32z4fx6l9h.skadnetwork","33r6p7g8nc.skadnetwork","3cgn6rq224.skadnetwork","3l6bd9hu43.skadnetwork",
        "3qcr597p9d.skadnetwork","3qy4746246.skadnetwork","3rd42ekr43.skadnetwork","3sh42y64q3.skadnetwork",
        "424m5254lk.skadnetwork","4468km3ulz.skadnetwork","44jx6755aq.skadnetwork","44n7hlldy6.skadnetwork",
        "47vhws6wlr.skadnetwork","488r3q3dtq.skadnetwork","4dzt52r2t5.skadnetwork","4fzdc2evr5.skadnetwork",
        "4mn522wn87.skadnetwork","4pfyvq9l8r.skadnetwork","4w7y6s5ca2.skadnetwork","523jb4fst2.skadnetwork",
        "52fl2v3hgk.skadnetwork","54nzkqm89y.skadnetwork","578prtvx9j.skadnetwork","5a6flpkh64.skadnetwork",
        "5ghnmfs3dh.skadnetwork","5l3tpt7t6e.skadnetwork","5lm9lj6jb7.skadnetwork","5mv394q32t.skadnetwork",
        "5tjdwbrq8w.skadnetwork","627r9wr2y5.skadnetwork","633vhxswh4.skadnetwork","67369282zy.skadnetwork",
        "6964rsfnh4.skadnetwork","6g9af3uyq4.skadnetwork","6p4ks3rnbw.skadnetwork","6qx585k4p6.skadnetwork",
        "6v7lgmsu45.skadnetwork","6xzpu9s2p8.skadnetwork","6yxyv74ff7.skadnetwork","737z793b9f.skadnetwork",
        "74b6s63p6l.skadnetwork","7953jerfzd.skadnetwork","79pbpufp6p.skadnetwork","79w64w269u.skadnetwork",
        "7fbxrn65az.skadnetwork","7fmhfwg9en.skadnetwork","7k3cvf297u.skadnetwork","7rz58n8ntl.skadnetwork",
        "7tnzynbdc7.skadnetwork","7ug5zh24hu.skadnetwork","84993kbrcf.skadnetwork","866k9ut3g3.skadnetwork",
        "88k8774x49.skadnetwork","899vrgt9g8.skadnetwork","89z7zv988g.skadnetwork","8c4e2ghe7u.skadnetwork",
        "8m87ys6875.skadnetwork","8qiegk9qfv.skadnetwork","8r8llnkz5a.skadnetwork","8s468mfl3y.skadnetwork",
        "8w3np9l82g.skadnetwork","97r2b46745.skadnetwork","9b89h5y424.skadnetwork","9g2aggbj52.skadnetwork",
        "9nlqeag3gk.skadnetwork","9rd848q2bz.skadnetwork","9t245vhmpl.skadnetwork","9vvzujtq5s.skadnetwork",
        "9wsyqb3ku7.skadnetwork","9yg77x724h.skadnetwork","a2p9lx4jpn.skadnetwork","a7xqa6mtl2.skadnetwork",
        "a8cz6cu7e5.skadnetwork","au67k4efj4.skadnetwork","av6w8kgt66.skadnetwork","axh5283zss.skadnetwork",
        "b55w3d8y8z.skadnetwork","b9bk5wbcq9.skadnetwork","bvpn9ufa9b.skadnetwork","bxvub5ada5.skadnetwork",
        "c3frkrj4fj.skadnetwork","c6k4g5qg8m.skadnetwork","c7g47wypnu.skadnetwork","cad8qz2s3j.skadnetwork",
        "cg4yq2srnc.skadnetwork","cj5566h2ga.skadnetwork","cp8zw746q7.skadnetwork","cs644xg564.skadnetwork",
        "cstr6suwn9.skadnetwork","d7g9azk84q.skadnetwork","dbu4b84rxf.skadnetwork","dd3a75yxkv.skadnetwork",
        "dkc879ngq3.skadnetwork","dmv22haz9p.skadnetwork","dn942472g5.skadnetwork","dr774724x4.skadnetwork",
        "dticjx1a9i.skadnetwork","dzg6xy7pwj.skadnetwork","e5fvkxwrpn.skadnetwork","ecpz2srf59.skadnetwork",
        "eh6m2bh4zr.skadnetwork","ejvt5qm6ak.skadnetwork","eqhxz8m8av.skadnetwork","f38h382jlk.skadnetwork",
        "f73kdq92p3.skadnetwork","f7s53z58qe.skadnetwork","feyaarzu9v.skadnetwork","fkak3gfpt6.skadnetwork",
        "fz2k2k5tej.skadnetwork","g28c52eehv.skadnetwork","g2y4y55b64.skadnetwork","g69uk9uh2b.skadnetwork",
        "g6gcrrvk4p.skadnetwork","gfat3222tu.skadnetwork","ggvn48r87g.skadnetwork","glqzh8vgby.skadnetwork",
        "gta8lk7p23.skadnetwork","gta9lk7p23.skadnetwork","gvmwg8q7h5.skadnetwork","h5jmj969g5.skadnetwork",
        "h65wbv5k3f.skadnetwork","h8vml93bkz.skadnetwork","hb56zgv37p.skadnetwork","hdw39hrw9y.skadnetwork",
        "hjevpa356n.skadnetwork","hs6bdukanm.skadnetwork","jb7bn6koa5.skadnetwork","k674qkevps.skadnetwork",
        "k6y4y55b64.skadnetwork","kbd757ywx3.skadnetwork","kbmxgpxpgc.skadnetwork","klf5c3l5u5.skadnetwork",
        "krvm3zuq6h.skadnetwork","l6nv3x923s.skadnetwork","l93v5h6a4m.skadnetwork","ln5gz23vtd.skadnetwork",
        "lr83yxwka7.skadnetwork","ludvb6z3bs.skadnetwork","m297p6643m.skadnetwork","m5mvw97r93.skadnetwork",
        "m8dbw4sv7c.skadnetwork","mj797d8u6f.skadnetwork","mlmmfzh3r3.skadnetwork","mls7yz5dvl.skadnetwork",
        "mp6xlyr22a.skadnetwork","mqn7fxpca7.skadnetwork","mtkv5xtk9e.skadnetwork","n38lu8286q.skadnetwork",
        "n66cz3y3bx.skadnetwork","n6fk4nfna4.skadnetwork","n9x2a789qt.skadnetwork","nfqy3847ph.skadnetwork",
        "nrt9jy4kw9.skadnetwork","nu4557a4je.skadnetwork","nzq8sh4pbs.skadnetwork","p78axxw29g.skadnetwork",
        "pd25vrrwzn.skadnetwork","ppxm28t8ap.skadnetwork","prcb7njmu6.skadnetwork","pu4na253f3.skadnetwork",
        "pwa73g5rt2.skadnetwork","pwdxu55a5a.skadnetwork","qlbq5gtkt8.skadnetwork","qqp299437r.skadnetwork",
        "qu637u8glc.skadnetwork","r26jy69rpl.skadnetwork","r45fhb6rf7.skadnetwork","rvh3l7un93.skadnetwork",
        "rx5hdcabgc.skadnetwork","s39g8k73mm.skadnetwork","s69wq72ugq.skadnetwork","sczv5946wb.skadnetwork",
        "su67r6k2v3.skadnetwork","t38b2kh725.skadnetwork","t3b3f7n3x8.skadnetwork","t6d3zquu66.skadnetwork",
        "t7ky8fmwkd.skadnetwork","tl55sbb4fm.skadnetwork","tmhh9296z4.skadnetwork","u679fj5vs4.skadnetwork",
        "uw77j35x4d.skadnetwork","uzqba5354d.skadnetwork","v4nxqhlyqp.skadnetwork","v72qych5uu.skadnetwork",
        "v7896pgt74.skadnetwork","v79kvwwj4g.skadnetwork","v9wttpbfk9.skadnetwork","vc83br9sjg.skadnetwork",
        "vcra2ehyfk.skadnetwork","vutu7akeur.skadnetwork","w28pnjg2k4.skadnetwork","w9q455wk68.skadnetwork",
        "wg4vff78zm.skadnetwork","wzmmz9fp6w.skadnetwork","x2jnk7ly8j.skadnetwork","x44k69ngh6.skadnetwork",
        "x5854y7y24.skadnetwork","x5l83yy675.skadnetwork","x8jxxk4ff5.skadnetwork","x8uqf25wch.skadnetwork",
        "xmn954pzmp.skadnetwork","xx9sdjej2w.skadnetwork","xy9t38ct57.skadnetwork","y45688jllp.skadnetwork",
        "y5ghdn5j9k.skadnetwork","y755zyxw56.skadnetwork","yclnxrl5pm.skadnetwork","ydx93a7ass.skadnetwork",
        "yrqqpx2mcb.skadnetwork","z24wtl6j62.skadnetwork","z4gj7hsk7h.skadnetwork","z5b3gh5ugf.skadnetwork",
        "z959bm4gru.skadnetwork","zh3b7bxvad.skadnetwork","zmvfpc5aq8.skadnetwork","zq492l623r.skadnetwork" };
        
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
        str = str + "   [AppsFlyerProxy instanceInitNoATTWithKey:@\"SFmpoiTG8y4MzCSPePPpsP\" appId: @\"\" gameName: @\"\" delegate:self];\n";
        str = str + "   [AppsFlyerProxy refrenceWindow:self.window];\n";
        str = str + "   [AppsFlyerProxy firstCheckSystemIdfaSet];\n\n";
        str = str + "   //获取国家，通知unity\n";
        str = str + "   NSString *countryStr = [AppsFlyerProxy getPhoneCountryCodeStr];\n";
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