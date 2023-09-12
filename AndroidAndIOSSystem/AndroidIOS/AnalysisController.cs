using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public enum VideoEventName
{
    GiftCard_ClickBox,
    DoubleBonus,
    UseOrGetItem,
    WheelStart,
}
public static class EventName
{
#if LuckyBlock
    private const string gameName = "luckyblock_wx_";
#elif MusicIOMode
    private const string gameName = "musicio_wx_";
#else
    private const string gameName = "blockpuzzle_wx_";
#endif

    public const string gameStart = gameName + "game_start";
    public const string gameEnd = gameName + "game_end";

    #region 游戏界面
    public const string screen_splash = gameName + "screen_splash";
    public const string screen_home = gameName + "screen_home";
    public const string screen_luck = gameName + "screen_luck";
    public const string screen_game = gameName + "screen_game";
    public const string screen_shop = gameName + "screen_shop";
    public const string screen_luckywheel = gameName + "screen_luckywheel";
    public const string screen_gameend = gameName + "screen_gameend";
    public const string screen_pkmode = gameName + "screen_pkmode";

    #endregion

    #region 参与游戏
    public const string game_playtime = gameName + "game_playtime";
    public const string game_start = gameName + "game_start";
    public const string game_win = gameName + "game_win";
    public const string game_fail = gameName + "game_fail";
    public const string game_retry = gameName + "game_retry";
    public const string energy_empty = gameName + "energy_empty";



    public static string mode_name = "";
    public static string mode_start = gameName + "mode_" + mode_name + "_start";
    public static string mode_win = gameName + "mode_" + mode_name + "_win";
    public static string mode_fail = gameName + "mode_" + mode_name + "_fail";
    public static string mode_retry = gameName + "mode_" + mode_name + "_retry";
    


    /// <summary>
    /// 累计胜利次数
    /// </summary>
    /// <returns></returns>
    public static string Game_Win()
    {
        var i = PlayerPrefs.GetInt(gameName+ "game_win_",0);
        i++;
        PlayerPrefs.SetInt(gameName + "game_win_", i);

        if (i >= 5 && i < 7) i = 5;
        if (i >= 7 && i < 10) i = 7;
        if (i >= 10 && i < 15) i = 10;
        if (i >= 15 && i < 25) i = 15;
        if (i >= 25 && i < 40) i = 25;
        if (i >= 40 && i < 65) i = 40;
        if (i >= 65 && i < 100) i = 65;
        if (i >= 100) i = 100;

        return gameName + "game_win_" + i;
    }



    /// <summary>
    /// 累计金条金额
    /// </summary>
    /// <returns></returns>
    public static string Gold()
    {
        var i = GoldData.goldNum.Value;

        if (i >= 1 && i < 100) i = 1;
        if (i >= 100 && i < 500) i = 100;
        if (i >= 500 && i < 1000) i = 500;
        if (i >= 1000 && i < 5000) i = 1000;
        if (i >= 5000 && i < 10000) i = 5000;
        if (i >= 10000 && i < 50000) i = 10000;
        if (i >= 50000 && i < 100000) i = 50000;
        if (i >= 100000) i = 100000;

        return gameName + "gold_" + i;
    }

    /// <summary>
    /// 使用道具
    /// </summary>
    /// <param name="tip"></param>
    /// <returns></returns>
    public static string Use_Item(ItemSystemData.ItemEnum tip)
    {
        switch (tip)
        {
            case ItemSystemData.ItemEnum.chuizi:
                return gameName + "use_item_hammer";
            case ItemSystemData.ItemEnum.chongzhi:
                return gameName + "use_item_roll";
            case ItemSystemData.ItemEnum.shandian:
                return gameName + "use_item_bliz";
            case ItemSystemData.ItemEnum.dachuizi:
                return gameName + "use_item_bomb";
            default:
                return gameName + "use_item_default";
        }
    }




    public const string pk_lose = gameName + "pk_lose";

