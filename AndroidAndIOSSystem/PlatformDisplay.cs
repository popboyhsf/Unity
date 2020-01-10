using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDisplay : MonoBehaviour
{
    public enum Platform
    {
        Android,
        IOS
    }
    [SerializeField]
    Platform platform;
    void Start()
    {
#if UNITY_ANDROID
        if(platform!= Platform.Android)
        {
            gameObject.SetActive(false);
        }
#endif
#if UNITY_IPHONE
        if(platform!= Platform.IOS)
        {
            gameObject.SetActive(false);
        }
#endif
    }

}


