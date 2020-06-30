using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
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
        if(max - min +1 < returnCount)
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

    public static int RandomOne()
    {
        return Random(0, 1) > 0 ? 1 : -1;
    }

    private static string AESKey = "fjkasdjJdA4178A0";

    ///  AES 加密
    /// </summary>
    /// <param name="str">明文（待加密）</param>
    /// <returns></returns>
    public static string AESEncrypt(string str)
    {
        if (string.IsNullOrEmpty(str)) return null;
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

}
