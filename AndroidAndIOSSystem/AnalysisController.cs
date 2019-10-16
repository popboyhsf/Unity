using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventName
{
    private const string gameName = "XXX";
    public const string gameStart = gameName + "_game_start";
    public const string gameEnd = gameName + "_game_end";
    public const string levelUp = gameName + "_game_lv";

    public const string openLuckWallet = gameName + "_luck_wallet";
    public const string openLuckBox = gameName + "_luck_box";
    public const string gainLuckBalance = gameName + "_luck_balance";
}

public static class AnalysisController
{
    public enum AFStatus
    {
        Unknow = -1,
        Organic = 0,
        NonOrganic = 1,
    }
    public static UnityAction OnAFStatusChanged;
    public static UnityAction OffAFStatusChanged;
    private static AFStatus afStatus = AFStatus.Unknow;
    /// <summary>
    /// AF识别
    /// </summary>
    public static AFStatus AfStatus
    {
        get
        {
            return afStatus;
        }
        set
        {
            if (afStatus != value)
            {
                afStatus = value;
                OnAFStatusChanged?.Invoke();
            }
        }
    }

    private static bool mustNonOrganic;
    public static bool MustNonOrganic
    {
        get { return mustNonOrganic; }
        set
        {
            if (mustNonOrganic != value)
            {
                mustNonOrganic = value;
                OnAFStatusChanged?.Invoke();
            }
        }
    }

    /// <summary>
    /// 是否是买量用户,默认为自然
    /// </summary>
    public static bool IsNonOrganic
    {
        get
        {
            if (FackAF.isFackAF) return true;
            return MustNonOrganic || AfStatus == AFStatus.NonOrganic;
        }
    }


    public static void TraceEvent(string eventName, string jsonStr="")
    {
#if SafeMode
        return;
#elif UNITY_ANDROID && !UNITY_EDITOR
        CrossAndroid.LogEvent(eventName, jsonStr);
#elif UNITY_IPHONE && !UNITY_EDITOR
        CrossIos.LogEvent(eventName, jsonStr);
#endif
    }

    public static void TraceEvent(string eventName, Dictionary<string, string> dic)
    {
        TraceEvent(eventName, JsonMapper.ToJson(dic));
    }
}
