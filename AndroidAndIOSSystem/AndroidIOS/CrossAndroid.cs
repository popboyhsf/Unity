﻿using Mkey;
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

    private void Awake()
    {
        gameObject.name = "CrossAndroidObject";
        Application.targetFrameRate = 60;

#if !UNITY_ANDROID
        gameObject.SetActive(false);
#endif
    }

    private void Start()
    {
        WpSdkAndroid.AF.onSucessEvent += (bool sucessed) => {

            if (sucessed)
                AnalysisController.AfStatus = AnalysisController.AFStatus.NonOrganic;
            else
                AnalysisController.AfStatus = AnalysisController.AFStatus.Organic;

        };

        WpSdkAndroid.AF.onFailEvent += (string msg) => {

            Debuger.LogWarning(msg);

            AnalysisController.AfStatus = AnalysisController.AFStatus.Unknow;

        };



        WpSdkAndroid.Rewarded.onAdNotReadyEvent += ShowLoadingRewardVideoWindow;

        WpSdkAndroid.Rewarded.onAdDisplayStartEvent += HideLoadingRewardVideoWindow;



        WpSdkAndroid.Interstitial.onAdHiddenEvent += WatchInterstitialComplete;

        WpSdkAndroid.Rewarded.onAdHiddenEvent += WatchRewardVideoComplete;


        WpSdkAndroid.GDPR.onOptionFromShow += OnPrivacyOptionsFormShow;

        SimpleTween.DelayAction(this.gameObject, 1f, () => {
            getCountry();
        });

    }


    /// <summary>
    /// 播放插屏广告
    /// </summary>
    /// <param name="pos">0启动游戏,1切回游戏,2获取到奖励</param>
    /// <param name="must"></param>
    public static void ShowInterstitial(int pos)
    {
        WpSdkAndroid.instance.showIntAd();
    }

    /// <summary>
    /// 观看插屏结束,,unity需在此发放奖励
    /// </summary>
    /// <param name="returnCode"></param>
    public void WatchInterstitialComplete()
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
        WpSdkAndroid.instance.showRewardAd();
    }


    /// <summary>
    /// 观看视频结束,,unity需在此发放奖励
    /// </summary>
    /// <param name="returnCode"></param>
    public void WatchRewardVideoComplete()
    {
        Debuger.Log("WatchRewardVideoComplete");
        AdController.ShowRewardedVideoCallBack();
    }

    public void WatchRewardVideoFail()
    {
        Debuger.Log("WatchRewardVideoFail");
        AdController.ShowRewardedVideoFail();
    }

    public void ShowLoadingRewardVideoWindow()
    {
        ADLoading.Instance.ShowLoading();
    }

    public void HideLoadingRewardVideoWindow()
    {
        ADLoading.Instance.HiddenLoading();
    }

    public static void LogEvent(String eventName, String jsonStr)
    {
        WpSdkAndroid.instance.logEvent(eventName);
    }

    /// <summary>
    /// 传递礼品卡信息
    /// </summary>
    /// <param name="i">当前礼品卡总额</param>
    /// <param name="j">当前礼品卡梯度</param>
    public static void LogEvetnForTrack(float i, int j = 200)
    {
        WpSdkAndroid.instance.LogEventWithBalance();
    }

    #region Conutry

    private void getCountry()
    {
        var _c = "nil";

        _c = GetCountry();


        if (!_c.Equals("nil"))
        {
            ReturnContry(_c);
            ReturnContryChangeUI(_c);

            var _msg = EventName.gameAFName + "_lucky_lang_" + _c;

            LogEvent(_msg, "");

            SimpleTween.DelayAction(this.gameObject, 1f, () =>
            {
                WpSdkAndroid.instance.LogEventWithVersionCode(_msg);

            });

            Debuger.Log($"获取Country = {_c}");
        }
        else
        {
#if UNITY_EDITOR
            Debuger.LogWarning("获取Country请在手机上测试");
#else
            Debuger.LogError("获取Country错误");
#endif

        }

    }

    private string GetCountry()
    {
        string var1;
        string var3;
        int var4;


        {
            var1 = WpSdkAndroid.instance.getLanguageCode();
            string var2 = WpSdkAndroid.instance.getCountryCode();

            Debuger.Log($"Language = {var1}, Country = {var2}");

            var3 = null;
            var4 = -1;

            Dictionary<string, int> countryCodes = new Dictionary<string, int>
            {
                {"AE", 0},
                {"AR", 1},
                {"AT", 2},
                {"AU", 3},
                {"BD", 4},
                {"BE", 5},
                {"BR", 6},
                {"CA", 7},
                {"CH", 8},
                {"CL", 9},
                {"CO", 10},
                {"CZ", 11},
                {"DE", 12},
                {"DK", 13},
                {"EC", 14},
                {"EG", 15},
                {"ES", 16},
                {"ET", 17},
                {"FI", 18},
                {"FR", 19},
                {"GB", 20},
                {"GH", 21},
                {"GR", 22},
                {"HU", 23},
                {"ID", 24},
                {"IE", 25},
                {"IN", 26},
                {"IQ", 27},
                {"IT", 28},
                {"JP", 29},
                {"KE", 30},
                {"KR", 31},
                {"KW", 32},
                {"MX", 33},
                {"MY", 34},
                {"NG", 35},
                {"NL", 36},
                {"NO", 37},
                {"PE", 38},
                {"PH", 39},
                {"PK", 40},
                {"PL", 41},
                {"PT", 42},
                {"QA", 43},
                {"RO", 44},
                {"RU", 45},
                {"SA", 46},
                {"SE", 47},
                {"TH", 48},
                {"TR", 49},
                {"US", 50},
                {"VN", 51},
                {"ZA", 52},
                {"TW", 53},
                {"KZ", 54},
            };

            if (countryCodes.ContainsKey(var2))
            {
                var4 = countryCodes[var2];
            }

        }

        string var6;

        {
            switch (var4)
            {
                case 0:
                    var6 = "LANG_AE";
                    break;
                case 1:
                    var6 = "LANG_AR";
                    break;
                case 2:
                    var6 = "LANG_AT";
                    break;
                case 3:
                    var6 = "LANG_AU";
                    break;
                case 4:
                    var6 = "LANG_BD";
                    break;
                case 5:
                    var6 = "LANG_BE";
                    break;
                case 6:
                    var6 = "LANG_PT";
                    break;
                case 7:
                    var6 = "LANG_CA";
                    break;
                case 8:
                    var6 = "LANG_CH";
                    break;
                case 9:
                    var6 = "LANG_CL";
                    break;
                case 10:
                    var6 = "LANG_CO";
                    break;
                case 11:
                    var6 = "LANG_CZ";
                    break;
                case 12:
                    var6 = "LANG_DE";
                    break;
                case 13:
                    var6 = "LANG_DK";
                    break;
                case 14:
                    var6 = "LANG_EC";
                    break;
                case 15:
                    var6 = "LANG_EG";
                    break;
                case 16:
                    var6 = "LANG_ES";
                    break;
                case 17:
                    var6 = "LANG_ET";
                    break;
                case 18:
                    var6 = "LANG_FI";
                    break;
                case 19:
                    var6 = "LANG_FR";
                    break;
                case 20:
                    var6 = "LANG_GB";
                    break;
                case 21:
                    var6 = "LANG_GH";
                    break;
                case 22:
                    var6 = "LANG_GR";
                    break;
                case 23:
                    var6 = "LANG_HU";
                    break;
                case 24:
                    var6 = "LANG_ID";
                    break;
                case 25:
                    var6 = "LANG_IE";
                    break;
                case 26:
                    var6 = "LANG_IN";
                    break;
                case 27:
                    var6 = "LANG_IQ";
                    break;
                case 28:
                    var6 = "LANG_IT";
                    break;
                case 29:
                    var6 = "LANG_JP";
                    break;
                case 30:
                    var6 = "LANG_KE";
                    break;
                case 31:
                    var6 = "LANG_KR";
                    break;
                case 32:
                    var6 = "LANG_KW";
                    break;
                case 33:
                    var6 = "LANG_MX";
                    break;
                case 34:
                    var6 = "LANG_MY";
                    break;
                case 35:
                    var6 = "LANG_NG";
                    break;
                case 36:
                    var6 = "LANG_NL";
                    break;
                case 37:
                    var6 = "LANG_NO";
                    break;
                case 38:
                    var6 = "LANG_PE";
                    break;
                case 39:
                    var6 = "LANG_PH";
                    break;
                case 40:
                    var6 = "LANG_PK";
                    break;
                case 41:
                    var6 = "LANG_PL";
                    break;
                case 42:
                    var6 = "LANG_BR";
                    break;
                case 43:
                    var6 = "LANG_QA";
                    break;
                case 44:
                    var6 = "LANG_RO";
                    break;
                case 45:
                    var6 = "LANG_RU";
                    break;
                case 46:
                    var6 = "LANG_SA";
                    break;
                case 47:
                    var6 = "LANG_SE";
                    break;
                case 48:
                    var6 = "LANG_TH";
                    break;
                case 49:
                    var6 = "LANG_TR";
                    break;
                case 50:
                    var6 = "LANG_US";
                    break;
                case 51:
                    var6 = "LANG_VN";
                    break;
                case 52:
                    var6 = "LANG_ZA";
                    break;
                case 53:
                    var6 = "LANG_TW";
                    break;
                case 54:
                    var6 = "LANG_KZ";
                    break;
                default:
                    var6 = null;
                    break;
            }

            var3 = var6;
        }

        {
            if (var3 == null)
            {
                if (var1.Equals("ko"))
                {
                    var6 = "LANG_KR";
                }
                else if (var1.Equals("ja"))
                {
                    var6 = "LANG_JP";
                }
                else if (var1.Equals("ru"))
                {
                    var6 = "LANG_RU";
                }
                else if (var1.Equals("pt"))
                {
                    var6 = "LANG_PT";
                }
                else if (var1.Equals("in"))
                {
                    var6 = "LANG_ID";
                }
                else if (!var1.Equals("tl") && !var1.Equals("fil"))
                {
                    if (var1.Equals("es"))
                    {
                        var6 = "LANG_MX";
                    }
                    else if (var1.Equals("th"))
                    {
                        var6 = "LANG_TH";
                    }
                    else if (var1.Equals("vi"))
                    {
                        var6 = "LANG_VN";
                    }
                    else if (var1.Equals("de"))
                    {
                        var6 = "LANG_DE";
                    }
                    else if (var1.Equals("fr"))
                    {
                        var6 = "LANG_FR";
                    }
                    else if (var1.Equals("ar"))
                    {
                        var6 = "LANG_EG";
                    }
                    else if (var1.Equals("tr"))
                    {
                        var6 = "LANG_TR";
                    }
                    else
                    {
                        if (!var1.Equals("en"))
                        {

                            switch (var1)
                            {
                                case "hi":
                                    var6 = "LANG_IN";
                                    break;
                                case "ur":
                                    var6 = "LANG_PK";
                                    break;
                                case "bn":
                                    var6 = "LANG_BD";
                                    break;
                                case "ha":
                                    var6 = "LANG_NG";
                                    break;
                                case "am":
                                    var6 = "LANG_ET";
                                    break;
                                case "sw":
                                    var6 = "LANG_KE";
                                    break;
                                case "nl":
                                    var6 = "LANG_NL";
                                    break;
                                case "it":
                                    var6 = "LANG_IT";
                                    break;
                                case "sv":
                                    var6 = "LANG_SE";
                                    break;
                                case "pl":
                                    var6 = "LANG_PL";
                                    break;
                                case "ro":
                                    var6 = "LANG_RO";
                                    break;
                                case "el":
                                    var6 = "LANG_GR";
                                    break;
                                case "nb":
                                    var6 = "LANG_NO";
                                    break;
                                case "ga":
                                    var6 = "LANG_IE";
                                    break;
                                case "da":
                                    var6 = "LANG_DK";
                                    break;
                                case "fi":
                                    var6 = "LANG_FI";
                                    break;
                                case "ms":
                                    var6 = "LANG_MY";
                                    break;
                                case "cs":
                                    var6 = "LANG_CZ";
                                    break;
                                case "hu":
                                    var6 = "LANG_HU";
                                    break;
                                case "tw":
                                    var6 = "LANG_TW";
                                    break;
                                default:
                                    var6 = "LANG_OTH";
                                    break;
                            }
                        }
                        else
                        {
                            var6 = "LANG_US";
                        }

                    }
                }
                else
                {
                    var6 = "LANG_PH";
                }

                var3 = var6;
            }
        }

        if (var3 == null)
        {
            var3 = "LANG_OTH";
            Debuger.LogWarning("没有根据手机获取到语言,强制OTH");
        }


        return var3;
    }


    /// <summary>
    /// 返回国家 --- 需要在Android - GetAF后执行
    /// </summary>
    /// <param name="returnC"></param>
    public static void ReturnContry(string returnC)
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
        else if (_s.IndexOf("TW") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.TW);
        }
        else if (_s.IndexOf("GB") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.GB);
        }
        else if (_s.IndexOf("KZ") >= 0)
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.KZ);
        }
        else
        {
            I2Language.Instance.ApplyLanguage(I2Language.LanguageEnum.EN);
        }
    }

    private static void ReturnContryChangeUI(string returnC)
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
        else if (_s.IndexOf("TW") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.TW);
        }
        else if (_s.IndexOf("GB") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.GB);
        }
        else if (_s.IndexOf("KZ") >= 0)
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.KZ);
        }
        else
        {
            I2Language.Instance.ChangeUI(I2Language.LanguageEnum.EN);
        }
    }

    #endregion


    #region GDPR

    public static bool CanShowGDPR()
    {
        return WpSdkAndroid.instance.canShowGDPR();
    }

    private static IIsShowGDPRBtn isShowGDPRBtn;
    public static void ClickShowGDPR(IIsShowGDPRBtn value)
    {
        WpSdkAndroid.instance.showPrivacyOptionsForm();
    }

    public void OnPrivacyOptionsFormShow(bool status)
    {
        //var _status = status;
        var _status = true;

        isShowGDPRBtn?.BtnClickStatus(_status);
    }

    #endregion

}
#endif
