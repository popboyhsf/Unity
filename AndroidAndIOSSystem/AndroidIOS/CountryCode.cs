using UnityEngine;

public static class CountryCode
{
    // Java class path
    private const string ANDROID_CLASS = "com.unity3d.player.LocaleUtils";

    // Android method to get country code
    private const string ANDROID_METHOD_COUNTRY = "getCountryCode";

    // Android method to get language code
    private const string ANDROID_METHOD_LANGUAGE = "getLanguageCode";

    // Function to retrieve country code
    public static string GetCountryCode()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaClass javaClass = new AndroidJavaClass(ANDROID_CLASS))
            {
                AndroidJavaObject context = GetContext();
                if (context != null)
                {
                    return javaClass.CallStatic<string>(ANDROID_METHOD_COUNTRY, context);
                }
            }
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {

        }
        return "";
    }

    // Function to retrieve language code
    public static string GetLanguageCode()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaClass javaClass = new AndroidJavaClass(ANDROID_CLASS))
            {
                return javaClass.CallStatic<string>(ANDROID_METHOD_LANGUAGE);
            }
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {

        }
        return "";
    }

    private static AndroidJavaObject GetContext()
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        return currentActivity;
    }
}
