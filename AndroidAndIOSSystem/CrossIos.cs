using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_IPHONE && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif


public class CrossIos : MonoBehaviour
{

    private static CrossIos _instance;
    public static CrossIos Instance { get => _instance; }

    public enum AdType
    {
        UNKNOW,
        BANNER,
        INTERSTITIAL,
        REWARDED_VIDEO
    }

    /**
    * IOS 相关函数
    */
#if UNITY_IPHONE && !UNITY_EDITOR && !SafeMode
    /**
     * IOS   广告
     */

    [DllImport("__Internal")]
    public static extern void showInterstitial(int p);

    [DllImport("__Internal")]
    public static extern void showRewardBasedVideoParam(int p);
    
    [DllImport("__Internal")]
    public static extern void showRewardBasedVideo();
    
    [DllImport("__Internal")]
    public static extern void LogEventIOS(string eventName,string content);
    

    
    [DllImport("__Internal")]
    public static extern void hideLoadingRewardVideoWindow();

    [DllImport("__Internal")]
    public static extern void isRewardVideoReady();

    [DllImport("__Internal")]
    public static extern void rewardVideoCancel();

    [DllImport("__Internal")]
    public static extern void startVibrator(int type);

    [DllImport("__Internal")]
    public static extern void gameStart(bool type);
        

    
#endif


    private static bool inited = false;
    private static bool causeAdLeftApplication;

    public static UnityAction InterstitialOpenCallback;
    public static UnityAction InterstitialCloseCallback;

    private void Awake()
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        Application.targetFrameRate = 60;
        gameObject.name = "CrossIosObject";
        _instance = this;
#endif

