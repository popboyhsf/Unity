using System;
using UnityEngine;

public static class CrossAndroidUtils
{
    private const string emailTarget = @"starland0317@gmail.com";

    private static string emailTitle = "";

    private static StringData userID = new StringData("CrossAndroidUtils_userID","");


    public static void PostEmail()
    {
        InitUserID();
        if (emailTitle == "")
            emailTitle = Application.productName + "(" + Application.version + ")";
#if UNITY_EDITOR || SafeMode



#elif UNITY_ANDROID

        CrossAndroid.PostEmail(emailTarget, emailTitle, Application.productName, userID.Value, Application.identifier, Application.version);
        
#elif UNITY_IPHONE
        
#endif
    }

    private static void InitUserID()
    {

        if (userID.Value == "")
        {
            userID.Value = GetRandomString(6,false,true,true,false,"");
        }
    }

    ///<summary>
    ///生成随机字符串 
    ///</summary>
    ///<param name="length">目标字符串的长度</param>
    ///<param name="useNum">是否包含数字，1=包含，默认为包含</param>
    ///<param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
    ///<param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
    ///<param name="useSpe">是否包含特殊字符，1=包含，默认为不包含</param>
    ///<param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
    ///<returns>指定长度的随机字符串</returns>
    public static string GetRandomString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
    {
        byte[] b = new byte[4];
        new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
        System.Random r = new System.Random(BitConverter.ToInt32(b, 0));
        string s = null, str = custom;
        if (useNum == true) { str += "0123456789"; }
        if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
        if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
        if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
        for (int i = 0; i < length; i++)
        {
            s += str.Substring(r.Next(0, str.Length - 1), 1);
        }
        return s;
    }


}
