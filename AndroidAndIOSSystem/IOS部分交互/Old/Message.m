//
//  Message.m
//  Unity-iPhone
//
//  Created by wanxiong mac on 2021/3/16.
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
        
        int status = 0;//模拟买量
        if(!isOrganic) status = 1;
        
        //if(isAppsFlyerReturn)
        //UnitySendMessage("CrossIosObject", "AppsFlyerState",  [NSString stringWithFormat:@"%d",status].UTF8String );
        //else CLog(@"appsFlayerIsOrganicIsSafeChannel is Fail . Wait for AF");
        //[LuckDrawManager getEventEntryRes];
            
            NSString* resStr = [AppsFlyerProxy getPhoneCountryCodeStr];
            CLog(@"LanguageResStr === %@",resStr);
            UnitySendMessage("CrossIosObject","ReturnContry", resStr.UTF8String);
        }];
    }

    void GetUnityPostInt(int type){
        
        CLog(@"点击回报_%i",type);
        //[LuckDrawManager showLuckDraw:type];
        UnityPause(true);
        [AdManager hideBannerAd];
    }


    void CashOutI(float i,const char* s){
        NSString* ss = [NSString stringWithUTF8String:s];
        NSLog(@" CashOutI_%@",ss);
        UnityPause(true);
        [AdManager hideBannerAd];
    }

    void GetUrlForIcon(){
        //SDK 控制
    }
    
    void GetTimerFromUnity(){
        NSLog(@"GetTimerFromUnity_AS");
        [AdManager getServerTime];
        
    }

    void LogEvetnForTrackLuckBalance(int j,float i){
    
        [AppsFlyerProxy trackLuckBalance:(float)j curBalance:(float)i];
    
    }

    void RateUs(bool isScoreEnogh){
        if(isScoreEnogh){
            [AppsFlyerProxy appScoring];
        }
    }


    void PushMessage(){
    
        NSString* iddAM = [NSString stringWithUTF8String:"1"];
        NSString* iddPM = [NSString stringWithUTF8String:"2"];
        NSString* mTitled = [NSString stringWithUTF8String:"T"];
        NSString* mSubTitled = [NSString stringWithUTF8String:"T"];
        NSString* mess1d = [NSString stringWithUTF8String:"🎻 Your energy is full! Start the event now! 🎻"];
        NSString* mess2d = [NSString stringWithUTF8String:"🥁 Someone asks for a challenge! Can you win? 🥁"];
        NSString* mess3d = [NSString stringWithUTF8String:"🎉 So lucky! A special gift is waiting for you! 🎉"];
        NSString* mess4d = [NSString stringWithUTF8String:"🧐 Where this sound comes from? Can you find it out? 🧐"];
        NSString* mess5d = [NSString stringWithUTF8String:"😱 Someone breaks your record! Win it back! 😱"];

        NSLog(@" setPushNormal: 1");
        NSArray *message=@[mess1d,mess2d,mess3d,mess4d,mess5d];
        NSLog(@" setPushNormal: 2");
        //[OSUtil removeOneNotificationWithID:idd];
        NSLog(@" setPushNormal: 3");
        
        NSLog(@" setPushNormal: 4");
    
    }
    
    //展示idfaview
    void showIDFA() {
            [AppsFlyerProxy firstRequestATTDialog];
            [AppsFlyerProxy logEvent:[AppsFlyerProxy getEventName:_tracking_show]];
    }

    //获取是否展示过授权请求，1展示过，0没展示
    int canShowIDFA() {
        BOOL result = [AppsFlyerProxy attDialogCanShow];
        if (!result) {
            [AppsFlyerProxy logEvent:[AppsFlyerProxy getEventName:_screen_tracking]];
        }
        return result?1:0;
    }

    //unity调用展示idfa之前调用，每个用户只调用一次
    void logEvetnForIDFA() {
        BOOL result = [AppsFlyerProxy attDialogStatus];
        if (result) {
            [AppsFlyerProxy logEvent:[AppsFlyerProxy getEventName:_tracking_close]];
            }
    }
    
    void rateUS(int count,int max,const char* patch)
    {
        NSString* patchNS = [NSString stringWithUTF8String:patch];
        
        NSString *starStr = [NSString stringWithFormat:@"%@%d", _rate_score_, count];
        [AppsFlyerProxy logEvent:[AppsFlyerProxy getEventName:starStr]];
        
        if(count == max)
        {
            [AppsFlyerProxy logEvent:[AppsFlyerProxy getEventName:_rate_show]];
            [AppsFlyerProxy appScoring];
            
        }
           
        
    }

    
#ifdef __cplusplus
}
#endif
