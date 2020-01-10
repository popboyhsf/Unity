using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_IPHONE && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

public class MultiHaptic {

#if UNITY_IPHONE && !UNITY_EDITOR 

	[DllImport("__Internal")]
	static extern void _hapticMedium();

	[DllImport("__Internal")]
	static extern void _hapticLight();

	[DllImport("__Internal")]
	static extern void _hapticHeavy();

    [DllImport("__Internal")]
	static extern void _AudioServices(int type);
    
#endif

    public static void HapticLight() {
        #if UNITY_IPHONE && !UNITY_EDITOR
            _hapticLight ();
        #endif
	}

	public static void HapticMedium() {

        #if UNITY_IPHONE && !UNITY_EDITOR
            _hapticMedium();
        #endif
    }

    public static void HapticHeavy() {

        #if UNITY_IPHONE && !UNITY_EDITOR
            _hapticHeavy();
        #endif
    }

    public static void AudioServices(int type)
    {
#if UNITY_IPHONE && !UNITY_EDITOR
            _AudioServices(type);
#endif
    }
}