        CheckInited();
        GetVersionInfo();
    }

    private void Start()
    {

    }

    public static UnityAction RewardVideoCompletedAction;
    public static UnityAction RewardVideoOpenCallback;
    public static UnityAction RewardVideoCloseCallback;

    private static void Init()
    {

    }

    private static bool CheckInited()
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        return true;
#endif
        return false;
    }

    /// <summary>
    /// 获取版本名跟版本号
    /// </summary>
    public static void GetVersionInfo()
    {

    }

    /// <summary>
    /// 显示Banner
    /// </summary>
    public static bool ShowBanner(bool must = true)
    {
        return false;
    }

    /// <summary>
    /// 隐藏Banner
    /// </summary>
    public static void HideBanner()
    {

    }

    /// <summary>
    /// 显示插屏广告
    /// </summary>
    /// <param name="pos">0 表示SplashEnd，1表示NextLevel，2表示Resume</param>
    public static void ShowInterstitial(int pos = 1, UnityAction openCallback = null, UnityAction closeCallback = null)
    {
        if (!CheckInited())
        {
            return;
        }
        InterstitialOpenCallback = openCallback;
        InterstitialCloseCallback = closeCallback;
        int p = pos;
#if UNITY_IPHONE && !UNITY_EDITOR && !SafeMode
        showInterstitial(p);
#endif
    }

    // 供Android和IOS平台调用
    public void InterstitialAdOpen()
    {
        Debug.Log(" InterstitialAdOpen ");
        if (InterstitialOpenCallback != null)
            InterstitialOpenCallback();

        InterstitialOpenCallback = null;
    }

    public void InterstitialAdClose()
    {
        Debug.Log(" InterstitialAdClose ");
        if (InterstitialCloseCallback != null)
            InterstitialCloseCallback();

        InterstitialCloseCallback = null;
    }

    /// <summary>
    /// 播放视频广告
    /// </summary>
    public static void ShowRewardedVideo(int entry, UnityAction watchCompletedAction, UnityAction watchStartAction = null, UnityAction watchClossAction = null)
    {
        Debuger.Log("ShowRewardedVideo");
        if (!CheckInited())
        {
            return;
        }
        RewardVideoCompletedAction = watchCompletedAction;
        RewardVideoOpenCallback = watchStartAction;
        RewardVideoCloseCallback = watchClossAction;
#if UNITY_IPHONE && !UNITY_EDITOR && !SafeMode
        showRewardBasedVideoParam(entry);
#endif
    }
    public static void ShowRewardedVideo(UnityAction watchCompletedAction, UnityAction watchStartAction = null, UnityAction watchClossAction = null)
    {
        Debuger.Log("ShowRewardedVideo");
        if (!CheckInited())
        {
            return;
        }
        RewardVideoCompletedAction = watchCompletedAction;
        RewardVideoOpenCallback = watchStartAction;
        RewardVideoCloseCallback = watchClossAction;
#if UNITY_IPHONE && !UNITY_EDITOR && !SafeMode
        showRewardBasedVideo();
#endif
    }

    /**
    移动平台的回调接口
    */
    public void RewardVideoOpen()
    {
        Debug.Log(" RewardVideoOpen ");
        if (RewardVideoOpenCallback != null)
            RewardVideoOpenCallback();
        RewardVideoOpenCallback = null;
    }

    // IOS版本 不会直接调用这个接口，调用的是WatchRewardVideoComplete ，然后判断返回的参数
    public void RewardVideoClose()
    {
        Debug.Log(" RewardVideoClose ");
        if (RewardVideoCloseCallback != null)
            RewardVideoCloseCallback();
        RewardVideoCloseCallback = null;
    }


    public static void ReqHideLoadingRewardVideoWindow()
    {
        Debug.Log("ReqHideLoadingRewardVideoWindow");
        if (!CheckInited())
        {
            return;
        }

#if UNITY_IPHONE && !UNITY_EDITOR && !SafeMode
        hideLoadingRewardVideoWindow();
#endif
    }

    public static void LogEvent(String eventName, String jsonStr)
    {
        if (!CheckInited())
        {
            return;
        }

#if UNITY_IPHONE && !UNITY_EDITOR && !SafeMode
        LogEventIOS(eventName, jsonStr);
#endif
    }

    /// <summary>
    ///  开始震动,
    /// </summary>
    /// <param name="pattern">第一个为延迟震动时间,第二个为停止振动时间,以此类推</param>
    /// <param name="type">重复几次pattern,-1表示不重复</param>
    public void StartVibrator(long[] pattern, int type)
    {
        if (!CheckInited())
        {
            return;
        }
        StartCoroutine(PlaySoungType(pattern[0], type));
    }

    static IEnumerator PlaySoungType(long time, int type)
    {
        var timer = time / 1000f;
        yield return new WaitForSecondsRealtime(timer);
#if UNITY_IPHONE && !UNITY_EDITOR && !SafeMode
            startVibrator(type);
            //Debug.Log("Vibrator === " + type);
#endif
        yield break;
    }

    /// <summary>
    /// 观看视频结束,,unity需在此发放奖励
    /// </summary>
    /// <param name="returnCode"></param>
    public void WatchRewardVideoComplete(string returnCode)
    {
        Debuger.Log("WatchRewardVideoComplete");
#if UNITY_IPHONE && !UNITY_EDITOR && !SafeMode
        RewardVideoClose();
        if (returnCode == "success")
            RewardVideoCompletedAction?.Invoke();
#endif

    }

    /// <summary>
    /// 展示启动插屏
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowGameStartInterstitial(bool isShow)
    {
#if UNITY_IPHONE && !UNITY_EDITOR && !SafeMode
            gameStart(isShow);
            
#endif
    }



    public void ShowLoadingRewardVideoWindow(string returnCode)
    {
        //ADLoading.Instance.ShowLoading();
    }

    public void HideLoadingRewardVideoWindow(string returnCode)
    {
        //ADLoading.Instance.HiddenLoading();
    }

    #region 请求广告

    private static IIsViedoReady thisIsReadyI;

    /// <summary>
    /// 请求广告是否加载完毕
    /// </summary>
    /// <param name="isViedoReady">接口类</param>
    public static void VideoIsReady(IIsViedoReady isViedoReady)
    {
        if (!CheckInited())
        {
            return;
        }
#if UNITY_IPHONE && !UNITY_EDITOR && !SafeMode
        isRewardVideoReady();
#endif
        thisIsReadyI = isViedoReady;

    }

    /// <summary>
    /// 接收是否有广告
    /// </summary>
    /// <param name="status"></param>
    public void RewardVideoIsReady(string status)
    {
        var isRead = bool.Parse(status);
        thisIsReadyI?.isReady(isRead);
    }

    /// <summary>
    /// 没有广告时等待加载完成后返回
    /// </summary>
    public void RewardVideoIsReadyCall()
    {
        thisIsReadyI?.isReady(true);
    }

    /// <summary>
    /// 强制取消回调
    /// </summary>
    public static void RewardVideoCancel()
    {
        if (!CheckInited())
        {
            return;
        }

#if UNITY_IPHONE && !UNITY_EDITOR && !SafeMode
        rewardVideoCancel();
#endif
        Debug.Log("rewardVideoCancel ====== " + thisIsReadyI);
    }

    #endregion

    /// <summary>
    /// af返回买量跟自然区分
    /// </summary>
    /// <param name="status"></param>
    public void AppsFlyerState(string status)
    {
        AnalysisController.AfStatus = (AnalysisController.AFStatus)(int.Parse(status));

        Debug.Log("AppsFlyerState === " + status);
    }
}
