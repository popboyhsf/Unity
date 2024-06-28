//
//  Message.m
//  Unity-iPhone
//
//  Created by wanxiong mac on 2024/01/10.
//

#import <Foundation/Foundation.h>
#import <OSAttributionStatistics/OSAttributionStatistics.h>
//#import <LuckDrawSDK/LuckDrawSDK.h>
#import <AdMediationMax/AdMediationMax.h>
#import <AudioToolbox/AudioToolbox.h>

#ifdef __cplusplus
extern "C" {
#endif
    
    /**
     OC --->>>>  C#
     */

    
    
    void showInterstitial(int p){
        NSString * nsStr = [NSString stringWithFormat:@"showInterstitial %d",p];
        //        NSLog(nsStr);
        NSLog(@"NSLog(nsStr);  showInterstitial %s",[nsStr UTF8String]);
        
        if (p < 3 ) {
            InterstitialEnum pos = InterstitialEnumNextLevel;
            if(pos == InterstitialEnumNextLevel)
                [AdManager showMaxInterstitialAd:pos interval:60];
            else
                [AdManager showMaxInterstitialAd:pos];
        }
        
    }
    
    void showRewardBasedVideo(){
        NSLog(@" showRewardBasedVideo ");
        [AdManager showMaxRewardedAd:-1];
    }

    void showRewardBasedVideoParam(int entry){
        NSString * nsStr = [NSString stringWithFormat:@"showRewardBasedVideoParam %d",entry];

        NSLog(@"NSLog(nsStr);  showRewardBasedVideoParam %s",[nsStr UTF8String]);
        
        NSInteger i = entry;
        
        [AdManager showMaxRewardedAd:i];
    }


    void isRewardVideoReady(BOOL isCash){
        //[AdManager setAdmobSafeMode:isCash];
        [AdManager isRewardVideoReady:^(BOOL isReady) {
                NSString* str ;
            if (isReady) {
                str = [NSString stringWithUTF8String:"true"];
            }else{
                str = [NSString stringWithUTF8String:"false"];
            }
            UnitySendMessage("CrossIosObject", "RewardVideoIsReady",  str.UTF8String);
                } rewardVideoReady:^{
                        //[self hideLoading];
                    UnitySendMessage("CrossIosObject", "RewardVideoIsReadyCall",  "");
        }];
    }


    void rewardVideoCancel(){
        [AdManager userCancelRewardVideo];
    }
    
    void LogEventIOS(const char* eventName,const char* content){
        //        NSLog(eventName + content);
        NSString* eventNs = [NSString stringWithUTF8String:eventName];
        NSString* contentNs = [NSString stringWithUTF8String:content];
        NSLog(@" eventName: %@",eventNs);
        NSLog(@" content: %@",contentNs);
        [AppsFlyerProxy logEvent:eventNs json:contentNs];
    }
    
    void hideLoadingRewardVideoWindow(){
        NSLog(@" hideLoadingRewardVideoWindow ");
        [AdManager userCancelRewardVideo];
        UnitySendMessage("CrossIosObject", "HideLoadingRewardVideoWindow",  "");
    }



    void startVibrator(int type){
    
        AudioServicesPlaySystemSound(type);
        
    }


    void gameStart(BOOL isShow){
        
		if(true)
			[AdManager showMaxInterstitialAd:InterstitialEnumSplashEnd];
        
        [AdManager initBanner:AdBannerPosBottom];
        [AdManager showBannerAd:true];
    }

    void getAF(){
        [AppsFlyerProxy appsFlayerIsOrganicIsSafeChannel:^(BOOL isAppsFlyerReturn, BOOL isOrganic, BOOL isSafeChannel) {
		
			OSLog(@"isAppsFlyerReturn is %i, isOrganic is %i, isSafeChannel is %i", isAppsFlyerReturn, isOrganic, isSafeChannel);
                
			CLog(@"isSafeChannel is %i", isSafeChannel);
			CLog(@"isOrganic is %i", isOrganic);
       
            
        }];
    }


    void LogEvetnForTrackLuckBalance(int j,float i){
    
        [AppsFlyerProxy trackLuckBalance:(float)j curBalance:(float)i];
    
    }

    //展示idfaview
    void showIDFA() {
        [AppsFlyerProxy firstRequestATTDialog];
        [AppsFlyerProxy logEvent:[AppsFlyerProxy getEventName:_tracking_show]];
		//UnitySendMessage("CrossIosObject", "IDFACallBack",  "");
    }

    //获取是否展示过授权请求，1展示过，0没展示
    int canShowIDFA() {
        BOOL result = [AppsFlyerProxy attDialogCanShow];
        if (!result) {
            [AppsFlyerProxy logEvent:[AppsFlyerProxy getEventName:_screen_tracking]];
        }
        return result?1:0;
    }
    
    void requestIDFA(){
        [AppsFlyerProxy requestATTDialog];
    }

    //unity调用展示idfa之前调用，每个用户只调用一次
    void logEvetnForIDFA() {
        BOOL result = [AppsFlyerProxy attDialogStatus];
        if (result) {
            [AppsFlyerProxy logEvent:[AppsFlyerProxy getEventName:_tracking_close]];
            }
    }
	
	//协议web页面调用
    void iOSWebPageShow(char *str) {
        NSString *valueStr = [NSString stringWithCString:str encoding:NSUTF8StringEncoding];
        NSLog(@"%@", valueStr);
        [[NSNotificationCenter defaultCenter] postNotificationName:@"iOSWebPageShow" object:nil userInfo:@{@"url": valueStr}];
    }

    //震动
    void iOSDeviceShock(int value) {
        AudioServicesPlaySystemSound(value);
    }
	
	void rateUSShow()
    {
		[AppsFlyerProxy logEvent:[AppsFlyerProxy getEventName:_rate_show]];
	}
	
    void rateUS(int count,int max,const char* patch)
    {
        NSString* patchNS = [NSString stringWithUTF8String:patch]; 
        
        if(count == max)
        {
            NSString *starStr = [NSString stringWithFormat:@"%@%d", _rate_score_, count];
			[AppsFlyerProxy logEvent:[AppsFlyerProxy getEventName:starStr]];
			
            [AppsFlyerProxy appScoring];
            
        }
        
    }
	
	//GDPR按钮能否展示0不展示，1展示
    int iOSCanShowGDPR() {
        NSString *str = [AdManager canShowGDPR];
        if ([str isEqualToString:@"1"])
            return 1;
        return 0;
    }

    //展示GDPR
    void showPrivacyOptionsForm() {
        [AdManager showPrivacyOptionsForm];
    }
	
    
#ifdef __cplusplus
}
#endif
