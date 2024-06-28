using System;
using System.Globalization;
using UnityEngine;

public static class FirstCheck
{
    public static bool GetIsDayFirst(string key, bool is24 = false)
    {
        StringData data = new StringData(key, "");

        if (data.Value == "")
        {
            data.Value = new DateTime(1949, 01, 01).ToString(CultureInfo.InvariantCulture);
        }

        var d1 = DateTime.Parse(data.Value, CultureInfo.InvariantCulture);
        var d2 = is24 ? DateTime.Now : DateTime.Today;
        var diff = d2 - d1;

        if (diff.TotalDays >= 1)
        {
            data.Value = d2.ToString(CultureInfo.InvariantCulture);
            return true;
        }
        else
        {
            return false;
        }

    }

    public static bool GetIsGameFirst(string key, bool writeOff = false)
    {
        BoolData data = new BoolData(key, false);

        if (!data.Value)
        {
            if (!writeOff)
                data.Value = true;

            return true;
        }
        else
        {
            return false;
        }
    }

    public static void GameFirstWriteOff(string key)
    {
        BoolData data = new BoolData(key, false);

        if (!data.Value)
        {
            data.Value = true;
        }
        else
        {
            Debuger.LogWarning($"{key} 已经核销过！！");
        }
    }
}
