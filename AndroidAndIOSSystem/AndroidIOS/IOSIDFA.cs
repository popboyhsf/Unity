using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IOSIDFA : SingletonMonoBehaviour<IOSIDFA>
{
    private UnityAction backCall;


    /// <summary>
    /// 展示IDFA
    /// </summary>
    /// <param name="enterCall"></param>
    /// <param name="backCall"></param>
    public void ShowIDFA(UnityAction enterCall, UnityAction backCall)
    {
        this.backCall = backCall;


#if UNITY_EDITOR || SafeMode
        enterCall?.Invoke();
#elif UNITY_ANDROID && !UNITY_EDITOR
        
#elif UNITY_IPHONE && !UNITY_EDITOR

        if (CrossIos.CanShowIDFA())
        {
            enterCall?.Invoke();
        }
        else
        {
            this.backCall?.Invoke();
        }
#endif
    }

    /// <summary>
    /// 點擊Unity界面的Allow
    /// </summary>
    public void ClickAllow()
    {

#if UNITY_EDITOR || SafeMode
        this.backCall?.Invoke();
        Debuger.Log("現在你在Unity編譯環境/SafeMode，假裝點擊了按鈕");
#elif UNITY_ANDROID && !UNITY_EDITOR
        
#elif UNITY_IPHONE && !UNITY_EDITOR

        CrossIos.ShowIDFA(this.backCall);
#endif

    }

    /// <summary>
    /// 在Unity界面展示的同時統計
    /// </summary>
    public void LogEvetnForIDFA()
    {
#if UNITY_EDITOR || SafeMode
        Debuger.Log("現在你在Unity編譯環境/SafeMode，假裝統計了IDFA事件");
#elif UNITY_ANDROID && !UNITY_EDITOR
        
#elif UNITY_IPHONE && !UNITY_EDITOR

        if (FirstCheck.GetIsGameFirst("IOSIDFA_LogIDFA"))
        {
            CrossIos.LogEvetnForIDFA();
        }
#endif
    }

}
