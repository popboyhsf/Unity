using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAF : MonoBehaviour
{

    BoolData isNotCallAF = new BoolData("GetAF_AnalysisControllerLuckReady", true);

    void Start()
    {
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(4f);

#if !SafeMode

    #if ADV2

            AnalysisController.AfStatus = CrossAndroid.GetAF();


    #else

            //AdController.ShowGameStartInterstitial(PlayerData.CashCount >= 0.01f);
            CrossIos.Instance.GetAF(0);

    #endif

#endif



        if (isNotCallAF.Value)
        {
            if (GoldData.giftNum.Value <= 0f)
            {
                if (AnalysisController.IsNonOrganic)
                {
                    AnalysisController.TraceEvent(EventName.luck_ready + "_4");
                    isNotCallAF.Value = false;
                    yield break;
                }
            }

            yield return new WaitForSeconds(6f); //6+4

            if (GoldData.giftNum.Value <= 0f)
            {
                if (AnalysisController.IsNonOrganic)
                {
                    AnalysisController.TraceEvent(EventName.luck_ready + "_10");
                    isNotCallAF.Value = false;
                    yield break;
                }
            }


            yield return new WaitForSeconds(10f); //10+6+4

            if (GoldData.giftNum.Value <= 0f)
            {
                if (AnalysisController.IsNonOrganic)
                {
                    AnalysisController.TraceEvent(EventName.luck_ready + "_20");
                    isNotCallAF.Value = false;
                    yield break;
                }
            }

            yield return new WaitForSeconds(10f); //10+10+6+4

            if (GoldData.giftNum.Value <= 0f)
            {
                if (AnalysisController.IsNonOrganic)
                {
                    AnalysisController.TraceEvent(EventName.luck_ready + "_30");
                    isNotCallAF.Value = false;
                    yield break;
                }
            }

            yield return new WaitForSeconds(30f); //30+10+10+6+4

            if (GoldData.giftNum.Value <= 0f)
            {
                if (AnalysisController.IsNonOrganic)
                {
                    AnalysisController.TraceEvent(EventName.luck_ready + "_60");
                    isNotCallAF.Value = false;
                    yield break;
                }
            }

            yield return new WaitForSeconds(60f); //60+30+10+10+6+4

            if (GoldData.giftNum.Value <= 0f)
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
            if (GoldData.giftNum.Value >= 0.1f)
            {
                if (!AnalysisController.IsNonOrganic)
                    AnalysisController.TraceEvent(EventName.luck_miss);
            }
        }




        yield break;
    }
}
