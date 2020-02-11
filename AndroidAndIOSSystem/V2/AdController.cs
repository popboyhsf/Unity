using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IIsViedoReady
{
    void isReady(bool isReady);

}

public static class VideoRewardPos
{
    //获取现金
    public static int RewardCash = 1;
    //金币宝箱
    public static int RewardCoin = 2;
    //金钩
    public static int GoodHook = 3;
    //离线奖励
    public static int Offline = 4;
    //结算奖励
    public static int Clear = 5;
    //解锁新鱼
    public static int NewFish = 6;
    //能量
    public static int Power = 7;
}

public class AdController
{

    private static ADTimeLimit srot;

    public static ADTimeLimit Srot
    {
        get
        {
            if (srot == null) srot = new GameObject("ADControllerSrot").AddComponent<ADTimeLimit>();
            return srot;
        }
    }

    /// <summary>
    /// 播放插屏广告
    /// </summary>
    /// <param name="pos">0启动游戏,1切回游戏,2获取到奖励</param>
    /// <param name="must"></param>
    public static void ShowInterstitial()
    {
        if (Srot.limit > 0) return;

        Debug.Log("ShowInterstitial");

#if UNITY_EDITOR || NoAd || SAFETY
        return;
#elif UNITY_ANDROID && !UNITY_EDITOR
        CrossAndroid.ShowInterstitial();
#elif UNITY_IPHONE// && !UNITY_EDITOR
        CrossIos.ShowInterstitial(1,SoundController.Instance.MuteMusic, SoundController.Instance.UnMuteMusic);
#endif

    }

    /// <summary>
    /// 播放视频广告
    /// </summary>
    /// <param name="watchCompletedAction"></param>
    /// <returns>是否播放成功</returns>
    public static void ShowRewardedVideo(UnityAction watchCompletedAction, int entry)
    {
        Srot.StartLimit();

        Debug.Log("ShowRewardedVideo");


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
