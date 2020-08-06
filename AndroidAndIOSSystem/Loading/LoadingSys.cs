using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LoadingSys : MonoBehaviour
{
    private const float timeLimit = 10f;
    private float timer = 0f;

    public TextMeshProUGUI text;

    private float speed = 0;

    private GameObject temp;

    void Start()
    {

#if UNITY_EDITOR
        Destroy(this.gameObject,1f);
#endif

#if !UNITY_IPHONE
        Destroy(this.gameObject);
#endif


        speed = 100f / timeLimit;
    }

    private void Update()
    {

        if (timer >= 100)
        {
            Destroy(this.gameObject);
        }
        else
        {
            timer += speed * Time.deltaTime;
            text.text = timer.ToString("0") + "%";
        }
    }

    private void OnDestroy()
    {


#if UNITY_IPHONE
        CrossIos.Instance.GetAF(0);
#endif

        AdController.ShowGameStartInterstitial(MoneyData.Instance.GetGoldMoney >= 0.01f);


        GameStart();
    }


    private void GameStart()
    {

        if (Utils.SpendDaySince1970 > PlayerData.LastDailyDay
            && PlayerData.DailyId < GameData.DailyInfoDic.Count)
        {
            PopUIManager.Instance.ShowUI(PopUIEnum.daily);
            Daily.autoPop = true;
        }
        else
        {
            if ((DateTime.Now - PlayerData.LastAutoPopDailyTime).TotalHours >= 1)
            {
                PlayerData.LastAutoPopDailyTime = DateTime.Now;
                PopUIManager.Instance.ShowUI(PopUIEnum.offline);
            }
        }
        MainControl.Instance.Idle();

#if ADV2
        temp = new GameObject();
        temp.AddComponent<LoadingDelay>().StartCoroutine(Delay());
#endif
    }

    IEnumerator Delay()
    {

        #if UNITY_ANDROID

                yield return new WaitForSeconds(10f);

                AnalysisController.AfStatus = CrossAndroid.GetAF();

                Destroy(temp);

        #endif

        yield break;
    }

}
