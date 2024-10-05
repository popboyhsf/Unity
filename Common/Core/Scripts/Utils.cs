using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Globalization;
using UnityEngine.Events;
using System.Text.RegularExpressions;

/// <summary>
/// 工具类
/// </summary>
public static class Utils
{
    /// <summary>
    /// 获取指定范围内随机数
    /// </summary>
    /// <param name="num1"></param>
    /// <param name="num2"></param>
    /// <returns></returns>
    public static int Random(int num1, int num2)
    {
        if (num1 < num2)
        {
            return UnityEngine.Random.Range(num1, num2 + 1);
        }
        else
        {
            return UnityEngine.Random.Range(num2, num1 + 1);
        }
    }

    /// <summary>
    /// 获取不重复的随机数
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="returnCount"></param>
    /// <returns></returns>
    public static List<int> RandomUnduplicated(int min, int max, int returnCount)
    {
        if (max - min + 1 < returnCount)
        {
            Debug.LogError("UnduplicatedRandom error! num Count doesn't enought");
        }
        List<int> randomList = new List<int>();

        List<int> allSeed = new List<int>();
        for (int i = min; i <= max; i++)
        {
            allSeed.Add(i);
        }
        allSeed.Shuffle();

        for (int i = 0; i < returnCount; i++)
        {
            randomList.Add(allSeed[i]);
        }

        return randomList;
    }

    /// <summary>
    /// 转成日期格式
    /// </summary>
    /// <param name="self"></param>
    /// <param name="useFix">00:00:00多显示少不显示</param>
    /// <returns></returns>
    public static string floatToData(this float self, bool useFix = false)
    {
        TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(self));
        string str = "";
        if (useFix)
        {
            if (ts.Hours > 0)
            {
                str = ts.Hours.ToString("00") + ": " + ts.Minutes.ToString("00") + ": " + ts.Seconds.ToString("00");
            }
            if (ts.Hours == 0 && ts.Minutes > 0)
            {
                str = ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00");
            }
            if (ts.Hours == 0 && ts.Minutes == 0)
            {
                str = ts.Seconds.ToString("00");
            }
        }
        else
        {
            str = Math.Floor(ts.TotalMinutes).ToString("00") + ":" + ts.Seconds.ToString("00");
        }

