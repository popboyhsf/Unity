using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeAds : MonoBehaviour
{
    private const string MaxSdkKey = @"f6N71KFAOcJtC/daU32stkPYY4FugAznUulJt1t/SYd/pkm99EEr4nPRsxz1Gz2H4AbgVfOt3v+sBkLNHrXxhCwReZFRiYPswagoIkOKJ8H45/gZOFND9W4FJiTECA4F";

#if UNITY_IPHONE
    private const string InterstitialAdUnitId = "ENTER_IOS_INTERSTITIAL_AD_UNIT_ID_HERE";
    private const string RewardedAdUnitId = "ENTER_IOS_REWARD_AD_UNIT_ID_HERE";
    private const string RewardedInterstitialAdUnitId = "ENTER_IOS_REWARD_INTER_AD_UNIT_ID_HERE";
    private const string BannerAdUnitId = "ENTER_IOS_BANNER_AD_UNIT_ID_HERE";
    private const string MRecAdUnitId = "ENTER_IOS_MREC_AD_UNIT_ID_HERE";
#else // UNITY_ANDROID
    private const string InterstitialAdUnitId = @"iBS11cQnLuUb5rwVlwANllg6amUNmxEchfwVk6eTw2s=";
    private const string RewardedAdUnitId = @"y0dpzogQy0vAJb4dpp6A8lg6amUNmxEchfwVk6eTw2s=";
    private const string RewardedInterstitialAdUnitId = "ENTER_ANDROID_REWARD_INTER_AD_UNIT_ID_HERE";
    private const string BannerAdUnitId = @"T40gbLxr5KWSwyMmtRqxQVg6amUNmxEchfwVk6eTw2s=";
    private const string MRecAdUnitId = "ENTER_ANDROID_MREC_AD_UNIT_ID_HERE";
#endif

    private bool isBannerShowing;
    private bool isMRecShowing;

    private int interstitialRetryAttempt;
    private int rewardedRetryAttempt;

    private static bool isNeedInterstitialADShow = false;
    private static bool isNeedRewardedADShow = false;

    private bool isReward = false;


    private void Awake()
    {
        gameObject.name = "NativeObj";
        Application.targetFrameRate = 60;
#if !UNITY_ANDROID && !NativeAds
        gameObject.SetActive(false);
#endif
    }

    private void Start()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
        {
            // AppLovin SDK is initialized, configure and start loading ads.
            Debuger.Log("MAX SDK Initialized");

            InitializeInterstitialAds();
            InitializeRewardedAds();
            //InitializeRewardedInterstitialAds();
            InitializeBannerAds();
            //InitializeMRecAds();
        };

        MaxSdk.SetSdkKey(Utils.AESDecrypt(MaxSdkKey));
        MaxSdk.InitializeSdk();
    }


    #region Interstitial

    /// <summary>
    /// 播放插屏广告
    /// </summary>
    /// <param name="pos">0启动游戏,1切回游戏,2获取到奖励</param>
    /// <param name="must"></param>
    public static void ShowInterstitial(int pos,bool needAutoShow = false)
    {
       
        if (MaxSdk.IsInterstitialReady(Utils.AESDecrypt(InterstitialAdUnitId)))
        {
            MaxSdk.ShowInterstitial(Utils.AESDecrypt(InterstitialAdUnitId));
        }
        else
        {
            if(needAutoShow) 
                isNeedInterstitialADShow = true;
        }
    }




    private void InitializeInterstitialAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;

        // Load the first interstitial
        LoadInterstitial();
    }


    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(Utils.AESDecrypt(InterstitialAdUnitId));
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(interstitialAdUnitId) will now return 'true'
        Debuger.Log("Interstitial loaded");

        // Reset retry attempt
        interstitialRetryAttempt = 0;

        if (isNeedInterstitialADShow)
        {
            MaxSdk.ShowInterstitial(Utils.AESDecrypt(InterstitialAdUnitId));
        }
    }

    private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        interstitialRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));

        Debuger.Log("Interstitial failed to load with error code: " + errorInfo.Code);

        Invoke("LoadInterstitial", (float)retryDelay);
    }

    private void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. We recommend loading the next ad
        Debuger.Log("Interstitial failed to display with error code: " + errorInfo.Code);
        LoadInterstitial();
    }

    private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad
        Debuger.Log("Interstitial dismissed");
        LoadInterstitial();


        AdController.ShowInterstitialCallBack();
    }

    private void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad revenue paid. Use this callback to track user revenue.
        Debuger.Log("Interstitial revenue paid");

    }


    #endregion

    #region Reward

    /// <summary>
    /// 播放视频广告
    /// </summary>
    public static void ShowRewardedVideo(int entry)
    {
        Debuger.Log("ShowRewardedVideo");

        if (MaxSdk.IsRewardedAdReady(Utils.AESDecrypt(RewardedAdUnitId)))
        {
            MaxSdk.ShowRewardedAd(Utils.AESDecrypt(RewardedAdUnitId));
        }
        else
        {
            isNeedRewardedADShow = true;
            ADLoading.Instance.ShowLoading();
        }
    }

    public static void ReqHideLoadingRewardVideoWindow()
    {
        isNeedRewardedADShow = false;
    }


    private void InitializeRewardedAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;

        // Load the first RewardedAd
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(Utils.AESDecrypt(RewardedAdUnitId));
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'

        Debuger.Log("Rewarded ad loaded");

        // Reset retry attempt
        rewardedRetryAttempt = 0;
        if (isNeedRewardedADShow)
        {
            MaxSdk.ShowRewardedAd(Utils.AESDecrypt(RewardedAdUnitId));
        }
    }

    private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        rewardedRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, rewardedRetryAttempt));

        Debuger.Log("Rewarded ad failed to load with error code: " + errorInfo.Code);

        Invoke("LoadRewardedAd", (float)retryDelay);
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. We recommend loading the next ad
        Debuger.Log("Rewarded ad failed to display with error code: " + errorInfo.Code);
        LoadRewardedAd();
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debuger.Log("Rewarded ad displayed");
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debuger.Log("Rewarded ad clicked");
    }

    private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        Debuger.Log("Rewarded ad dismissed");
        LoadRewardedAd();

        if (isNeedRewardedADShow)
        {
            ADLoading.Instance.HiddenLoading();
            isNeedRewardedADShow = false;
        }

        if (isReward)
        {
            isReward = false;

            Debuger.Log("WatchRewardVideoComplete");

            AdController.ShowRewardedVideoCallBack();
        }
        else
        {
            Debuger.Log("WatchRewardVideoFail");

            AdController.ShowRewardedVideoFail();
        }
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad was displayed and user should receive the reward
        Debuger.Log("Rewarded ad received reward");

        isReward = true;
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad revenue paid. Use this callback to track user revenue.
        Debuger.Log("Rewarded ad revenue paid");

    }

    #endregion

    #region Banner

    private void InitializeBannerAds()
    {
        // Attach Callbacks
        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;

        // Banners are automatically sized to 320x50 on phones and 728x90 on tablets.
        // You may use the utility method `MaxSdkUtils.isTablet()` to help with view sizing adjustments.
        MaxSdk.CreateBanner(Utils.AESDecrypt(BannerAdUnitId), MaxSdkBase.BannerPosition.BottomCenter);

        // Set background or background color for banners to be fully functional.
        MaxSdk.SetBannerBackgroundColor(Utils.AESDecrypt(BannerAdUnitId), Color.black);

        MaxSdk.StartBannerAutoRefresh(Utils.AESDecrypt(BannerAdUnitId));

        ToggleBannerVisibility();
    }

    private void ToggleBannerVisibility()
    {
        if (!isBannerShowing)
        {
            MaxSdk.ShowBanner(Utils.AESDecrypt(BannerAdUnitId));
        }
        else
        {
            MaxSdk.HideBanner(Utils.AESDecrypt(BannerAdUnitId));
        }

        isBannerShowing = !isBannerShowing;
    }

    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Banner ad is ready to be shown.
        // If you have already called MaxSdk.ShowBanner(BannerAdUnitId) it will automatically be shown on the next ad refresh.
        Debuger.Log("Banner ad loaded");
    }

    private void OnBannerAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Banner ad failed to load. MAX will automatically try loading a new ad internally.
        Debuger.Log("Banner ad failed to load with error code: " + errorInfo.Code);
    }

    private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debuger.Log("Banner ad clicked");
    }

    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Banner ad revenue paid. Use this callback to track user revenue.
        Debuger.Log("Banner ad revenue paid");


    }

    #endregion

}
