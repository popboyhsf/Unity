﻿using System;
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
    public static void CashOut(float balance,string s)
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
}