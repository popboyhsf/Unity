using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UIFrameWork;

public class GiftCardChosseManager : MonoBehaviour
{
    private static GiftCardChosseManager _instance;

    public static GiftCardChosseManager Instance { get => _instance; }

    [SerializeField]
    List<CountryGiftImageList> countryGiftImageLists = new List<CountryGiftImageList>();

    public List<CountryGiftImageList> CountryGiftImageLists { get => countryGiftImageLists; }


    [SerializeField]
    GiftCardChosseParam param;
    public GiftCardChosseParam Param { get => param; set => param = value; }



    [SerializeField]
    List<GameObject> bindList = new List<GameObject>();
    [SerializeField]
    GameObject loadingObj;

    private int bindIndex = 0;
    private List<GiftCardCheckTool> giftCardCheckTools = new List<GiftCardCheckTool>();

    private List<GiftCardChosseParam> giftCardChosseParams = new List<GiftCardChosseParam>();

    private bool isInited = false;

    public int remeberID { get; private set; } = 0;

    public IntData RemeberGiftCardTypeID { get; private set; } = new IntData("GiftCardChosseManager_RemeberGiftCardTypeID", -1);

    public StringData MailData { get; private set; } = new StringData("GiftCardChosseManager_mailData", "");

    public string mailData { get; set; }


    private void Awake()
    {
        _instance = this;
        param.gameObject.SetActive(false);
    }

    public void Show()
    {
        if (!AnalysisController.IsNonOrganic) return;

        var _country = I2Language.Instance.Language.ToString();

        if (RemeberGiftCardTypeID.Value < 0)
        {
            if (!isInited) Init(_country);

            if (IsHaseCountry(_country))
            {
                //展开UI1
                bindIndex = 0;
                //Choose(remeberID);
            }
            else
            {
                bindIndex = - 1;
            }
        }
        else
        {
            bindIndex = bindList.Count - 1;
        }

        if (bindIndex >= 0)
        {
            foreach (var item in bindList)
            {
                item.SetActive(false);
            }

            bindList[bindIndex].SetActive(true);

            UIManager.Instance.ShowWindow(WindowName.GiftCardIDbindUI);
        }
        else
        {
            Debuger.LogWarning("这个国家 _" + _country + "_没有对应的多选");
        }

    }

    public void NextParamUI()
    {
        foreach (var item in bindList)
        {
            item.SetActive(false);
        }


        var _i = ++bindIndex;

        if (_i > bindList.Count - 1)
        {
            //关闭整体
            UIManager.Instance.HideWindow(WindowName.GiftCardIDbindUI);

        }
        else
        {
            bindList[_i].SetActive(true);
        }
        
    }

    public void BackPramUI()
    {
        if (RemeberGiftCardTypeID.Value >= 0)
        {
            UIManager.Instance.HideWindow(WindowName.GiftCardIDbindUI);
            return;
        }

        foreach (var item in bindList)
        {
            item.SetActive(false);
        }

        var _i = --bindIndex;

        if (_i < 0)
        {
            //关闭整体
            UIManager.Instance.HideWindow(WindowName.GiftCardIDbindUI);
        }
        else
        {
            bindList[_i].SetActive(true);
        }
    }

    public void Rebeck()
    {
        if (!isInited)
        {
            var _country = I2Language.Instance.Language.ToString();
            Init(_country);
        } 

        foreach (var item in bindList)
        {
            item.SetActive(false);
        }

        bindIndex = 0;

        RemeberGiftCardTypeID.Value = -1;
        MailData.Value = "";

        bindList[bindIndex].SetActive(true);

        foreach (var item in giftCardCheckTools)
        {
             item.RefreshFunction();
        }
    }

    public void InitDate()
    {
        if (RemeberGiftCardTypeID.Value < 0)
        {
            RemeberGiftCardTypeID.Value = remeberID;
            MailData.Value = mailData;
            GiftCardManager.Instance.RefreshDot();
        }
        
        foreach (var item in giftCardCheckTools)
        {
            item.StartCoroutine(item.Refresh());
        }

        if (FirstCheck.GetIsGameFirst("GiftCardChosseManager_Post"))
        { 
            
            AnalysisController.TraceEvent(EventName.luck_card_X + (RemeberGiftCardTypeID.Value + 1));
            AnalysisController.TraceEvent(EventName.luck_accountready);

        }
    }

    public void Choose(int id)
    {
        for (int i = 0; i < giftCardChosseParams.Count; i++)
        {
            giftCardChosseParams[i].Choose(id == i);
        }
        remeberID = id;
        NextParamUI();
    }

    public Sprite GetGiftCardType(int ID)
    {
        var _country = I2Language.Instance.Language.ToString();

        foreach (var item in countryGiftImageLists)
        {
            if (item.country.Equals(_country))
            {
                foreach (var img in item.images)
                {
                    if(img.ID == ID)
                        return img.typeImages[RemeberGiftCardTypeID.Value < 0 ? remeberID : RemeberGiftCardTypeID.Value];
                }
            }
        }

        return null;
    }

    private void Init(string country,int ID = 0)
    {
        foreach (var item in countryGiftImageLists)
        {
            if (item.country.Equals(country))
            {
                var _id = 0;
                foreach (var img in item.images)
                {
                    if(img.ID == ID)
                        foreach (var _img in img.typeImages)
                        {
                            GiftCardChosseParam _sc = Instantiate(param, param.transform.parent);
                            _sc.Init(_img, _id++);
                            giftCardChosseParams.Add(_sc);
                            _sc.gameObject.SetActive(true);
                        }
                }
            }
        }

        FindAllTools();

        isInited = true;
    }

    private bool IsHaseCountry(string country)
    {
        var _b = false;
        foreach (var item in countryGiftImageLists)
        {
            if (item.country.Equals(country))
            {
                _b = true;
                break;
            }
        }
        return _b;
    }

    private void FindAllTools()
    {
        
        var _all = FindObjectsOfType<GiftCardCheckTool>();

        foreach (var item in _all)
        {
            giftCardCheckTools.Add(item);
        }
    }
}

[Serializable]
public class CountryGiftImageList
{
    public string country;
    public List<CountryTypeImageList> images;
}

[Serializable]
public struct CountryTypeImageList
{
    public int ID;
    public List<Sprite> typeImages;
}