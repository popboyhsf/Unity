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
    public static extern void isRewardVideoReady(bool isCash);

    [DllImport("__Internal")]
    public static extern void rewardVideoCancel();

    [DllImport("__Internal")]
    public static extern void startVibrator(int type);
        
    [DllImport("__Internal")]
    public static extern void gameStart(bool type);     
        
    [DllImport("__Internal")]
    public static extern void getAF();   

    [DllImport("__Internal")]
    public static extern void GetUnityPostInt(int type);   

    [DllImport("__Internal")]
    public static extern void GetUrlForIcon();   

    [DllImport("__Internal")]
    public static extern void LogEvetnForTrackLuckBalance(int j,float i);

    [DllImport("__Internal")]
    public static extern void CashOutI(float i,string j);

    [DllImport("__Internal")]
    public static extern void PushMessage();

    
    [DllImport("__Internal")]
    public static extern void GetTimerFromUnity();

    [DllImport("__Internal")]
    public static extern void showIDFA();
	
	[DllImport("__Internal")]
    public static extern void requestIDFA();
    
    [DllImport("__Internal")]
    public static extern void logEvetnForIDFA();

    [DllImport("__Internal")]
    public static extern int canShowIDFA();

    [DllImport("__Internal")]
    public static extern void rateUS(int count,int max，string patch);

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
        
