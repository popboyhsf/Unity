using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;
using System;

public class I2Language : MonoBehaviour
{

    private static I2Language _instance;

    public static I2Language Instance
    {
        get
        {
            return _instance;
        }
    }
    public enum LanguageEnum
    {
        EN = 0,
        JP = 1,
        KR = 2,
        RU = 3,
        BR = 4,
        ID = 5,
        PH = 6,
        TH = 7,
        VN = 8,
        MX = 9,
    }

    public LanguageEnum Language { private set; get; } = LanguageEnum.EN;

    private void Awake()
    {
        _instance = this;
    }

    public void ApplyLanguage(LanguageEnum language)
    {
        Language = language;


#if UNITY_EDITOR

        var _l = LocalizationManager.GetAllLanguages()[(int)language];

        if (LocalizationManager.HasLanguage(_l))
        {
            LocalizationManager.CurrentLanguage = _l;
        }


#endif

        //GiftCardManager.Instance.Refresh();


#if !UNITY_EDITOR


        if (AnalysisController.IsNonOrganic)
        {
            //SevenDaysManager.Instance.Show();
        }
        else
        {
            AnalysisController.OnAFStatusChanged += () => {

                //SevenDaysManager.Instance.Show();

            };
        }

#endif

    }

    public void ChangeUI(LanguageEnum language)
    {
        var _l = LocalizationManager.GetAllLanguages()[(int)language];

        if (LocalizationManager.HasLanguage(_l))
        {
            LocalizationManager.CurrentLanguage = _l;
        }
    }

    public string ChangeMoney(float i, bool usFolat = true)
    {
        string _m = "";

        if (usFolat) _m = i.ToString("0.00");
        else _m = i.ToString("0");


        switch (Language)
        {
            case LanguageEnum.EN:
                if (usFolat) _m = i.ToString("0.00");
                else _m = i.ToString("0");
                break;
            case LanguageEnum.JP:
                var _i = Mathf.RoundToInt(i * 100);
                _m = (_i).ToString();
                break;
            case LanguageEnum.KR:
                _i = Mathf.RoundToInt(i * 100);
                _m = (_i * 10).ToString();
                break;
            case LanguageEnum.RU:
                _i = Mathf.RoundToInt(i * 100);
                _m = (_i).ToString();
                break;
            case LanguageEnum.BR:
                var _ii = i * 5f;
                _m = (_ii).ToString("0.00");
                break;
            case LanguageEnum.ID:
                _i = Mathf.RoundToInt(i * 100);
                _m = (_i * 150).ToString();
                break;
            case LanguageEnum.PH:
                var _iii = i * 50f;
                _m = (_iii).ToString("0.00");
                break;
            case LanguageEnum.TH:
                _iii = i * 30f;
                _m = (_iii).ToString("0.00");
                break;
            case LanguageEnum.VN:
                _i = Mathf.RoundToInt(i * 200);
                _m = (_i * 100).ToString();
                break;
            case LanguageEnum.MX:
                _ii = i * 20f;
                _m = (_ii).ToString("0.00");
                break;
            default:
                break;
        }

        return _m;
    }

    public string ChangeMoneyIcon()
    {
        string _m = "";

        _m = "$";

        switch (Language)
        {
            case LanguageEnum.EN:
                _m = "$";
                break;
            case LanguageEnum.JP:
                _m = "¥";
                break;
            case LanguageEnum.KR:
                _m = "₩";
                break;
            case LanguageEnum.RU:
                _m = "^";
                break;
            case LanguageEnum.BR:
                _m = @"R$";
                break;
            case LanguageEnum.ID:
                _m = "RP"; 
                break;
            case LanguageEnum.PH:
                _m = "PHP";
                break;
            case LanguageEnum.TH:
                _m = "[";
                break;
            case LanguageEnum.VN:
                _m = "]";
                break;
            case LanguageEnum.MX:
                _m = "MXN";
                break;
            default:
                break;
        }

        return _m;
    }

    public bool IsInI2(string _contry)
    {
        var _b = true;

        try
        {
            LanguageEnum _language = (LanguageEnum)Enum.Parse(typeof(LanguageEnum), _contry);
            
        }
        catch (Exception)
        {
            _b = false;
        }

        return _b;
    }
}
