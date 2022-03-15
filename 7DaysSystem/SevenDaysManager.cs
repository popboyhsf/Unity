using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SevenDaysManager : MonoBehaviour
{
    private static SevenDaysManager _instance;

    public List<SevenDaysParam> aniList = new List<SevenDaysParam>();

    [SerializeField]
    List<SevenDayIcon> sevenDayIcons = new List<SevenDayIcon>();

    [SerializeField]
    TextMeshProUGUI lun, baifen;

    [SerializeField]
    float clickAniWait = 0f;

    public Button click;

    public Animator ani;

    public static SevenDaysManager Instance { get => _instance; }

    private void Awake()
    {
        click.onClick.AddListener(Click);
        _instance = this;
    }

    public void Show()
    {

        if (SevenDaysData.GetTime() >= 1 && SevenDaysData.GetCount() < aniList.Count)
        {
            Init();
            PopUIManager.Instance.ShowUI("signUI");
        }
    }

    private void Click()
    {
        click.interactable = false;

        var index = Mathf.Clamp(SevenDaysData.GetCount(), 0, 6);

        var count = SevenDaysData.SevenDayDataL[index];

        SevenDaysData.flopEnum f = (SevenDaysData.flopEnum)Enum.Parse(typeof(SevenDaysData.flopEnum), count.count);

        var num = SevenDaysData.GetNum(index);

        switch (f)
        {
            case SevenDaysData.flopEnum.Coin:
                GoldManager.Instance.AddGold(num);
                break;
            case SevenDaysData.flopEnum.Unkonw:
                Debuger.LogError("签到奖励内容出错，请检查 === " + num);
                break;
            default:
                break;
        }



        aniList[index].PlayGetAni();
        SevenDaysData.SetTime();
        SevenDaysData.SetCoun();

        Invoke("WaitForAni", clickAniWait);

    }

    private void WaitForAni()
    {
        if (ani) ani.SetTrigger("Closs");
        Invoke("AniCallBack", ani.GetAnimationClipLength("pop close"));
    }

    private void AniCallBack()
    {
        PopUIManager.Instance.HiddenUI("signUI");
    }

    private void Init()
    {
        click.interactable = true;

        var count = Mathf.Min(SevenDaysData.GetCount(), aniList.Count - 1);

        var _dayC = 1;

        for (int i = 0; i < aniList.Count; i++)
        {
            if (i < count)
                aniList[i].IsGet();
            else
            {
                if (i == count)
                    aniList[i].IsNow();
                else
                    aniList[i].IsNotGet();

            }           
            sevenDayIcons.ForEach(j => {
                var countA = SevenDaysData.SevenDayDataL[i];
                if (countA.count.Equals(j.flopEnum.ToString()))
                {
                    var s = j.sprite;
                    aniList[i].Init(s, SevenDaysData.GetNum(i), _dayC++);
                }
            });

        }

        if (lun) lun.text = (SevenDaysData.GetLunCount()+1).ToString("0");
        if (baifen) baifen.text = ((SevenDaysData.GetLunCount() + 1) * 100) + "%";

    }


}
