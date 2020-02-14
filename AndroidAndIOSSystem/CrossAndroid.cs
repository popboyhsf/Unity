using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CrossAndroid : MonoBehaviour
{
    public enum AdType
    {
        UNKNOW,
        BANNER,
        INTERSTITIAL,
        REWARDED_VIDEO
    }

    private const string unityActivityName = "unity.SdkActivity";
    private static bool inited = false;
    private static AndroidJavaObject activity = null;
    private static bool causeAdLeftApplication;

    private void Awake()
    {
        gameObject.name = "CrossAndroidObject";
        //DontDestroyOnLoad(gameObject);
        CheckInited();
        GetVersionInfo();
#if !UNITY_ANDROID
        gameObject.SetActive(false);
#endif
    }

    private void Start()
    {
        
    }

    private static UnityAction WatchVideoCompletedAction;

    private static void Init()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {

            AndroidJavaClass jc = new AndroidJavaClass(unityActivityName);
            if (jc != null)
            {
                activity = jc.GetStatic<AndroidJavaObject>("activity");
                if (activity != null)
                {
                    inited = true;
                    if (!FindObjectOfType<CrossAndroid>())
                    {
                        GameObject go = new GameObject();
                        go.AddComponent<CrossAndroid>();
                    }
                }
            }
        }
        catch(Exception e)
        {
            Debuger.Log("CrossAndroid:Init error! unityActivityName was wrong");
            //Debuger.LogException(e);
        }
#endif
    }

    private static bool CheckInited()
    {
        if (!inited)
        {
            Init();
        }
        Debug.Log("CanInvoke is :" + (activity != null));
        return activity != null;
    }

    /// <summary>
    /// 获取版本名跟版本号
    /// </summary>
    public static void GetVersionInfo()
    {
        if (!CheckInited())
        {
            Debuger.Log("CheckInited ==== false");
            return;
        }
        string version = activity.Call<string>("getVersionInfo");
        string[] versions = version.Split('_');
        string versionName = versions[0];
        string versionCode = versions[1];

        //GameData.VersionCode = versionCode;
        //GameData.VersionName = versionName;
        Debuger.Log("CheckInited ==== true");
    }

    /// <summary>
    /// 显示Banner
    /// </summary>
    public static bool ShowBanner(bool must = true)
    {
        Debuger.Log("ShowBanner");
        if (!CheckInited())
        {
            return false;
        }
        return activity.Call<bool>("showBanner", must);
    }

    /// <summary>
    /// 隐藏Banner
    /// </summary>
    public static void HideBanner()
    {
        if (!CheckInited())
        {
            return;
        }
        activity.Call("hideBanner");
    }

    /// <summary>
    /// 显示插屏广告
    /// </summary>
    /// <param name="pos">0 表示gamestart，1表示gameresume，2表示gameended</param>
    public static void ShowInterstitial()
    {
        if (!CheckInited())
        {
            return;
        }
        activity.Call("showInterstitial");
    }

    /// <summary>
    /// 播放视频广告
    /// </summary>
    public static void ShowRewardedVideo(UnityAction watchCompletedAction, int entry)
    {
        Debuger.Log("ShowRewardedVideo");
        if (!CheckInited())
        {
            return;
        }
        WatchVideoCompletedAction = watchCompletedAction;
        activity.Call("showRewardBasedVideo", entry);
    }

    public static void ReqHideLoadingRewardVideoWindow()
    {
        Debug.Log("ReqHideLoadingRewardVideoWindow");
        if (!CheckInited())
        {
            return;
        }
        activity.Call("hideLoadingRewardVideoWindow");
    }

    public static void LogEvent(String eventName, String jsonStr)
    {
        if (!CheckInited())
        {
            return;
        }
        activity.Call("LogEvent", eventName, jsonStr);
    }


    private static IIsViedoReady thisIsReadyI;

    #region 请求广告

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
        activity.Call("rewardVideoIsReady");
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
        activity.Call("rewardVideoCancel");
        Debug.Log("rewardVideoCancel ====== " + thisIsReadyI);
    }

    #endregion

    /// <summary>
    ///  开始震动,
    /// </summary>
    /// <param name="pattern">第一个为延迟震动时间,第二个为停止振动时间,以此类推</param>
    /// <param name="type">重复几次pattern,-1表示不重复</param>
    public static void StartVibrator(long[] pattern, int type)
    {
        Debuger.Log("CancelShowRewardVideo");
        if (!CheckInited())
        {
            return;
        }
        activity.Call("startVibrator", pattern, type);
    }

    /// <summary>
    /// 观看视频结束,,unity需在此发放奖励
    /// </summary>
    /// <param name="returnCode"></param>
    public void WatchRewardVideoComplete(string returnCode)
    {
        Debuger.Log("WatchRewardVideoComplete");
        WatchVideoCompletedAction?.Invoke();
    }

    public void ShowLoadingRewardVideoWindow(string returnCode)
    {
        //ADLoading.Instance.ShowLoading();
    }

    public void HideLoadingRewardVideoWindow(string returnCode)
    {
        //ADLoading.Instance.HiddenLoading();
    }

    /// <summary>
    /// af返回买量跟自然区分
    /// </summary>
    /// <param name="status"></param>
    public void OnAppsFlyerReturnStatus(string status)
    {
        AnalysisController.AfStatus = (AnalysisController.AFStatus)(int.Parse(status));
    }


}

