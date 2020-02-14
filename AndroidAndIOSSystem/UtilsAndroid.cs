using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilsAndroid
{
    public enum IosType
    {
        Null = 0,
        SMSReceived_Vibrate = 1011,
        Vibrate = 4095,
    }

    //是否开启震动
    public static bool IsVibrator
    {
        get { return PlayerPrefs.GetInt("IsVibrator", 1) == 1; }
        set { PlayerPrefs.SetInt("IsVibrator", value ? 1 : 0); }
    }

    /// <summary>
    /// 震动一次
    /// </summary>
    /// <param name="delayTime"></param>
    /// <param name="duration"></param>
    public static void Vibrator(float delayTime, float duration,int type = 0)
    {
#if SafeMode
        return;
#endif
        if (!IsVibrator)
        {
            return;
        }
        long delayMillisecond = (long)(1000 * delayTime);
        long vibratorMillisecond = (long)(1000 * duration);
        long[] pattern = new long[2];
        pattern[0] = delayMillisecond;
        pattern[1] = vibratorMillisecond;
#if UNITY_ANDROID && !UNITY_EDITOR
        CrossAndroid.StartVibrator(pattern, -1);
#elif UNITY_IPHONE && !UNITY_EDITOR
        if(type != 0)
            CrossIos.Instance.StartVibrator(pattern, type);
#endif
    }
}
