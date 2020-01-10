using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_ANDROID
        gameObject.AddComponent<CrossAndroid>();
#elif UNITY_IPHONE
        gameObject.AddComponent<CrossIos>();
#endif
    }
}


