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
        Application.targetFrameRate = 60;
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
        activity.Call("hideLoadingRewardVideoWindow"); // hideLoadingRewardVideoWindow 改名  hideWindow
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
        activity.Call("rewardVideoIsReady");  // rewardVideoIsReady 改名 isPlaying
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
        Application.targetFrameRate = 60;
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
    public static void ShowBanner()
    {
        Debuger.Log("ShowBanner");
        if (!CheckInited())
        {
            return;
        }
        activity.Call("showBanner");
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
    /// 观看插屏结束,,unity需在此发放奖励
    /// </summary>
    /// <param name="returnCode"></param>
    public void WatchInterstitialComplete(string returnCode)
    {
        Debuger.Log("WatchInterstitialComplete");
        AdController.ShowInterstitialCallBack();
    }

    public void WatchInterstitialFail()
    {
        Debuger.Log("WatchInterstitialFail");
        AdController.ShowInterstitialFail();
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
        activity.Call("hideLoadingRewardVideoWindow"); // hideLoadingRewardVideoWindow 改名  hideWindow
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
        activity.Call("rewardVideoIsReady"); // rewardVideoIsReady 改名 isPlaying
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
        if (name.ToLower().Equals("hammer"))
        {
            ItemSystemData.AddItem(ItemSystemData.ItemEnum.chuizi,num);
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
            AnalysisController.AfStatus = AnalysisController.AFStatus.Organic;
            AnalysisController.OffAFStatusChanged?.Invoke();
            return AnalysisController.AFStatus.Organic;
        }

        var status = activity.Call<string>("getAF");

        AnalysisController.AFStatus afStatus = AnalysisController.AFStatus.Organic;

        var _status = (AnalysisController.AFStatus)int.Parse(status);

        if (_status == AnalysisController.AFStatus.NonOrganic)
            afStatus = _status;

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



    /// <summary>
    /// 返回国家 --- 需要在Android - GetAF后执行
    /// </summary>
    /// <param name="returnC"></param>
    public void ReturnContry(string returnC)
    {
        if (I2Language.Instance == null) return;

        var _s = returnC.ToUpper();

        try
        {
            _s = _s.Split('_')[1];
        }
        catch (Exception e)
        {

            Debuger.LogError("返回國家Error");
        }

        if (_s.IndexOf("OTH") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.EN);
        }
        else if (_s.IndexOf("US") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.EN);
        }
        else if (_s.IndexOf("KR") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.KR);
        }
        else if (_s.IndexOf("JP") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.JP);
        }
        else if (_s.IndexOf("RU") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.RU);
        }
        else if (_s.IndexOf("PT") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.BR);
        }
        else if (_s.IndexOf("ID") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.ID);
        }
        else if (_s.IndexOf("PH") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.PH);
        }
        else if (_s.IndexOf("DE") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.DE);
        }
        else if (_s.IndexOf("FR") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.FR);
        }
        else if (_s.IndexOf("TH") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.TH);
        }
        else if (_s.IndexOf("VN") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.VN);
        }
        else if (_s.IndexOf("MX") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.MX);
        }
        else if (_s.IndexOf("TR") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.TR);
        }
        else if (_s.IndexOf("EG") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.EG);
        }
        else if (_s.IndexOf("IN") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.IN);
        }
        else if (_s.IndexOf("PK") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.PK);
        }
        else if (_s.IndexOf("BD") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.BD);
        }
        else if (_s.IndexOf("ZA") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.ZA);
        }
        else if (_s.IndexOf("NG") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.NG);
        }
        else if (_s.IndexOf("CO") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.CO);
        }
        else if (_s.IndexOf("AR") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.AR);
        }
        else if (_s.IndexOf("SA") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.SA);
        }
        else if (_s.IndexOf("AE") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.AE);
        }
        else if (_s.IndexOf("IQ") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.EN);
        }
        else if (_s.IndexOf("ES") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.ES);
        }
        else if (_s.IndexOf("IT") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.IT);
        }
        else if (_s.IndexOf("PL") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.PL);
        }
        else if (_s.IndexOf("NL") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.NL);
        }
        else if (_s.IndexOf("RO") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.RO);
        }
        else if (_s.IndexOf("SE") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.SE);
        }
        else if (_s.IndexOf("GR") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.GR);
        }
        //231114新增
        else if (_s.IndexOf("AT") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.AT);
        }
        else if (_s.IndexOf("CH") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.CH);
        }
        else if (_s.IndexOf("BE") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.BE);
        }
        else if (_s.IndexOf("NO") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.NO);
        }
        else if (_s.IndexOf("IE") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.IE);
        }
        else if (_s.IndexOf("DK") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.DK);
        }
        else if (_s.IndexOf("FI") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.FI);
        }
        else if (_s.IndexOf("BR") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.PT);
        }
        else if (_s.IndexOf("PE") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.PE);
        }
        else if (_s.IndexOf("EC") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.EC);
        }
        else if (_s.IndexOf("MY") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.MY);
        }
        else if (_s.IndexOf("CL") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.CL);
        }
        else if (_s.IndexOf("CZ") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.CZ);
        }
        else if (_s.IndexOf("HU") >= 0) 
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.HU);
        }
        else
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.EN);
        }
    }

    public void ReturnContryChangeUI(string returnC)
    {
        if (I2Language.Instance == null) return;

        var _s = returnC.ToUpper();

        try
        {
            _s = _s.Split('_')[1];
        }
        catch (Exception e)
        {

            Debuger.LogError("返回國家Error");
        }

        if (_s.IndexOf("OTH") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.EN);
        }
        else if (_s.IndexOf("US") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.EN);
        }
        else if (_s.IndexOf("KR") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.KR);
        }
        else if (_s.IndexOf("JP") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.JP);
        }
        else if (_s.IndexOf("RU") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.RU);
        }
        else if (_s.IndexOf("PT") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.BR);
        }
        else if (_s.IndexOf("ID") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.ID);
        }
        else if (_s.IndexOf("PH") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.PH);
        }
        else if (_s.IndexOf("DE") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.DE);
        }
        else if (_s.IndexOf("FR") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.FR);
        }
        else if (_s.IndexOf("TH") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.TH);
        }
        else if (_s.IndexOf("VN") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.VN);
        }
        else if (_s.IndexOf("MX") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.MX);
        }
        else if (_s.IndexOf("TR") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.TR);
        }
        else if (_s.IndexOf("EG") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.EG);
        }
        else if (_s.IndexOf("IN") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.IN);
        }
        else if (_s.IndexOf("PK") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.PK);
        }
        else if (_s.IndexOf("BD") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.BD);
        }
        else if (_s.IndexOf("ZA") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.ZA);
        }
        else if (_s.IndexOf("NG") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.NG);
        }
        else if (_s.IndexOf("CO") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.CO);
        }
        else if (_s.IndexOf("AR") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.AR);
        }
        else if (_s.IndexOf("SA") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.SA);
        }
        else if (_s.IndexOf("AE") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.AE);
        }
        else if (_s.IndexOf("IQ") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.EN);
        }
        else if (_s.IndexOf("ES") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.ES);
        }
        else if (_s.IndexOf("IT") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.IT);
        }
        else if (_s.IndexOf("PL") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.PL);
        }
        else if (_s.IndexOf("NL") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.NL);
        }
        else if (_s.IndexOf("RO") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.RO);
        }
        else if (_s.IndexOf("SE") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.SE);
        }
        else if (_s.IndexOf("GR") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.GR);
        }
        //231114新增
        else if (_s.IndexOf("AT") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.AT);
        }
        else if (_s.IndexOf("CH") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.CH);
        }
        else if (_s.IndexOf("BE") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.BE);
        }
        else if (_s.IndexOf("NO") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.NO);
        }
        else if (_s.IndexOf("IE") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.IE);
        }
        else if (_s.IndexOf("DK") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.DK);
        }
        else if (_s.IndexOf("FI") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.FI);
        }
        else if (_s.IndexOf("BR") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.PT);
        }
        else if (_s.IndexOf("PE") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.PE);
        }
        else if (_s.IndexOf("EC") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.EC);
        }
        else if (_s.IndexOf("MY") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.MY);
        }
        else if (_s.IndexOf("CL") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.CL);
        }
        else if (_s.IndexOf("CZ") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.CZ);
        }
        else if (_s.IndexOf("HU") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.HU);
        }
        else
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.EN);
        }
    }


    /// <summary>
    /// 返回网络状态
    /// </summary>
    /// <param name="returnState">返回状态</param>
    public void ReturnNetState(string returnState)
    {
        if (returnState == "1")
        {
            NetWorkStateController.Instance.Show();
        }
        else
        {
            NetWorkStateController.Instance.Hidden();
        }
    }

    public static void PostTimer()
    {
        if (!CheckInited())
        {
            return;
        }
        activity.Call("GetTimerFromUnity");
    }

    public void GetTimer(string s)
    {
        NetWorkTimerManager.Instance.GetTimeFromAndroid(s);
        Debuger.Log("Get Timer From Android == " + s);
    }

    public static void PostEmail(String address, String title, String gameName, String userID, String packageName, String OSVersion)
    {
        if (!CheckInited())
        {
            return;
        }
        activity.Call("SendEmailFromUnity", address, title, gameName, userID, packageName, OSVersion);
    }

    
    public void PostEmail(string address, string title, string gameName, string playerID, string amount)
    {
        var _data = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
        activity.Call("SendEmailFromUnity", address, title, gameName, playerID, amount, _data);
    }

    public static void OpenOtherGame(string packageName)
    {
        if (!CheckInited())
        {
            return;
        }

        activity.Call("LaunchOtherApp", packageName);
    }

    #region GDPR

    public static bool CanShowGDPR()
    {
        if (!CheckInited())
        {
            Debuger.Log("CheckInited ==== false");
            return false;
        }
        var status = activity.Call<string>("canShowGDPR");
        return int.Parse(status).IntToBool();
    }

    private static IIsShowGDPRBtn isShowGDPRBtn;
    public static void ClickShowGDPR(IIsShowGDPRBtn value)
    {
        if (!CheckInited())
        {
            Debuger.Log("CheckInited ==== false");
            return;
        }

        isShowGDPRBtn = value;
        isShowGDPRBtn.BtnClickStatus(false);

        activity.Call("showPrivacyOptionsForm");
    }

    public void OnPrivacyOptionsFormShow(string status)
    {
        var _status = int.Parse(status).IntToBool();
        
        isShowGDPRBtn?.BtnClickStatus(_status);
    }

    #endregion

}
#endif
