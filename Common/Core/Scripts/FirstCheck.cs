using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;

public static class FirstCheck
{
    public static bool GetIsDayFirst(string key)
    {
        StringData data = new StringData(key,"");

        if (data.Value == "")
        {
            data.Value = new DateTime(1949,01,01).ToString(CultureInfo.InvariantCulture);
        }

        var d1 = DateTime.Parse(data.Value, CultureInfo.InvariantCulture);
        var d2 = DateTime.Now;
        var diff = d2 - d1;

        if (diff.TotalDays >= 1)
        {
             data.Value = DateTime.Now.ToString(CultureInfo.InvariantCulture);
             return true;
        }
        else
        {
             return false;
        }
                
    }

    public static bool GetIsGameFirst(string key)
    {
        BoolData data = new BoolData(key, false);

        if (!data.Value)
        {
            data.Value = true;
            return true;
        }
        else
        {
            return false;
        }
    }
}
