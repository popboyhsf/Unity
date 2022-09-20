using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
        EN,
        JP,
        KR,
        RU,
        BR,
        ID,
        PH,
        //TH,
        //VN,
        MX,
        DE,
        FR,
        EG,
        TR,
    }

    public LanguageEnum Language { private set; get; } = LanguageEnum.EN;

    public UnityAction onApplyLanguage { get; set; }
    public UnityAction OnChangeLanguage { get; set; }

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



#if !UNITY_EDITOR

        onApplyLanguage?.Invoke();
        if (!AnalysisController.IsNonOrganic)
        {
            AnalysisController.OnAFStatusChanged += () => {

                onApplyLanguage?.Invoke();

            };
        }

#endif
        OnChangeLanguage?.Invoke();
    }

    public void ChangeUI(LanguageEnum language)
    {
        var _l = LocalizationManager.GetAllLanguages()[(int)language];

        if (LocalizationManager.HasLanguage(_l))
        {
            LocalizationManager.CurrentLanguage = _l;
        }
        OnChangeLanguage?.Invoke();
    }

    public string ChangeMoney(float value, bool usFolat = true)
    {
        string _m = "";

        if (usFolat) _m = value.ToString("0.00");
        else _m = value.ToString("0");


        switch (Language)
        {
            case LanguageEnum.EN:
                if (usFolat) _m = value.ToString("0.00");
                else _m = value.ToString("0");
                break;
            case LanguageEnum.JP:
                var _i = Mathf.RoundToInt(value * 100);
                _m = (_i).ToString();
                break;
            case LanguageEnum.KR:
                _i = Mathf.RoundToInt(value * 100);
                _m = (_i * 10).ToString();
                break;
            case LanguageEnum.RU:
                _i = Mathf.RoundToInt(value * 100);
                _m = (_i).ToString();
                break;
            case LanguageEnum.BR:
                var _fi = value * 5;
                if (usFolat) _m = (_fi).ToString("0.0");
                else _m = _fi.ToString("0");
                break;
            case LanguageEnum.ID:
                _i = Mathf.RoundToInt(value * 100);
                _m = (_i * 150).ToString();
                break;
            case LanguageEnum.PH:
                _fi = value * 50f;
                _m = (_fi).ToString("0.0");
                break;
            case LanguageEnum.MX:
                _fi = value * 20f;
                _m = (_fi).ToString("0.0");
                break;
            case LanguageEnum.DE:
                if (usFolat) _m = value.ToString("0.00");
                else _m = value.ToString("0");
                break;
            case LanguageEnum.FR:
                if (usFolat) _m = value.ToString("0.00");
                else _m = value.ToString("0");
                break;
            case LanguageEnum.EG:
                _fi = value * 15f;
                _m = (_fi).ToString("0.0");
                break;
            case LanguageEnum.TR:
                _fi = value * 15f;
                _m = (_fi).ToString("0.0");
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
                _m = "₽";
                break;
            case LanguageEnum.BR:
                _m = @"R$";
                break;
            case LanguageEnum.ID:
                _m = "§";
                break;
            case LanguageEnum.PH:
                _m = "¤";
                break;
            case LanguageEnum.MX:
                _m = "^";
                break;
            case LanguageEnum.DE:
                _m = "€";
                break;
            case LanguageEnum.FR:
                _m = "€";
                break;
            case LanguageEnum.EG:
                _m = "→";
                break;
            case LanguageEnum.TR:
                _m = "←";
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
