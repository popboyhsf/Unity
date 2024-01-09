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
        EN,//en     美国
        JP,//ja     日本
        KR,//ko     韩国
        RU,//ru     俄罗斯
        BR,//pt-BR  巴西
        ID,//id     印尼
        PH,//en-PH  菲律宾
        MX,//es-MX  墨西哥
        TR,//tr     土耳其



        EG,//ar-EG  埃及
        DE,//de     德国
        FR,//fr     法国
        TH,//th     泰国
        VN,//vi     越南
        IN,//hi     印度
        PK,//ur     巴基斯坦
        BD,//bn     孟加拉
        ZA,//af     南非
        NG,//ng     尼日利亚

        CO,//es-CO  哥伦比亚
        AR,//es-AR  阿根廷
        SA,//ar-SA  沙特
        AE,//ar-AE  阿联酋


        ES,//es     西班牙
        IT,//it     意大利
        PL,//pl     波兰
        NL,//nl     荷兰
        RO,//ro     罗马尼亚
        SE,//sv-SE  瑞典
        GR,//el     希腊


        /*
         *231114
         *新增
         */


        AT,//de-AT  奥地利
        CH,//de-CH  瑞士
        BE,//nl-BE  比利时
        NO,//nb     挪威
        IE,//ga     爱尔兰
        DK,//da     丹麦
        FI,//fi     芬兰
        PT,//pt-PT  葡萄牙
        PE,//es-PE  秘鲁
        EC,//es-EC  厄瓜多尔
        MY,//ms-MY  马来西亚
        CL,//es-CL  智利
        CZ,//cs     捷克
        HU,//hu     匈牙利
    }

    public LanguageEnum Language { private set; get; } = LanguageEnum.EN;

    public UnityAction OnChangeLanguage { get; set; }
    private bool isGetLan = false;

    public bool IsGetLan
    {
        get => isGetLan;

    }

    private void Awake()
    {
        _instance = this;
    }

    public void ApplyLanguage(LanguageEnum language)
    {
        Language = language;
        isGetLan = true;

#if UNITY_EDITOR

        var _l = LocalizationManager.GetAllLanguages()[(int)language];

        if (LocalizationManager.HasLanguage(_l))
        {
            LocalizationManager.CurrentLanguage = _l;
        }


#endif

#if UNITY_IPHONE && !UNITY_EDITOR
        ChangeUI(Language);
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
                _m = (_i * 100).ToString("N0");
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
                _m = (_i * 100).ToString("N0");
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
            case LanguageEnum.ZA:
                _i = Mathf.RoundToInt(i * 20);
                _m = (_i).ToString();
                break;
            case LanguageEnum.NG:
                _i = Mathf.RoundToInt(i * 500);
                _m = (_i).ToString();
                break;
            case LanguageEnum.CO:
                _i = Mathf.RoundToInt(i * 5000);
                _m = (_i).ToString();
                break;
            case LanguageEnum.AR:
                _i = Mathf.RoundToInt(i * 200);
                _m = (_i).ToString();
                break;
            case LanguageEnum.SA:
                if (usFolat) _m = (i * 3).ToString("0.0");
                else _m = (i * 3).ToString("0");
                break;
            case LanguageEnum.AE:
                if (usFolat) _m = (i * 3).ToString("0.0");
                else _m = (i * 3).ToString("0");
                break;


            case LanguageEnum.ES:
                if (usFolat) _m = i.ToString("0.00");
                else _m = i.ToString("0");
                break;
            case LanguageEnum.IT:
                if (usFolat) _m = i.ToString("0.00");
                else _m = i.ToString("0");
                break;
            case LanguageEnum.PL:
                if (usFolat) _m = (i * 4).ToString("0.0");
                else _m = (i * 4).ToString("0");
                break;
            case LanguageEnum.NL:
                if (usFolat) _m = i.ToString("0.00");
                else _m = i.ToString("0");
                break;
            case LanguageEnum.RO:
                if (usFolat) _m = (i * 5).ToString("0.0");
                else _m = (i * 5).ToString("0");
                break;
            case LanguageEnum.SE:
                if (usFolat) _m = (i * 10).ToString("0.0");
                else _m = (i * 10).ToString("0");
                break;
            case LanguageEnum.GR:
                if (usFolat) _m = i.ToString("0.00");
                else _m = i.ToString("0");
                break;

            //231114新增
            case LanguageEnum.AT:
                if (usFolat) _m = i.ToString("0.00");
                else _m = i.ToString("0");
                break;
            case LanguageEnum.CH:
                if (usFolat) _m = i.ToString("0.00");
                else _m = i.ToString("0");
                break;
            case LanguageEnum.BE:
                if (usFolat) _m = i.ToString("0.00");
                else _m = i.ToString("0");
                break;
            case LanguageEnum.NO:
                if (usFolat) _m = (i * 10).ToString("0.00");
                else _m = (i * 10).ToString("0");
                break;
            case LanguageEnum.IE:
                if (usFolat) _m = i.ToString("0.00");
                else _m = i.ToString("0");
                break;
            case LanguageEnum.DK:
                if (usFolat) _m = (i * 7).ToString("0.00");
                else _m = (i * 7).ToString("0");
                break;
            case LanguageEnum.FI:
                if (usFolat) _m = i.ToString("0.00");
                else _m = i.ToString("0");
                break;
            case LanguageEnum.PT:
                if (usFolat) _m = i.ToString("0.00");
                else _m = i.ToString("0");
                break;
            case LanguageEnum.PE:
                if (usFolat) _m = (i * 4).ToString("0.00");
                else _m = (i * 4).ToString("0");
                break;
            case LanguageEnum.EC:
                if (usFolat) _m = i.ToString("0.00");
                else _m = i.ToString("0");
                break;
            case LanguageEnum.MY:
                if (usFolat) _m = (i * 5).ToString("0.00");
                else _m = (i * 5).ToString("0");
                break;
            case LanguageEnum.CL:
                _i = Mathf.RoundToInt(i * 900);
                _m = (_i).ToString();
                break;
            case LanguageEnum.CZ:
                if (usFolat) _m = (i * 25).ToString("0.00");
                else _m = (i * 25).ToString("0");
                break;
            case LanguageEnum.HU:
                if (usFolat) _m = (i * 350).ToString("0.00");
                else _m = (i * 350).ToString("0");
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
                _m = "Rp";
                break;
            case LanguageEnum.PH:
                _m = "₱";
                break;
            case LanguageEnum.MX:
                _m = "Mex$";//Mex$
                break;
            case LanguageEnum.DE:
                _m = "€";
                break;
            case LanguageEnum.FR:
                _m = "€";
                break;
            case LanguageEnum.EG:
                _m = "E£";
                break;
            case LanguageEnum.TR:
                _m = "₺";
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
            case LanguageEnum.ZA:
                _m = "R.";
                break;
            case LanguageEnum.NG:
                _m = "₦";
                break;
            case LanguageEnum.CO:
                _m = "$";
                break;
            case LanguageEnum.AR:
                _m = "$";
                break;
            case LanguageEnum.SA:
                _m = "﷼";
                break;
            case LanguageEnum.AE:
                _m = "د.إ";
                break;

            case LanguageEnum.ES:
                _m = "€";
                break;
            case LanguageEnum.IT:
                _m = "€";
                break;
            case LanguageEnum.PL:
                _m = "zł";
                break;
            case LanguageEnum.NL:
                _m = "€";
                break;
            case LanguageEnum.RO:
                _m = "L";
                break;
            case LanguageEnum.SE:
                _m = "kr";
                break;
            case LanguageEnum.GR:
                _m = "€";
                break;

            //231114新增
            case LanguageEnum.AT:
                _m = "€";
                break;
            case LanguageEnum.CH:
                _m = "CHF";
                break;
            case LanguageEnum.BE:
                _m = "€";
                break;
            case LanguageEnum.NO:
                _m = "kr";
                break;
            case LanguageEnum.IE:
                _m = "€";
                break;
            case LanguageEnum.DK:
                _m = "Kr";
                break;
            case LanguageEnum.FI:
                _m = "€";
                break;
            case LanguageEnum.PT:
                _m = "€";
                break;
            case LanguageEnum.PE:
                _m = "S/.";
                break;
            case LanguageEnum.EC:
                _m = "$";
                break;
            case LanguageEnum.MY:
                _m = "RM";
                break;
            case LanguageEnum.CL:
                _m = "CLP$";
                break;
            case LanguageEnum.CZ:
                _m = "Kč";
                break;
            case LanguageEnum.HU:
                _m = "Ft";
                break;
            default:
                break;
        }

        return _m;
    }

    public string ChangeMoneyAndIcon(float i, bool usFolat = true, string interval = "")
    {
        if (Language == LanguageEnum.ID)
        {
            return ChangeMoney(i, usFolat) + interval + ChangeMoneyIcon();
        }

        return ChangeMoneyIcon() + interval + ChangeMoney(i, usFolat);
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
