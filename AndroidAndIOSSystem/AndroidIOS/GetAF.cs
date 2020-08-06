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

#if ADV2

        yield return new WaitForSeconds(8f);

        AnalysisController.AfStatus = CrossAndroid.GetAF();

#else

        //AdController.ShowGameStartInterstitial(PlayerData.CashCount >= 0.01f);
        CrossIos.Instance.GetAF(0);

#endif

        yield break;
    }
}
