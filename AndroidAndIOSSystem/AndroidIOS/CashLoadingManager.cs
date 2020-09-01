using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CashLoadingManager : MonoBehaviour
{
    private static CashLoadingManager _instance;

    public static CashLoadingManager Instance { get => _instance; }

    public GameObject adLoading;
    public GameObject adLoadingBack;
    public GameObject adFail;

    private UnityAction watchCompletedAction;

    private float timer = 0;

    private void Awake()
    {
        _instance = this;
    }

    public void Show()
    {
        watchCompletedAction = null;
        adLoading.SetActive(true);
        adLoadingBack.SetActive(false);
        this.StartCoroutine(startTimer());
    }
    public void Showing()
    {
        this.StopAllCoroutines();
    }
    public void ShowBack()
    {
        adLoading.SetActive(false);
        adLoadingBack.SetActive(true);
        this.StopAllCoroutines();
        this.StartCoroutine(endTimer());
    }
    public void ShowBack(UnityAction watchCompletedAction)
    {
        this.watchCompletedAction = watchCompletedAction;
        adLoading.SetActive(false);
        adLoadingBack.SetActive(true);
        this.StopAllCoroutines();
        this.StartCoroutine(endTimer());
    }

    IEnumerator startTimer()
    {
        timer = 0;

        while (adLoading.activeSelf)
        {
            if (timer >= 13f)
            {
                adLoading.SetActive(false);
                adFail.SetActive(true);
                AdController.CancelShowRewardedVideo();
            }
            else
            {
                timer += Time.deltaTime;
            }
            yield return null;
        }
        yield return new WaitForSecondsRealtime(2f);
        adFail.SetActive(false);
        yield break;
    }

    IEnumerator endTimer()
    {
        timer = 0;

        while (adLoadingBack.activeSelf)
        {
            if (timer >= 1.5f)
            {
                adLoadingBack.SetActive(false);
                this.watchCompletedAction?.Invoke();
            }
            else
            {
                timer += Time.deltaTime;
            }
            yield return null;
        }

        yield break;
    }
}
