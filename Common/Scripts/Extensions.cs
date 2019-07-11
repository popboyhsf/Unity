using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static string ToThroundString(this int num)
    {
        if (num > 1000)
        {
            float value = num / 1000f;
            if (num % 1000 == 0)
            {
                return (num / 1000) + "k";
            }
            else
            {
                return value.ToString("f1") + "k";
            }
        }
        else
        {
            return num.ToString();
        }
    }

    public static string ToCashString(this int num)
    {
        return "$" + num / 100 + "." + (num % 100).ToString("D2");
    }

    public static string ToThroundString(this ulong num)
    {
        if (num > 1000)
        {
            float value = num / 1000f;
            if (num % 1000 == 0)
            {
                return (num / 1000) + "k";
            }
            else
            {
                return value.ToString("f1") + "k";
            }
        }
        else
        {
            return num.ToString();
        }
    }
}
