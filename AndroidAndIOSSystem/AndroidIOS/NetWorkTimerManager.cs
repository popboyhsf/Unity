using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

[CheatableAttribute("启用本地时间")]
public class NetWorkTimerManager : SingletonMonoBehaviour<NetWorkTimerManager>
{
    [SerializeField]
    GameObject loading, failed;
    [SerializeField]
    Button failedB;

    [CheatableAttribute("启用本地时间")]
    public static BoolData useLocalTime { get; private set; } = new BoolData("NetWorkTimerManager_useLocalTime",false);

    private string timer = "";

    public DateTime dateTime
    {
        get
        {
            try
            {
                return DateTime.Parse(timer);
            }
            catch (Exception)
            {
                Debuger.LogError("检测是否请求时间后操作的");
                return new DateTime(1949, 01, 01);
            }
        }
    }

    private List<UnityAction> action { get; set; } = new List<UnityAction>();

    protected override void Awake()
    {
        base.Awake();

        failedB.onClick.AddListener(RePostTimer);

        loading.SetActive(false);
        failed.SetActive(false);
    }

    public void PostTimer(UnityAction action)
    {
        this.action.Add(action);

        if (this.action.Count > 1) return;

        loading.SetActive(true);

        if (useLocalTime.Value)
        {
            StartCoroutine(fackWaitTimer());

            return;
        }
        else
        {
#if UNITY_EDITOR || SafeMode

            StartCoroutine(fackWaitTimer());

            return;



#elif UNITY_ANDROID

        CrossAndroid.PostTimer();
        
#elif UNITY_IPHONE
        CrossIos.PostTimer();
#endif
        }


    }

    public void RePostTimer()
    {
        loading.SetActive(true);
        failed.SetActive(false);

#if UNITY_EDITOR || SafeMode

        StartCoroutine(fackWaitTimer());
        return;
#elif UNITY_ANDROID

        CrossAndroid.PostTimer();
 
#elif UNITY_IPHONE
        CrossIos.PostTimer();
#endif
    }

    public void GetTimeFromAndroid(string s)
    {
        StartCoroutine(WaitTimer(s));
    }

    /// <summary>
    /// Android 回调
    /// </summary>
    /// <param name="s"></param>
    public void GetTimer(string s)
    {
        loading.SetActive(false);

        if (s == null)
        {
            failed.SetActive(true);
            AnalysisController.TraceEvent(EventName.screen_timelost);
        }
        else
        {
            timer = s;

            for (int i = 0; i < action.Count; i++)
            {
                action[i]?.Invoke();
            }

            action.Clear();
        }
    }

    private IEnumerator fackWaitTimer()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        GetTimer(DateTime.Now.ToString());

    }

    private IEnumerator WaitTimer(string s)
    {
        yield return new WaitForSecondsRealtime(0.2f);
        GetTimer(s);

    }
}
