using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class SevenDayData 
{
    public string count;
    public int num;

}

[Serializable]
public struct SevenDayIcon
{
    public SevenDaysData.flopEnum flopEnum;
    public Sprite sprite;

    public SevenDayIcon(SevenDaysData.flopEnum flopEnum, Sprite sprite)
    {
        this.flopEnum = flopEnum;
        this.sprite = sprite;
    }
}


public static class SevenDaysData
{
    private const string flag = "SevenDaysData";

    private const string timeFlag = "_Time";

    private const string countFlag = "_Count";

    private const string showFlag = "_show";

    public enum flopEnum
    {
        Gift = 0,


        Unkonw = 99,
    }

    public static List<SevenDayData> SevenDayDataL { get; private set; } = new List<SevenDayData>();

    static SevenDaysData()
    {
        Load();
    }

    private static void Load()
    {
        SevenDayDataL = JsonStructureLoader.Load<SevenDayData>();
    }

  
    /// <summary>
    /// 获得时间
    /// </summary>
    /// <returns>返回天数</returns>
    public static int GetTime()
    {
        if (PlayerPrefs.GetInt(flag + timeFlag, -1) == -1)
            return 1;
        var i = (int)(NetWorkTimerManager.Instance.dateTime - new DateTime(1970, 01, 01)).TotalDays;
        return Mathf.Max(i - PlayerPrefs.GetInt(flag + timeFlag, i) , 0);
    }

    /// <summary>
    /// 设置时间
    /// </summary>
    public static void SetTime()
    {
        var i = (int)(NetWorkTimerManager.Instance.dateTime - new DateTime(1970, 01, 01)).TotalDays;
        PlayerPrefs.SetInt(flag + timeFlag, i);
    }

    /// <summary>
    /// 获得次数
    /// </summary>
    /// <returns>返回第几次签到</returns>
    public static int GetCount()
    {
        return PlayerPrefs.GetInt(flag + countFlag, 0) % 7;
    }

    /// <summary>
    /// 获得轮回次数
    /// </summary>
    /// <returns>返回第几次签到</returns>
    public static int GetLunCount()
    {
        return 1;
        return (PlayerPrefs.GetInt(flag + countFlag, 0) / 7 + 1);
    }

    /// <summary>
    /// 设置次数
    /// </summary>
    public static void SetCoun()
    {
        var i = PlayerPrefs.GetInt(flag + countFlag, 0);
        PlayerPrefs.SetInt(flag + countFlag, i + 1);
    }

    /// <summary>
    /// 获得奖励数量
    /// </summary>
    /// <returns></returns>
    public static int GetNum(int index)
    {

        return SevenDayDataL[index].num * GetLunCount();
    }

    /// <summary>
    /// 簽到界面能否出現
    /// </summary>
    /// <returns></returns>
    public static bool CanShow()
    {
        return PlayerPrefs.GetInt(flag + showFlag, 0).IntToBool();
    }

    /// <summary>
    /// 設置簽到界面能否出現
    /// </summary>
    /// <param name="show"></param>
    public static void SetShow(bool show)
    {
        PlayerPrefs.SetInt(flag + showFlag, show.BoolToInt());
    }
}
