using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class LoadingWindow : PopUIBase
{
    public override string thisPopUIEnum => PopUIEnum.LoadingWindow.ToString();

    public override string thisUIType => PopUIType.TOP.ToString();

    [SerializeField]
    Slider slider;

    [SerializeField]
    Image loadingImg;

    private const float timeLimit = 10f;

    private float Speed
    {
        get
        {
            if (AnalysisController.IsNonOrganic)
                return 3f;
            else return 1f;
        }
    }

    private Action callBack;

    void Start()
    {

#if UNITY_EDITOR || SafeMode || NoAd

#if !ADV2 && !SafeMode && UNITY_IPHONE
                        IOSIDFA.Instance.ShowIDFA(()=> {
                            PopUIManager.Instance.ShowUI(PopUIEnum.IOSIDFAUI);
                        },()=> { 
                            this.StartCoroutine(LoadScreen());
                           // HiddenUIAI();
                        });
                        return;
#else
            StartCoroutine(LoadScreenAndroid());
            // HiddenUIAI();
#endif

#elif UNITY_ANDROID
        StartCoroutine(LoadScreenAndroid());
      
#elif UNITY_IPHONE

        IOSIDFA.Instance.ShowIDFA(()=> {
           PopUIManager.Instance.ShowUI(PopUIEnum.IOSIDFAUI);
        },()=> { 
            this.StartCoroutine(LoadScreen());
        });
        return;

#endif


    }

    IEnumerator LoadScreen()
    {
        var i = 0f;

        while (i <= timeLimit)
        {
            i += Time.deltaTime * Speed;
            if (slider) slider.value = i / timeLimit;
            if (loadingImg) loadingImg.fillAmount = i / timeLimit;

            yield return null;
        }

        if (slider) slider.value = 1f;
        if (loadingImg) loadingImg.fillAmount = 1f;

        AdController.ShowGameStartInterstitial(GiftCardData.giftAValue.Value + GiftCardData.giftBValue.Value >= 0.01f);
        if (I2Language.Instance.IsGetLan &&
            (I2Language.Instance.Language == I2Language.LanguageEnum.DE ||
             I2Language.Instance.Language == I2Language.LanguageEnum.FR) &&
            GiftCardData.FristCheckGRDPpop.Value)
        {
            PopUIManager.Instance.SpawnUI<GRDPpop>(PopUIEnum.GRDPpop).OkBtnDownCallBcak(delegate { HiddenUIAI(); });
            PopUIManager.Instance.ShowUI(PopUIEnum.GRDPpop);
        }
        else
        {
            HiddenUIAI();
        }



    }


    IEnumerator LoadScreenAndroid()
    {
        var i = 0f;

        while (i <= timeLimit)
        {
            i += Time.deltaTime * Speed;
            if (slider) slider.value = i / timeLimit;
            if (loadingImg) loadingImg.fillAmount = i / timeLimit;

            yield return null;
        }

        var _sc = this.transform.GetComponentInChildren<LoadingCheckPP>();

        if (_sc)
        {
            _sc.Check(() => {
                if (slider) slider.value = 1f;
                if (loadingImg) loadingImg.fillAmount = 1f;
                HiddenUIAI();
            });
        }
    }

    public override void BeforShow(object[] value)
    {
        if (value.Count() >= 0)
            callBack = (Action)value[0];
    }

    public override void AfterHiddenUI()
    {
        callBack?.Invoke();
        callBack = null;
    }
}
