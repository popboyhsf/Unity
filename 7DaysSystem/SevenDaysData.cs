using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SevenDaysData
{
    private const string flag = "SevenDaysData";

    private const string timeFlag = "_Time";

    private const string countFlag = "_Count";
  

    public static int GetTime()
    {
        var i = (int)(DateTime.Now - new DateTime(1970, 01, 01)).TotalHours;
        return Mathf.Max(i - PlayerPrefs.GetInt(flag + timeFlag, i) , 0);
    }

    public static void SetTime()
    {
        var i = (int)(DateTime.Now - new DateTime(1970, 01, 01)).TotalHours;
        PlayerPrefs.SetInt(flag + timeFlag, i);
    }

    public static int GetCount()
    {
        return PlayerPrefs.GetInt(flag + countFlag, 0);
    }

    public static void SetCoun()
    {
        PlayerPrefs.SetInt(flag + countFlag, GetCount()+1);
    }
}
