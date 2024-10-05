using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class About : MonoBehaviour
{

    [SerializeField]
    Button GP, FB, PP, TOS, cashPP;
    [SerializeField]
    Image GPImg;
    [SerializeField]
    Sprite GPS, APS;

    //Android


    public const string GPID = "TODO";
    private const string FBUrl = "";
    public const string PPUrl = "TODO";
    public const string TOSURl = "TODO";
    //IOS


    public const string IOSID = "TODO";
    private const string FBUrlForIOS = "";

    public const string PPUrlForIOS = "TODO";
    public const string TOSURlForIOS = "TODO";


    public const string GPUrl = "market://details?id=" + GPID;

    public const string GPUrlForIOS = "itms-apps://itunes.apple.com/app/id" + IOSID;


    protected void Awake()
    {

#if UNITY_ANDROID

        if (GPID == "")
        {
            if (GP) GP.gameObject.SetActive(false);
        }
        else
        {
            if (GP) GP.AddListener(openGP);
        }


        if (FBUrl == "")
        {
            if (FB) FB.gameObject.SetActive(false);

        }
        else
        {
            if (FB) FB.AddListener(openFB);
        }

#elif UNITY_IPHONE

        if (IOSID == "")
        {
            if (GP) GP.gameObject.SetActive(false);
        }
        else
        {
            if (GP) GP.AddListener(openGP);
        }

        if (FBUrlForIOS == "")
        {
            if (FB) FB.gameObject.SetActive(false);
        }
        else
        {
            if (FB) FB.AddListener(openFB);
        }

#endif



        if (PPUrl == "")
        {
            if (PP) PP.gameObject.SetActive(false);
        }
        else
        {
            if (PP) PP.AddListener(openPP);
        }

        if (TOSURl == "")
        {
            if (TOS) TOS.gameObject.SetActive(false);
        }
        else
        {
            if (TOS) TOS.AddListener(openTOS);
        }


        if (cashPP) cashPP.AddListener(openPP);


#if UNITY_ANDROID
        if(GPImg) GPImg.sprite = GPS;
#elif UNITY_IPHONE
        if(GPImg) GPImg.sprite = APS;
#endif
    }


    private void openGP()
    {
        SoundController.Instance.PlaySound(SoundType.tanchuang);
#if UNITY_IPHONE
        Application.OpenURL(GPUrlForIOS);
        return;
#endif
        Application.OpenURL(GPUrl);
    }

    private void openFB()
    {
        SoundController.Instance.PlaySound(SoundType.tanchuang);
        if (!string.IsNullOrEmpty(FBUrl))
        {

            Application.OpenURL(FBUrl);
        }
    }

    public static void openPP()
    {
        SoundController.Instance.PlaySound(SoundType.tanchuang);
#if UNITY_IPHONE
         CrossIos.IOSWebPageShow(PPUrlForIOS);
        return;
#endif
        Application.OpenURL(PPUrl);
    }

    public static void openTOS()
    {
        SoundController.Instance.PlaySound(SoundType.tanchuang);
#if UNITY_IPHONE
         CrossIos.IOSWebPageShow(TOSURlForIOS);
        return;
#endif
        Application.OpenURL(TOSURl);
    }
}