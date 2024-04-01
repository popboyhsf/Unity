using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAF : MonoBehaviour,IDebuger
{

    BoolData isNotCallAF = new BoolData("GetAF_AnalysisControllerLuckReady", true);

    private bool allowDebug = false;
    public bool AllowDebug
    {
        get => allowDebug;
        set
        {
            allowDebug = value;

            if (!AnalysisController.IsNonOrganic)
            {
                AnalysisController.AfStatus =
                    allowDebug
                    ?
                    AnalysisController.AFStatus.NonOrganic
                    :
                    AnalysisController.AFStatus.Organic;

            }
        }
    }

    public string AllowName => "FackAF";

    void Start()
    {
#if NativeAds
        NativeAF.GetConutry();
#endif
        StartCoroutine(Delay());
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            if (allowDebug)
                StartCoroutine(nameof(DelayFackAF));
        }
    }

    IEnumerator DelayFackAF()
    {
        yield return new WaitForSecondsRealtime(3f);

        if (allowDebug)
        {

            AnalysisController.AfStatus = AnalysisController.AFStatus.NonOrganic;
            Debuger.Log("OnApplicationPause AfSet == " + AnalysisController.AfStatus);
        }
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(4f);
#if UNITY_EDITOR

        if(I2Language.Instance) 
            I2Language.Instance.ApplyLanguage(FackI2Language.Instance == null ? I2Language.LanguageEnum.EN : FackI2Language.Instance.LanguageLocal);

#endif


#if !SafeMode

#if ADV2

        if (!allowDebug)
        {
            AnalysisController.OnAFStatusChanged?.Invoke();
#if NativeAds
            NativeAF.GetAF();

#else
            CrossAndroid.GetAF();
#endif
        }


#else

        //AdController.ShowGameStartInterstitial(PlayerData.CashCount >= 0.01f);
        AnalysisController.OnAFStatusChanged?.Invoke();
        CrossIos.Instance.GetAF(0);

#endif

#endif


        float _gold = -996; //TODO 后期接入礼品卡金额

        _gold = GoldData.giftNum.Value;

        if (_gold == -996) Debuger.LogError("尚未接入礼品卡");

        if (isNotCallAF.Value)
        {
            if (_gold <= 0f)
            {
                if (AnalysisController.IsNonOrganic)
                {
                    AnalysisController.TraceEvent(EventName.luck_ready + "_4");
                    isNotCallAF.Value = false;
                    yield break;
                }
            }

            yield return new WaitForSeconds(6f); //6+4

            if (_gold <= 0f)
            {
                if (AnalysisController.IsNonOrganic)
                {
                    AnalysisController.TraceEvent(EventName.luck_ready + "_10");
                    isNotCallAF.Value = false;
                    yield break;
                }
            }


            yield return new WaitForSeconds(10f); //10+6+4

            if (_gold <= 0f)
            {
                if (AnalysisController.IsNonOrganic)
                {
                    AnalysisController.TraceEvent(EventName.luck_ready + "_20");
                    isNotCallAF.Value = false;
                    yield break;
                }
            }

            yield return new WaitForSeconds(10f); //10+10+6+4

            if (_gold <= 0f)
            {
                if (AnalysisController.IsNonOrganic)
                {
                    AnalysisController.TraceEvent(EventName.luck_ready + "_30");
                    isNotCallAF.Value = false;
                    yield break;
                }
            }

            yield return new WaitForSeconds(30f); //30+10+10+6+4

            if (_gold <= 0f)
            {
                if (AnalysisController.IsNonOrganic)
                {
                    AnalysisController.TraceEvent(EventName.luck_ready + "_60");
                    isNotCallAF.Value = false;
                    yield break;
                }
            }

            yield return new WaitForSeconds(60f); //60+30+10+10+6+4

            if (_gold <= 0f)
            {
                if (AnalysisController.IsNonOrganic)
                {
                    AnalysisController.TraceEvent(EventName.luck_ready + "_120");
                    isNotCallAF.Value = false;
                    yield break;
                }
            }

        }
        else
        {
            AnalysisController.OnAFStatusChanged += () =>
            {
                if (_gold >= 0.1f)
                {
                    if (!AnalysisController.IsNonOrganic)
                        AnalysisController.TraceEvent(EventName.luck_miss);
                }
            };
        }




        yield break;
    }
}
