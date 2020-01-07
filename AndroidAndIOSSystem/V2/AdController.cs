using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class AdController
{
    /// <summary>
    /// 播放插屏广告
    /// </summary>
    /// <param name="pos">0启动游戏,1切回游戏,2获取到奖励</param>
    /// <param name="must"></param>
    public static void InterstitialShow()
    {
#if UNITY_EDITOR || NoAd || SAFETY
        return;
#elif UNITY_ANDROID && !UNITY_EDITOR
        CrossAndroid.ShowInterstitial();
#elif UNITY_IPHONE// && !UNITY_EDITOR
        int a = 1 + 0;
        CrossIos.ShowInterstitial(a,SoundController.Instance.MuteMusic, SoundController.Instance.UnMuteMusic);
#endif
    }

    /// <summary>
    /// 播放视频广告
    /// </summary>
    /// <param name="watchCompletedAction"></param>
    /// <returns>是否播放成功</returns>
    public static void RewardedVideoShow(UnityAction watchCompletedAction, int entry)
    {
        int a = 0;
        entry += a;
#if UNITY_EDITOR || SafeMode || NoAd
        watchCompletedAction?.Invoke();
        return;
#elif UNITY_ANDROID && !UNITY_EDITOR
        CrossAndroid.ShowRewardedVideo(watchCompletedAction,entry);
#elif UNITY_IPHONE && !UNITY_EDITOR
        CrossIos.ShowRewardedVideo(entry, watchCompletedAction, SoundController.Instance.MuteMusic, SoundController.Instance.UnMuteMusic);
#endif
    }

    /// <summary>
    /// 取消等待loading视频广告
    /// </summary>
    public static void ShowRewardedVideoCancel()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        CrossAndroid.ReqHideLoadingRewardVideoWindow();
#elif UNITY_IPHONE && !UNITY_EDITOR
        CrossIos.ReqHideLoadingRewardVideoWindow();
#endif
    }


    public static void VideoIsReady(IIsViedoReady isViedoReady)
    {

        Debug.Log("IIsViedoReady From ==== " + isViedoReady);

#if UNITY_EDITOR || SafeMode || NoAd
        isViedoReady?.isReady(true);
#elif UNITY_ANDROID
        CrossAndroid.VideoIsReady(isViedoReady);
#elif UNITY_IPHONE
        CrossIos.VideoIsReady(isViedoReady);
#endif
    }

    public static void CancelRewardVideo()
    {

#if UNITY_EDITOR || SafeMode || NoAd

#elif UNITY_ANDROID
        CrossAndroid.RewardVideoCancel();
#elif UNITY_IPHONE
        CrossIos.RewardVideoCancel();
#endif
    }

}