        return str;
    }

    /// <summary>
    /// 转成日期格式
    /// </summary>
    /// <param name="self"></param>
    /// <param name="useFix">00:00:00多显示少不显示</param>
    /// <returns></returns>
    public static string doubleToData(this double self, bool useFix = false)
    {
        TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(self));
        string str = "";
        if (useFix)
        {
            if (ts.Hours > 0)
            {
                str = ts.Hours.ToString("00") + ": " + ts.Minutes.ToString("00") + ": " + ts.Seconds.ToString("00");
            }
            if (ts.Hours == 0 && ts.Minutes > 0)
            {
                str = ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00");
            }
            if (ts.Hours == 0 && ts.Minutes == 0)
            {
                str = ts.Seconds.ToString("00");
            }
        }
        else
        {
            str = Math.Floor(ts.TotalMinutes).ToString("00") + ":" + ts.Seconds.ToString("00");
        }
        return str;
    }

    public static int RandomOne()
    {
        return Random(0, 1) > 0 ? 1 : -1;
    }

    public static string ChangeDataToD(string strData)
    {
        decimal dData = 0.0M;
        if (strData.Contains("E"))
        {
            dData = decimal.Parse(strData, System.Globalization.NumberStyles.Float);
        }
        else
        {
            return strData;
        }
        return dData.ToString();
    }

    /// <summary>
    /// String转Float（国际化）
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static float StringToFloat(this string self)
    {
        try
        {
            return float.Parse(self, CultureInfo.InvariantCulture);
        }
        catch (System.FormatException)
        {

            Debuger.LogError("对于转换成Float出错 ：" + self);
            return 0;
        }
    }

    /// <summary>
    /// Float转String（国际化）
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static string FloatToString(this float self)
    {
        return self.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// String转Float数组
    /// </summary>
    /// <param name="self"></param>
    /// <param name="splitValue"></param>
    /// <returns></returns>
    public static List<float> ChangeToFloatList(this string self, char splitValue)
    {
        var _list = new List<float>();

        foreach (var item in self.Split(splitValue))
        {
            _list.Add(float.Parse(item, CultureInfo.InvariantCulture));
        }

        return _list;
    }
	
	/// <summary>
    /// 尝试将键和值添加到字典中：如果不存在，才添加；存在，不添加也不抛导常
    /// </summary>
    public static Dictionary<TKey, TValue> TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        if (dict.ContainsKey(key) == false) dict.Add(key, value);
        return dict;
    }
    /// <summary>
    /// 将键和值添加或替换到字典中：如果不存在，则添加；存在，则替换
    /// </summary>
    public static Dictionary<TKey, TValue> AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        dict[key] = value;
        return dict;
    }

    #region AES

    //private static string AESKey = GPID.Replace(".", "").Remove(16);
    private static string AESKey = "16-TODO";

    /// <summary>
    ///  AES 加密
    /// </summary>
    /// <param name="str">明文（待加密）</param>
    /// <returns></returns>
    public static string AESEncrypt(string str)
    {
        if (string.IsNullOrEmpty(str)) return null;

        str = Regex.Replace(str, "ı", "i", RegexOptions.CultureInvariant);

        Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);

        System.Security.Cryptography.RijndaelManaged rm = new System.Security.Cryptography.RijndaelManaged
        {
            Key = Encoding.UTF8.GetBytes(AESKey),
            Mode = System.Security.Cryptography.CipherMode.ECB,
            Padding = System.Security.Cryptography.PaddingMode.PKCS7
        };

        System.Security.Cryptography.ICryptoTransform cTransform = rm.CreateEncryptor();
        Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }

    /// <summary>
    ///  AES 加密
    /// </summary>
    /// <param name="str">明文（待加密）</param>
    /// <returns></returns>
    public static byte[] AESEncrypt(byte[] bit)
    {

        Byte[] toEncryptArray = bit;

        System.Security.Cryptography.RijndaelManaged rm = new System.Security.Cryptography.RijndaelManaged
        {
            Key = Encoding.UTF8.GetBytes(AESKey),
            Mode = System.Security.Cryptography.CipherMode.ECB,
            Padding = System.Security.Cryptography.PaddingMode.PKCS7
        };

        System.Security.Cryptography.ICryptoTransform cTransform = rm.CreateEncryptor();
        Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

        return resultArray;
    }

    /// <summary>
    ///  AES 解密
    /// </summary>
    /// <param name="str">明文（待解密）</param>
    /// <returns></returns>
    public static string AESDecrypt(string str)
    {
        if (string.IsNullOrEmpty(str)) return null;
        Byte[] toEncryptArray = Convert.FromBase64String(str);

        System.Security.Cryptography.RijndaelManaged rm = new System.Security.Cryptography.RijndaelManaged
        {
            Key = Encoding.UTF8.GetBytes(AESKey),
            Mode = System.Security.Cryptography.CipherMode.ECB,
            Padding = System.Security.Cryptography.PaddingMode.PKCS7
        };

        System.Security.Cryptography.ICryptoTransform cTransform = rm.CreateDecryptor();
        Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

        return Encoding.UTF8.GetString(resultArray);
    }

    /// <summary>
    ///  AES 解密
    /// </summary>
    /// <param name="str">明文（待解密）</param>
    /// <returns></returns>
    public static byte[] AESDecrypt(byte[] bit)
    {

        Byte[] toEncryptArray = bit;

        System.Security.Cryptography.RijndaelManaged rm = new System.Security.Cryptography.RijndaelManaged
        {
            Key = Encoding.UTF8.GetBytes(AESKey),
            Mode = System.Security.Cryptography.CipherMode.ECB,
            Padding = System.Security.Cryptography.PaddingMode.PKCS7
        };

        System.Security.Cryptography.ICryptoTransform cTransform = rm.CreateDecryptor();
        Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

        return resultArray;
    }

    #endregion

}