#endif
        _instance = this;
        CheckInited();
        GetVersionInfo();
    }

    private void Start()
    {

    }

    public static UnityAction RewardVideoCompletedAction;
    public static UnityAction RewardVideoOpenCallback;
    public static UnityAction RewardVideoCloseCallback;
    public static UnityAction RewardVideoFailCallback;

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
    /// <param name="pos">0 表示gamestart，1表示gameresume，2表示gameended</param>
    public static void ShowInterstitial(int pos = 2, UnityAction openCallback = null, UnityAction closeCallback = null)
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

    public static void ShowRewardedVideo(int entry, UnityAction watchCompletedAction, UnityAction watchStartAction = null, UnityAction watchClossAction = null, UnityAction watchFailAction = null)
    {
        Debuger.Log("ShowRewardedVideo");
        if (!CheckInited())
        {
            return;
        }
        RewardVideoCompletedAction = watchCompletedAction;
        RewardVideoOpenCallback = watchStartAction;
        RewardVideoCloseCallback = watchClossAction;
        RewardVideoFailCallback = watchFailAction;
#if UNITY_IPHONE && !UNITY_EDITOR && !SafeMode
        showRewardBasedVideoParam(entry);
#endif
    }

    public static void ShowRewardedVideo(UnityAction watchCompletedAction, UnityAction watchStartAction = null, UnityAction watchClossAction = null, UnityAction watchFailAction = null)
    {
        Debuger.Log("ShowRewardedVideo");
        if (!CheckInited())
        {
            return;
        }
        RewardVideoCompletedAction = watchCompletedAction;
        RewardVideoOpenCallback = watchStartAction;
        RewardVideoCloseCallback = watchClossAction;
        RewardVideoFailCallback = watchFailAction;
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
        RewardVideoCloseCallback?.Invoke();
        RewardVideoCompletedAction = null;
        RewardVideoOpenCallback = null;
        RewardVideoCloseCallback = null;
        RewardVideoFailCallback = null;
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

    public static void LogEvetnForTrackLuckBalance(float i, int j)
    {
        if (!CheckInited())
        {
            return;
        }
#if UNITY_IPHONE && !UNITY_EDITOR && !SafeMode
            LogEvetnForTrackLuckBalance(j, i);         
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
        
        if (returnCode == "success")
            RewardVideoCompletedAction?.Invoke();
        else
        {
            
        }
        RewardVideoFailCallback?.Invoke();
        RewardVideoClose();
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
            PushMessage();
#endif
    }


    public void ShowLoadingRewardVideoWindow(string returnCode)
    {
        //ADLoading.Instance.ShowLoading();
    }

    public void HideLoadingRewardVideoWindow(string returnCode)
    {
        ADLoading.Instance.HiddenLoading();
    }


    #region 请求广告

    private static IIsViedoReady thisIsReadyI;

    /// <summary>
    /// 请求广告是否加载完毕
    /// </summary>
    /// <param name="isViedoReady">接口类</param>
    public static void VideoIsReady(IIsViedoReady isViedoReady, bool isCash)
    {
        if (!CheckInited())
        {
            return;
        }
#if UNITY_IPHONE && !UNITY_EDITOR && !SafeMode
        isRewardVideoReady(isCash);
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
#if UNITY_IPHONE && !UNITY_EDITOR && !SafeMode
            GetUnityPostInt(i);         
#endif

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
            //ItemSystemData.AddItem(ItemSystemData.ItemEnum.chuizi, num);
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
#if UNITY_IPHONE && !UNITY_EDITOR && !SafeMode
            GetUrlForIcon();         
#endif

    }

    //接受url
    public void GetUrlForIconCallBack(string url)
    {
        PostAndGetIcon.Instance.GetIcon(url);
    }

    //CashOut
    public static void CashOut(float balance, string s)
    {
        balance = (float)Math.Round(balance, 2);

        Debuger.Log("CashOut === " + balance);
        Debuger.Log("CashOut === " + s);

        if (!CheckInited())
        {
            return;
        }

#if UNITY_IPHONE && !UNITY_EDITOR && !SafeMode
            CashOutI(balance,s);         
#endif

    }

    #endregion

    #region 联网与时间

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
#if UNITY_IPHONE && !UNITY_EDITOR && !SafeMode
            GetTimerFromUnity();         
#endif
    }

    public void GetTimer(string s)
    {
        NetWorkTimerManager.Instance.GetTimeFromAndroid(s);
        Debuger.Log("Get Timer From IOS == " + s);
    }

    #endregion

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


    /// <summary>
    /// af返回买量跟自然区分
    /// </summary>
    /// <param name="status"></param>
    public void AppsFlyerState(string status)
    {
        if ((AnalysisController.AFStatus)(int.Parse(status)) == AnalysisController.AFStatus.NonOrganic)
            AnalysisController.AfStatus = (AnalysisController.AFStatus)(int.Parse(status));
        Debug.LogWarning("AFSET === " + AnalysisController.AfStatus);
    }

    private bool isCheckPhy = false;
    /// <summary>
    /// 显示静音状态弹窗
    /// </summary>
    public void ShowWindowsMute()
    {
        if (!isCheckPhy)
        {
            isCheckPhy = true;
            //PopUIManager.Instance.ShowUI(PopUIEnum.PhyMuteTip);
        }
    }

    public void GetAF(float timer)
    {
        StartCoroutine(GetAFI(timer));
    }

    private IEnumerator GetAFI(float timer)
    {
        yield return new WaitForSeconds(timer);

        Debug.Log("Send Massage To IOS === GetAF");

#if UNITY_IPHONE && !UNITY_EDITOR && !SafeMode
            getAF();          
#endif
        yield break;
    }

    /// <summary>
    /// 調用系統評分
    /// </summary>
    /// <param name="count"></param>
    /// <param name="patch"></param>
    public static void RateUS(int count, int max, string patch)
    {
        if (!CheckInited())
        {
            return;
        }

#if UNITY_IPHONE && !UNITY_EDITOR && !SafeMode
        rateUS(count, max, patch);
#endif
    }



    #region IDFA彈窗交互

    private static UnityAction ClickAllowCallBack;

    /// <summary>
    /// 展示IDFA
    /// </summary>
    /// <param name="callback"></param>
    public static void ShowIDFA(UnityAction callback)
    {
        Debuger.Log("ShowIDFA");
        if (!CheckInited())
        {
            return;
        }

        ClickAllowCallBack = callback;

#if UNITY_IPHONE && !UNITY_EDITOR && !SafeMode
        showIDFA();
#endif
    }

    /// <summary>
    /// 關閉IDFA后回調
    /// </summary>
    public void IDFACallBack()
    {
        if (ClickAllowCallBack == null)
        {
            IOSIDFA.Instance.ForceClickAllow();
            return;
        }
        ClickAllowCallBack?.Invoke();
    }
	
	
    /// <summary>
    /// 打開引導IDFA去系統設置
    /// </summary>
    public static void RequestIDFA()
    {
        if (!CheckInited())
        {
            return;
        }
#if UNITY_IPHONE && !UNITY_EDITOR && !SafeMode
        requestIDFA();
#endif
    }

    /// <summary>
    /// 傳遞IDFA的AF事件
    /// </summary>
    public static void LogEvetnForIDFA()
    {
        if (!CheckInited())
        {
            return;
        }
#if UNITY_IPHONE && !UNITY_EDITOR && !SafeMode
        logEvetnForIDFA();
#endif
    }

    /// <summary>
    /// 是否展示IDFA窗口
    /// </summary>
    /// <returns></returns>
    public static bool CanShowIDFA()
    {
        var _b = false;
        if (!CheckInited())
        {
            _b = true;
        }
#if UNITY_IPHONE && !UNITY_EDITOR && !SafeMode
       _b =  canShowIDFA() == 0;
#endif
        return _b;
    }

    #endregion
}