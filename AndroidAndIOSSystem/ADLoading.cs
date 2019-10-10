using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ADLoading : MonoBehaviour
{
    [SerializeField]
    GameObject loadingObj;
    [SerializeField]
    Button closs;

    private static ADLoading _instance;

    public static ADLoading Instance { get => _instance; }

    private void Awake()
    {
        _instance = this;
        closs.onClick.AddListener(HieednLoadingPrivata);
    }


    public void ShowLoading()
    {
#if SafeMode
        return;
#elif UNITY_ANDROID && !UNITY_EDITOR || UNITY_IPHONE && !UNITY_EDITOR
        loadingObj.SetActive(true);
#endif

    }

    private void HieednLoadingPrivata()
    {
        AdController.CancelShowRewardedVideo();
    }

    public void HiddenLoading()
    {
        loadingObj.SetActive(false);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause && loadingObj.activeInHierarchy)
        {
            AdController.CancelShowRewardedVideo();
        }
    }
}
