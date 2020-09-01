using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAF : MonoBehaviour
{

    void Start()
    {
#if ADV2


#if UNITY_EDITOR || SafeMode
        
        return;
#elif UNITY_ANDROID
        StartCoroutine(Delay());

#elif UNITY_IPHONE
        
#endif


#else



#if UNITY_EDITOR || SafeMode
        
        return;
#elif UNITY_ANDROID
            
#elif UNITY_IPHONE

        StartCoroutine(Delay());

#endif


#endif
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(4f);

#if ADV2

        AnalysisController.AfStatus = CrossAndroid.GetAF();


#else

        //AdController.ShowGameStartInterstitial(PlayerData.CashCount >= 0.01f);
        CrossIos.Instance.GetAF(0);

#endif


        if (GoldData.giftNum.Value >= 0.1f)
        {
            if (!AnalysisController.IsNonOrganic)
                AnalysisController.TraceEvent(EventName.luck_miss);
        }

        yield return new WaitForSeconds(4f);

        if (FirstCheck.GetIsGameFirst("GetAF_AnalysisControllerLuckReady") && GoldData.giftNum.Value <= 0f )
        {
            if (AnalysisController.IsNonOrganic)
                AnalysisController.TraceEvent(EventName.luck_ready);
        }



        yield break;
    }
}
