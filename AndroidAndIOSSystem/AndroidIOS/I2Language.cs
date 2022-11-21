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
                if (usFolat) _m = (i * 5).ToString("0.0");
                else _m = (i * 5).ToString("0");
                break;
            case LanguageEnum.ID:
                _i = Mathf.RoundToInt(i * 150);
                _m = (_i * 100).ToString();
                break;
            case LanguageEnum.PH:
                if (usFolat) _m = (i * 50f).ToString("0.0");
                else _m = (i * 50f).ToString("0");
                break;
            case LanguageEnum.MX:
                if (usFolat) _m = (i * 20f).ToString("0.0");
                else _m = (i * 20f).ToString("0");
                break;
            case LanguageEnum.DE:
                if (usFolat) _m = i.ToString("0.00");
                else _m = i.ToString("0");
                break;
            case LanguageEnum.FR:
                if (usFolat) _m = i.ToString("0.00");
                else _m = i.ToString("0");
                break;
            case LanguageEnum.EG:
                if (usFolat) _m = (i * 15).ToString("0.0");
                else _m = (i * 15).ToString("0");
                break;
            case LanguageEnum.TR:
                if (usFolat) _m = (i * 15).ToString("0.0");
                else _m = (i * 15).ToString("0");
                break;
            case LanguageEnum.TH:
                if (usFolat) _m = (i * 35f).ToString("0.0");
                else _m = (i * 35f).ToString("0");
                break;
            case LanguageEnum.VN:
                _i = Mathf.RoundToInt(i * 250);
                _m = (_i * 100).ToString();
                break;
            case LanguageEnum.IN:
                _i = Mathf.RoundToInt(i * 100);
                _m = (_i).ToString();
                break;
            case LanguageEnum.PK:
                _i = Mathf.RoundToInt(i * 200);
                _m = (_i).ToString();
                break;
            case LanguageEnum.BD:
                _i = Mathf.RoundToInt(i * 100);
                _m = (_i).ToString();
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
            case LanguageEnum.VN:
                _m = "₫";
                break;
            case LanguageEnum.TH:
                _m = "฿";
                break;
            case LanguageEnum.IN:
                _m = "₹";
                break;
            case LanguageEnum.PK:
                _m = "Rs";
                break;
            case LanguageEnum.BD:
                _m = "৳";
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
