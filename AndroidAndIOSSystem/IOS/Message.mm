

#import <Foundation/Foundation.h>

#include "UnityInterface.h"
#import <os-usrsrc/AppsFlyerProxy.h>
#import <os-usrsrc/AdManager.h>
#import <os-usrsrc/AdId.h>
#import <os-usrsrc/CLog.h>

#ifdef __cplusplus
extern "C" {
#endif
    
    /**
     OC --->>>>  C#
     */
    //    void WatchRewardVideoComplete(){
    //        NSLog(@"UnitySendMessage Begin");
    //        UnitySendMessage("GameMaster(Clone)", "WatchRewardVideoComplete", "");
    //        NSLog(@"UnitySendMessage End");
    //    }
    
    
    void showInterstitial(int p){
        NSString * nsStr = [NSString stringWithFormat:@"showInterstitial %d",p];
        //        NSLog(nsStr);
        NSLog(@"NSLog(nsStr);  showInterstitial %s",[nsStr UTF8String]);
        
        if (p < 3 ) {
            InterstitialEnum pos = InterstitialEnum(p);
            if(pos == InterstitialEnumNextLevel)
                [AdManager.getInstance showInterstitialWithType:pos interval:60];
            else
                [AdManager.getInstance showInterstitialWithType:pos];
        }
    }

    void showRewardBasedVideoParam(int entry){
        NSString * nsStr = [NSString stringWithFormat:@"showRewardBasedVideoParam %d",entry];

        NSLog(@"NSLog(nsStr);  showRewardBasedVideoParam %s",[nsStr UTF8String]);
        
        id i = [NSNumber numberWithInt:(int)entry];
        
        [AdManager.getInstance showRewardVideoParam:i];
    }
    
    void showRewardBasedVideo(){
        NSLog(@" showRewardBasedVideo ");
        [AdManager.getInstance showRewardVideo];
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
        [AdManager.getInstance userCancelRewardVideo];
        UnitySendMessage("CrossIosObject", "HideLoadingRewardVideoWindow",  "");
    }
    
#ifdef __cplusplus
}
#endif
