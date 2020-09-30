using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


#if !ADV2
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
        DontDestroyOnLoad(gameObject);
        CheckInited();
        GetVersionInfo();
    }

    private void Start()
    {

    }

    private static UnityAction WatchVideoCompletedAction;

    private static void Init()
    {
#if UNITY_ANDROID && !UNITY_EDITOR && !Marketing
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
#else
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
        Debug.Log("CrossAndroidCanInvoke is :" + (activity != null));
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

    public void GetClassToUnity(string c) 
    {
        Debug.Log("AF平台为 ===== " + c);
    }

#region 原始版本
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
    /// 播放插屏广告
    /// </summary>
    /// <param name="pos">0启动游戏,1切回游戏,2获取到奖励</param>
    /// <param name="must"></param>
    public static void ShowInterstitial(int pos)
    {
        if (!CheckInited())
        {
            return;
        }
        activity.Call("showInterstitialAd", pos);
    }

    /// <summary>
    /// 播放视频广告
    /// </summary>
    public static void ShowRewardedVideo(int entry)
    {
        Debuger.Log("ShowRewardedVideo");
        if (!CheckInited())
        {
            return;
        }
        activity.Call("showRewardedVideo", entry);
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

    /// <summary>
    /// 观看视频结束,,unity需在此发放奖励
    /// </summary>
    /// <param name="returnCode"></param>
    public void WatchRewardVideoComplete(string returnCode)
    {
        Debuger.Log("WatchRewardVideoComplete");
        AdController.ShowRewardedVideoCallBack();
    }

    public void WatchRewardVideoFail()
    {
        Debuger.Log("WatchRewardVideoFail");
        AdController.ShowRewardedVideoFail();
    }

    public void ShowLoadingRewardVideoWindow(string returnCode)
    {
        ADLoading.Instance.ShowLoading();
    }

    public void HideLoadingRewardVideoWindow(string returnCode)
    {
        ADLoading.Instance.HiddenLoading();
    }
#endregion

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

#region HW相关


    /// <summary>
    /// HW登陆函数
    /// </summary>
    public static void HWLogin()
    {
#if !HWMode
        return;
#endif
        Debuger.Log("HWLogin Log");
        if (!CheckInited())
        {
            return;
        }
        activity.Call("HWLogin");
    }

    public static string HWGetUserID()
    {
        if (!CheckInited())
        {
            Debuger.Log("CheckInited ==== false");
            return "";
        }
        string id = activity.Call<string>("GetName");

        Debuger.Log("GetName ==== " + id);

        return id;


    }

#endregion

#region 服务器抽奖

    /// <summary>
    /// 传递状态 int
    /// </summary>
    /// <param name="i"></param>
    public static void PostInt(int i)
    {
        Debuger.Log("PostUnityPostInt === " + i);
        if (!CheckInited())
        {
            return;
        }
        activity.Call("GetUnityPostInt", i);
    }

    /// <summary>
    /// 奖品回调
    /// </summary>
    /// <param name="name"></param>
    /// <param name="num"></param>
    public void OnLuckCallBack(string count)
    {
        string name = count.Split('_')[0];
        int num = int.Parse(count.Split('_')[1]);

        Debuger.Log("抽中的奖品 === " + name + "    数量 ==== " + num);
        if (name.Equals("Hint"))
        {
            TipItemData.AddTipOne();
        }
    }

    //发送请求url
    public static void PostUrlForIcon()
    {
        Debuger.Log("PostUrlForIcon === ");
        if (!CheckInited())
        {
            return;
        }
        activity.Call("GetUrlForIcon");
    }

    //接受url
    public void GetUrlForIconCallBack(string url)
    {
        PostAndGetIcon.Instance.GetIcon(url);
    }

    //CashOut
    public static void CashOut(float balance,string s)
    {
        balance = (float)Math.Round(balance, 2);

        Debuger.Log("CashOut === " + balance);
        Debuger.Log("CashOut === " + s);
        if (!CheckInited())
        {
            return;
        }
        activity.Call("CashOut", balance, s);
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
    /// af返回买量跟自然区分
    /// </summary>
    /// <param name="status"></param>
    public void OnAppsFlyerReturnStatus(string status)
    {
        AnalysisController.AfStatus = (AnalysisController.AFStatus)(int.Parse(status));
    }

    /// <summary>
    /// Unity主动访问Android寻求AF结果
    /// </summary>
    /// <returns></returns>
    public static AnalysisController.AFStatus GetAF()
    {
        if (!CheckInited())
        {
            Debuger.Log("CheckInited ==== false");
            return AnalysisController.AFStatus.Unknow;
        }

        var status = activity.Call<string>("getAF");

        AnalysisController.AFStatus afStatus = AnalysisController.AfStatus;

        if ((AnalysisController.AFStatus)(int.Parse(status)) == AnalysisController.AFStatus.NonOrganic)
            afStatus = (AnalysisController.AFStatus)(int.Parse(status));

        Debug.LogWarning("GetAF Function Start And Value == " + afStatus);

        return afStatus;
    }

    public static void LogEvent(String eventName, String jsonStr)
    {
        if (!CheckInited())
        {
            return;
        }
        activity.Call("LogEvent", eventName, jsonStr);
    }

    /// <summary>
    /// 传递礼品卡信息
    /// </summary>
    /// <param name="i">当前礼品卡总额</param>
    /// <param name="j">当前礼品卡梯度</param>
    public static void LogEvetnForTrackLuckBalance(float i, int j = 200)
    {
        if (!CheckInited())
        {
            return;
        }
        activity.Call("LogEvetnForTrackLuckBalance", j, i);
    }


}
#endif
