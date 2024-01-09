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


    public const string GPID = "com.bingo.dream.lx";
    private const string FBUrl = "";
    public const string PPUrl = "https://docs.google.com/document/d/e/2PACX-1vRWCPa1u0EF3YeMDKm6SBiu7urWllGzKW2GNFqk_HRnDEY6iGVl_XFDtfQ0iRggUHiVspnCgFGTY1gy/pub";
    public const string TOSURl = "https://docs.google.com/document/d/e/2PACX-1vSrALcRCwWtlFxFnQHUflePbGGuKCh3_QesDwg7xtZDNKOBBr-fcaYY5TKAU37PMxEZP4aOK8a6HncP/pub";
    //IOS


    public const string IOSID = "1541317383";
    private const string FBUrlForIOS = "";

    public const string PPUrlForIOS = "https://docs.google.com/document/d/e/2PACX-1vQyL92654KhL4REOq22YCroOZh87I8DGVLXxiKwMiBHUxAee2bjDEl5ERcD37aFgOVwA-aK9C9b-bzv/pub";
    public const string TOSURlForIOS = "https://docs.google.com/document/d/e/2PACX-1vQRDWWm6h6pylrnSenGYfgFAtWjF9fW3ln0xWQYHQH88oJ6hnoiFkZew5tG1TlXErbFGlRuFI6HE5gA/pub";


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



        if (PP) PP.AddListener(openPP);
        if (TOS) TOS.AddListener(openTOS);
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
        Application.OpenURL(PPUrlForIOS);
        return;
#endif
        Application.OpenURL(PPUrl);
    }

    public static void openTOS()
    {
        SoundController.Instance.PlaySound(SoundType.tanchuang);
#if UNITY_IPHONE
        Application.OpenURL(TOSURlForIOS);
        return;
#endif
        Application.OpenURL(TOSURl);
    }
}