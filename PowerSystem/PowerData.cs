using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PowerData
{
    private const string flag = "ESetPowerData_power";
    private const string flagTime = "ESetPowerData_time";
    private const string flagCount = "ESetPowerData_count";

    /// <summary>
    /// 体力
    /// </summary>
    public const int power = 5;
    /// <summary>
    /// 体力初始偏差
    /// </summary>
    public const int powerOffset = 5;
    /// <summary>
    /// 体力前几次恢复时间
    /// </summary>
    public const int powerRestore = 2700;
    /// <summary>
    /// 体力后几次恢复时间倍率 powerRestore * powerRestoreD
    /// </summary>
    public const int powerRestoreD = 1;
    /// <summary>
    /// 体力前后几次的几
    /// </summary>
    public const int powerRestoreLimit = -1;
    /// <summary>
    /// 每次消耗多少体力
    /// </summary>
    public const int powerOnce = 1;
    /// <summary>
    /// 看廣告給多少體力
    /// </summary>
    public const int powerAD = 2;

    public static int powerCheat = 0;

    /// <summary>
    /// 回复1体力所用时间
    /// </summary>
    public static int PowerRestore
    {
        get
        {
            var i = 1;
            if (GetCount > powerRestoreLimit) i = powerRestoreD;
            return powerRestore * i;
        }
    }

    public static int SetPower
    {
        set
        {
            var i = Mathf.Min(value, 99);
            PlayerPrefs.SetInt(flag, i);
        }
    }

    public static int GetPower
    {
        get
        {
            if (PlayerPrefs.GetInt(flag, -1) <= -1)
            {
                PlayerPrefs.SetInt(flag, power + powerOffset + powerCheat);
            }
            return PlayerPrefs.GetInt(flag, -1);
        }
    }


    public static string SetTimer
    {
        set
        {
            PlayerPrefs.SetString(flagTime,value);
        }

    }

    public static double GetTimer
    {
        get
        {
            if (PlayerPrefs.GetString(flagTime, "") == "")
            {
                var data = DateTime.Now;
                PlayerPrefs.SetString(flagTime, data.ToString());
                return 0;
            }
            else
            {
                var time = PlayerPrefs.GetString(flagTime, "");
                var d1 = DateTime.Parse(time);
                var d2 = DateTime.Now;
                var diff = d2 - d1;
                return diff.TotalSeconds;
            }
        }
    }

    public static int GetCount
    {
        get
        {
            return PlayerPrefs.GetInt(flagCount,0);
        }
    }

    public static int SetCount
    {
        set
        {
            PlayerPrefs.SetInt(flagCount,value);
        }
    }
}
