using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System;
using Random = System.Random;
using UnityEngine.UIElements;
using System.Text;
using Codice.CM.Common;

public class RandomCross
{

    // 读取文本文件的路径
    const string filePathU = "/Universal/Scripts/CrossPlatform/CrossIos.cs";
    const string filePathO = "/Editor/MessageFile.m";
    const string filePathMM = "/Editor/UnityAppController.mm";


    private static List<string> UTO = new List<string>()
    {
        " showInterstitial(",
        " showRewardBasedVideoParam(",
        " showRewardBasedVideo(",
        " isRewardVideoReady(",
        " rewardVideoCancel(",
        " LogEventIOS(",
        " hideLoadingRewardVideoWindow(",
        " startVibrator(",
        " gameStart(",
        " getAF(",
        " LogEvetnForTrackLuckBalance(",
        " showIDFA(",
        " canShowIDFA(",
        " requestIDFA(",
        " logEvetnForIDFA(",
        " iOSWebPageShow(",
        " iOSDeviceShock(",
        " rateUSShow(",
        " rateUS(",
        " iOSCanShowGDPR(",
        " showPrivacyOptionsForm(",
    };

    private static List<string> OTU = new List<string>()
    {
        "RewardVideoIsReadyCall",
        "RewardVideoIsReady",

        "HideLoadingRewardVideoWindow",
        "ReturnContry",
        "WatchRewardVideoComplete",
        "RewardVideoOpen",
        "ShowLoadingRewardVideoWindow",
        "InterstitialAdOpen",
        "InterstitialAdClose",
        "AppsFlyerState",
        "OnPrivacyOptionsFormShow",
        "IDFACallBack",
    };

    [MenuItem("Tools/YuanJi/混淆/UTO", false, 0)]
    public static void RandomCrossUTOFunction()
    {

        try
        {

            // 读取整个文件内容
            string fileContentU = File.ReadAllText(Application.dataPath + filePathU);
            string fileContentO = File.ReadAllText(Application.dataPath + filePathO);

            string newContentU = fileContentU;
            string newContentO = fileContentO;
            int _seed = 1;

            foreach (var item in UTO)
            {
                // 要查找和替换的字符串
                string searchString = item;
                _seed++;
                string replaceString = " " + GenerateRandomString(UnityEngine.Random.Range(8, 15), _seed) + "(";


                // 进行替换
                newContentU = newContentU.Replace(searchString, replaceString);

                newContentO = newContentO.Replace(searchString, replaceString);
            }

            // 将新内容写回文件
            File.WriteAllText(Application.dataPath + filePathU, newContentU);
            File.WriteAllText(Application.dataPath + filePathO, newContentO);

            Debug.Log("替换成功！");
        }
        catch (Exception ex)
        {
            Debug.LogError("发生错误：" + ex.Message);
        }
    }


    [MenuItem("Tools/YuanJi/混淆/OTU", false, 1)]
    public static void RandomCrossOTUFunction()
    {

        try
        {

            // 读取整个文件内容
            string fileContentU = File.ReadAllText(Application.dataPath + filePathU);
            string fileContentO = File.ReadAllText(Application.dataPath + filePathO);
            string fileContentMM = File.ReadAllText(Application.dataPath + filePathMM);

            string newContentU = fileContentU;
            string newContentO = fileContentO;
            string newContentMM = fileContentMM;
            int _seed = 1000;

            foreach (var item in OTU)
            {
                // 要查找和替换的字符串
                string searchString = item;
                _seed++;
                string replaceString = GenerateRandomString(UnityEngine.Random.Range(6, 20), _seed);


                // 进行替换
                newContentU = newContentU.Replace(" " + searchString + "(", " " + replaceString + "(");

                newContentO = newContentO.Replace(searchString, replaceString);
                newContentMM = newContentMM.Replace(searchString, replaceString);
            }

            // 将新内容写回文件
            File.WriteAllText(Application.dataPath + filePathU, newContentU);
            File.WriteAllText(Application.dataPath + filePathO, newContentO);
            File.WriteAllText(Application.dataPath + filePathMM, newContentMM);

            Debug.Log("替换成功！");
        }
        catch (Exception ex)
        {
            Debug.LogError("发生错误：" + ex.Message);
        }
    }

    private static string GenerateRandomString(int length, int seed)
    {
        const string chars1 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; // 不包含数字开头的字符集合
        const string chars2 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"; // 不包含数字开头的字符集合
        Random random = new Random(seed);
        StringBuilder sb = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            if (i == 0) sb.Append(chars1[random.Next(chars1.Length)]);
            else sb.Append(chars2[random.Next(chars2.Length)]);
        }

        return sb.ToString();
    }
}
