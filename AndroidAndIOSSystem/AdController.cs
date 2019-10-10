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
    /// <param name="pos">0启动游戏,1获取到奖励,2切回游戏</param>
    public static void ShowInterstitial(int pos = 1)
    {
#if UNITY_EDITOR || NoAd || SafeMode
        return;
#elif UNITY_ANDROID && !UNITY_EDITOR
        CrossAndroid.ShowInterstitial();
#elif UNITY_IPHONE && !UNITY_EDITOR
        CrossIos.ShowInterstitial(1,SoundController.Instance.MuteMusic, SoundController.Instance.UnMuteMusic);
#endif

    }

    /// <summary>
    /// 播放视频广告
    /// </summary>
    /// <param name="watchCompletedAction"></param>
    /// <returns>是否播放成功</returns>
    public static void ShowRewardedVideo(UnityAction watchCompletedAction,int entry)
    {
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
    public static void CancelShowRewardedVideo()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        CrossAndroid.ReqHideLoadingRewardVideoWindow();
#elif UNITY_IPHONE && !UNITY_EDITOR
        CrossIos.ReqHideLoadingRewardVideoWindow();
#endif

    }

}
