using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


#if !ADV2
public class ADLoading : MonoBehaviour,IIsViedoReady
{
    [SerializeField]
    GameObject loadingObj,failObj;
    //[SerializeField]
    //Button closs;

    private static ADLoading _instance;

    public static ADLoading Instance { get => _instance; }


    private float timer = 0;

    private void Awake()
    {
        _instance = this;
        //closs.onClick.AddListener(AdController.CancelRewardVideo);
        //closs.onClick.AddListener(HiddenLoading);
    }

    public void Open(bool isCash)
    {
        AdController.VideoIsReady(this, isCash);
    }

    public void ShowLoading()
    {
        loadingObj.SetActive(true);
    }
    public void HiddenLoading()
    {
        loadingObj.SetActive(false);
    }


    private void OnApplicationPause(bool pause)
    {
        if (pause && loadingObj.activeInHierarchy)
        {
            AdController.CancelRewardVideo();
        }
    }

    public void isReady(bool isReady)
    {
        loadingObj.SetActive(!isReady);

        if (!isReady) StartCoroutine(startTimer());
        else AdController.ShowRewardedVideoCallBack();
    }

    IEnumerator startTimer()
    {
        timer = 0;
        
        while (loadingObj.activeSelf)
        {
            if (timer >= 15f)
            {
                loadingObj.SetActive(false);
                failObj.SetActive(true);
                AdController.CancelRewardVideo();
            }
            else
            {
                timer += Time.deltaTime;
            }
            yield return null;
        }
        yield return new WaitForSecondsRealtime(2f);
        failObj.SetActive(false);
        yield break;
    }
}
#else
public class ADLoading : MonoBehaviour
{
    [SerializeField]
    GameObject loadingObj, failObj;


    private static ADLoading _instance;

    public static ADLoading Instance { get => _instance; }


    private float timer = 0;

    private void Awake()
    {
        _instance = this;
    }

    public void ShowLoading()
    {
        loadingObj.SetActive(true);
        StartCoroutine(startTimer());
    }
    public void HiddenLoading()
    {
        loadingObj.SetActive(false);
    }

    IEnumerator startTimer()
    {
        timer = 0;

        while (loadingObj.activeSelf)
        {
            if (timer >= 15f)
            {
                loadingObj.SetActive(false);
                failObj.SetActive(true);
                AdController.CancelShowRewardedVideo();
            }
            else
            {
                timer += Time.deltaTime;
            }
            yield return null;
        }
        yield return new WaitForSecondsRealtime(2f);
        failObj.SetActive(false);
        yield break;
    }
}
#endif