using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if !ADV2
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

    public static bool isDebug = false;

    /// <summary>
    /// 播放插屏广告
    /// </summary>
    /// <param name="pos">0启动游戏,1切回游戏,2获取到奖励</param>
    /// <param name="must"></param>
    public static void ShowInterstitial(int pos = 2)
    {
        //if (Srot.limit > 0) return;

        Debug.Log("ShowInterstitial");

        if (isDebug) return;

#if UNITY_EDITOR || NoAd || SafeMode
        return;
#elif UNITY_ANDROID && !UNITY_EDITOR
        CrossAndroid.ShowInterstitial();
#elif UNITY_IPHONE// && !UNITY_EDITOR
        CrossIos.ShowInterstitial(pos,null, null);
#endif

    }


    /// <summary>
    /// 播放插屏广告
    /// </summary>
    /// <param name="pos">0启动游戏,1切回游戏,2获取到奖励</param>
    /// <param name="must"></param>
    public static void ShowInterstitial(UnityAction watchCompletedAction, int pos = 2, UnityAction watchFailAction = null)
    {
        //if (Srot.limit > 0) return;

        Debug.Log("ShowInterstitial");

        if (isDebug) return;

#if UNITY_EDITOR || NoAd || SafeMode
        return;
#elif UNITY_ANDROID && !UNITY_EDITOR
        CrossAndroid.ShowInterstitial();
#elif UNITY_IPHONE// && !UNITY_EDITOR
        CrossIos.ShowInterstitial(pos,null, null);
#endif
        //TODO 还没写IOS方面的交互 记得写
    }

    /// <summary>
    /// 播放视频广告
    /// </summary>
    /// <param name="watchCompletedAction"></param>
    /// <returns>是否播放成功</returns>
    public static void ShowRewardedVideo(UnityAction watchCompletedAction, int entry, UnityAction watchFailAction = null, UnityAction watchEnterAction = null)
    {
        watchCompletedActionSelf = watchCompletedAction;
        watchFailActionSelf = watchFailAction;
        watchEnterActionSelf = watchEnterAction;
        entrySelf = entry;

        if (entrySelf == (int)VideoEventName.GiftCard_ClickBox)
        {
            ADLoading.Instance.Open(true);
        }
        else
        {
            ADLoading.Instance.Open(false);
        }


    }

    private static UnityAction watchCompletedActionSelf;
    private static UnityAction watchFailActionSelf;
    private static UnityAction watchEnterActionSelf;
    private static int entrySelf;

    public static void ShowRewardedVideoCallBack()
    {

        Srot.StartLimit();

        Debug.Log("ShowRewardedVideo");

        if (isDebug)
        {
            watchCompletedActionSelf?.Invoke();
            watchFailActionSelf?.Invoke();
            watchCompletedActionSelf = null;
            watchFailActionSelf = null;
            watchEnterActionSelf = null;
            entrySelf = 0;
            return;
        }

#if UNITY_EDITOR || SafeMode || NoAd
        watchCompletedActionSelf?.Invoke();
        watchFailActionSelf?.Invoke();
#elif UNITY_ANDROID && !UNITY_EDITOR
        CrossAndroid.ShowRewardedVideo(watchCompletedActionSelf,entrySelf);
#elif UNITY_IPHONE && !UNITY_EDITOR
        CrossIos.ShowRewardedVideo(entrySelf, watchCompletedActionSelf, Call, null,watchFailActionSelf);
#endif
        watchCompletedActionSelf = null;
        watchFailActionSelf = null;
        entrySelf = 0;
    }

    public static void Call()
    {
        watchEnterActionSelf?.Invoke();
        watchEnterActionSelf = null;
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

    public static void VideoIsReady(IIsViedoReady isViedoReady, bool isCash)
    {

        Debug.Log("IIsViedoReady From ==== " + isViedoReady);

        if (isDebug) isViedoReady?.isReady(true);

#if UNITY_EDITOR || SafeMode || NoAd
        isViedoReady?.isReady(true);
#elif UNITY_ANDROID
        CrossAndroid.VideoIsReady(isViedoReady);
#elif UNITY_IPHONE
        CrossIos.VideoIsReady(isViedoReady,isCash);
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


    public static void ShowGameStartInterstitial(bool isShow)
    {
        if (isDebug) return;
#if UNITY_EDITOR || SafeMode || NoAd

#elif UNITY_ANDROID
        
#elif UNITY_IPHONE
        CrossIos.Instance.ShowGameStartInterstitial(isShow);
#endif
    }
}
#else
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

    public static bool isDebug = false;

    private static UnityAction watchCompletedActionSelf;
    private static UnityAction watchFailActionSelf;
    private static UnityAction watchEnterActionSelf;

    private static UnityAction watchICompletedActionSelf;
    private static UnityAction watchIFailActionSelf;
    /// <summary>
    /// 播放插屏广告
    /// </summary>
    /// <param name="pos">0启动游戏,1切回游戏,2获取到奖励</param>
    public static void ShowInterstitial(int pos = 2)
    {
        //if (Srot.limit > 0) return;

        if (isDebug) return;

#if UNITY_EDITOR || NoAd || SafeMode
        return;
#elif UNITY_ANDROID && !UNITY_EDITOR
        CrossAndroid.ShowInterstitial(pos);
#elif UNITY_IPHONE// && !UNITY_EDITOR
        CrossIos.ShowInterstitial(pos,null, null);
#endif

    }

    /// <summary>
    /// 播放插屏广告带有回调
    /// </summary>
    /// <param name="pos">0启动游戏,1切回游戏,2获取到奖励</param>
    public static void ShowInterstitial(UnityAction watchCompletedAction, int pos = 2, UnityAction watchFailAction = null)
    {
        //if (Srot.limit > 0) return;

        watchICompletedActionSelf = watchCompletedAction;
        watchIFailActionSelf = watchFailAction;

        if (isDebug)
        {
            watchICompletedActionSelf?.Invoke();
            watchICompletedActionSelf = null;
            watchIFailActionSelf = null;
            return;
        }

#if UNITY_EDITOR || NoAd || SafeMode
        ShowInterstitialCallBack();
        return;
#elif UNITY_ANDROID && !UNITY_EDITOR
        CrossAndroid.ShowInterstitial(pos);
#elif UNITY_IPHONE// && !UNITY_EDITOR
        CrossIos.ShowInterstitial(pos,null, null);
#endif

    }

    public static void ShowInterstitialCallBack()
    {

        Debug.Log("ShowInterstitialCallBack");
        watchICompletedActionSelf?.Invoke();
        watchICompletedActionSelf = null;
        watchIFailActionSelf = null;
    }

    public static void ShowInterstitialFail()
    {

        Debug.Log("ShowInterstitialFail");
        watchIFailActionSelf?.Invoke();
        watchICompletedActionSelf = null;
        watchIFailActionSelf = null;
    }

    /// <summary>
    /// 播放视频广告
    /// </summary>
    /// <param name="watchCompletedAction"></param>
    /// <returns>是否播放成功</returns>
    public static void ShowRewardedVideo(UnityAction watchCompletedAction, int entry, UnityAction watchFailAction = null, UnityAction watchEnterAction = null)
    {

        watchCompletedActionSelf = watchCompletedAction;
        watchFailActionSelf = watchFailAction;
        watchEnterActionSelf = watchEnterAction;

        if (isDebug)
        {
            watchCompletedActionSelf?.Invoke();
            watchCompletedActionSelf = null;
            watchFailActionSelf = null;
            watchEnterActionSelf = null;
            return;
        }

#if UNITY_EDITOR || NoAd || SafeMode

        ShowRewardedVideoCallBack();
        return;
#elif UNITY_ANDROID && !UNITY_EDITOR
                        CrossAndroid.ShowRewardedVideo(entry);
#elif UNITY_IPHONE// && !UNITY_EDITOR
                        CrossIos.ShowRewardedVideo(entry,null);
#endif
        watchEnterActionSelf = null;
    }

    public static void ShowRewardedVideoCallBack()
    {

        Srot.StartLimit();

        Debug.Log("ShowRewardedVideo");



        watchCompletedActionSelf?.Invoke();

        watchCompletedActionSelf = null;
        watchFailActionSelf = null;
    }

    public static void ShowRewardedVideoFail()
    {
        Debug.Log("ShowRewardedVideoFail");
        watchFailActionSelf?.Invoke();

        watchCompletedActionSelf = null;
        watchFailActionSelf = null;
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

    public static void ShowGameStartInterstitial(bool isShow)
    {
        if (isDebug) return;
#if UNITY_EDITOR || SafeMode || NoAd

#elif UNITY_ANDROID
        
#elif UNITY_IPHONE
        CrossIos.Instance.ShowGameStartInterstitial(isShow);
#endif
    }

}
#endif