    /// <summary>
    /// 累计PK胜利次数
    /// </summary>
    /// <returns></returns>
    public static string Game_Win_PK()
    {
        var i = PlayerPrefs.GetInt(gameName + "pk_win_", 0);
        i++;
        PlayerPrefs.SetInt(gameName + "pk_win_", i);

        if (i >= 5 && i < 5) i = 5;
        if (i >= 7 && i < 10) i = 7;
        if (i >= 10 && i < 15) i = 10;
        if (i >= 15 && i < 25) i = 15;
        if (i >= 25 && i < 40) i = 25;
        if (i >= 40 && i < 65) i = 40;
        if (i >= 65 && i < 100) i = 65;
        if (i >= 100) i = 100;

        return gameName + "pk_win_" + i;
    }



    #endregion

    #region 广告

    public const string video_reward_paypal_game = gameName + "video_reward_paypal_game";
    public const string video_reward_paypal_day = gameName + "video_reward_paypal_day";
    public const string video_reward_paypal_click = gameName + "video_reward_paypal_click";

    public const string video_reward_item = gameName + "video_reward_item";
    public static string Video_Item(ItemSystemData.ItemEnum tip)
    {
        switch (tip)
        {
            case ItemSystemData.ItemEnum.chuizi:
                return gameName + "video_reward_item_hammer";
            case ItemSystemData.ItemEnum.chongzhi:
                return gameName + "video_reward_item_roll";
            case ItemSystemData.ItemEnum.shandian:
                return gameName + "video_reward_item_bliz";
            case ItemSystemData.ItemEnum.dachuizi:
                return gameName + "video_reward_item_bomb";
            default:
                return gameName + "video_reward_item_default";
        }
    }




    public const string video_reward_luckywheel_time = gameName + "video_reward_luckywheel_time";

    #endregion

    #region 礼品卡相关

    public const string screen_paypal_claim = gameName + "screen_paypal_claim";
    public const string luck_freeday = gameName + "luck_freeday";
    public const string luck_game = gameName + "luck_game";
    public const string luck_clickbox = gameName + "luck_clickbox";
    public const string luck_pop = gameName + "luck_pop";
    public const string luck_day = gameName + "luck_day";
    public const string luck_star = gameName + "luck_star";
    public const string luck_online = gameName + "luck_online";
    public const string luck_egg = gameName + "luck_egg";
    public const string luck_agree_show = gameName + "luck_agree_show";
    public const string luck_agree_done = gameName + "luck_agree_done";
    public const string luck_paypal_show = gameName + "luck_paypal_show";
    public const string luck_click_card = gameName + "luck_click_card";
    public const string luck_miss = gameName + "luck_miss";
    public const string luck_ready = gameName + "luck_ready";
    public const string luck_phone = gameName + "luck_phone";
    public const string luck_cashout = gameName + "luck_cashout";
    public const string luck_cashout_fromUI = gameName + "luck_cashout_fromUI";
    public const string luck_cashout_fromList = gameName + "luck_cashout_fromList";


    #endregion
}

[CheatableAttribute("ANA")]
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
    [CheatableAttribute("是否是强制买量1是0否-1不启用")]
    public static IntData fixOrganic { get; set; } = new IntData("AnalysisController_fixOrganic", -1);


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
                Debuger.Log("DoOnAFStatusChanged == " + afStatus);
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
    /// 是否是买量用户,默认为自然 true是买量
    /// </summary>
    public static bool IsNonOrganic
    {
        get
        {
#if UNITY_EDITOR
            if (FackAF.isFackAF) return true;
#endif
            if (fixOrganic.Value > 0) return fixOrganic.Value.IntToBool();
            return MustNonOrganic || (AfStatus == AFStatus.NonOrganic);
        }
    }

    public static void TraceEvent(string eventName, string jsonStr="")
    {
        Debuger.LogWarning("AF ====== " + eventName + "    Content === " + jsonStr);

#if SafeMode || Marketing      
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

    public static void LogEvetnForTrackLuckBalance(float i, int j = 200)
    {
#if SafeMode || Marketing
        Debuger.LogWarning("AF ====== LogEvetnForTrackLuckBalance" +  "    Content === " + i + "   ===== " + j);
        return;
#elif UNITY_ANDROID && !UNITY_EDITOR
        CrossAndroid.LogEvetnForTrackLuckBalance(i,j);
#elif UNITY_IPHONE && !UNITY_EDITOR
        CrossIos.LogEvetnForTrackLuckBalance(i,j);
#endif
    }
}
