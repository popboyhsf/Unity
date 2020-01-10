//
//  MultiHaptic.m
//  Unity-iPhone
//
//  Created by tangz on 2018/9/6.
//
#import <Foundation/Foundation.h>

#include "UnityInterface.h"
#import <AudioToolbox/AudioToolbox.h>

#ifdef __cplusplus
extern "C" {
#endif

    
    void _hapticMedium(){
        UIImpactFeedbackGenerator *gen = [[UIImpactFeedbackGenerator alloc]initWithStyle:UIImpactFeedbackStyleMedium];
        [gen prepare];
        [gen impactOccurred];
    }
    
    void _hapticLight(){
        UIImpactFeedbackGenerator *gen = [[UIImpactFeedbackGenerator alloc]initWithStyle:UIImpactFeedbackStyleLight];
        [gen prepare];
        [gen impactOccurred];
    }
    
    void _hapticHeavy(){
        UIImpactFeedbackGenerator *gen = [[UIImpactFeedbackGenerator alloc]initWithStyle:UIImpactFeedbackStyleHeavy];
        [gen prepare];
        [gen impactOccurred];
    }

	void _AudioServices(int type){
		AudioServicesPlaySystemSound(type);
	}


#ifdef __cplusplus
}
#endif
