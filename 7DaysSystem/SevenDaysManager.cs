using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SevenDaysManager : MonoBehaviour
{
    public GameObject self;


    public List<SevenDaysParam> aniList = new List<SevenDaysParam>();

    public Button click;

    public Animator ani;


    private void Awake()
    {
        click.onClick.AddListener(Click);
    }



    private void Start()
    {
        if (SevenDaysData.GetTime() >= 24) Init();
    }

    private void Click()
    {
        click.interactable = false;

        var index = SevenDaysData.GetCount();

        switch (index)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;

            default:
                break;
        }


        aniList[index].PlayGetAni();
        SevenDaysData.SetTime();
        SevenDaysData.SetCoun();
        Invoke("WaitForAni", 1.0f);

    }

    private void WaitForAni()
    {
        ani.SetTrigger("Closs");
        Invoke("AniCallBack", 0.65f);
    }

    private void AniCallBack()
    {
        self.SetActive(false);
    }

    private void Init()
    {
        click.interactable = true;

        self.SetActive(true);

        var count = Mathf.Min(SevenDaysData.GetCount(), aniList.Count-1) ;

        for (int i = 0; i < aniList.Count; i++)
        {
            if(i<count)
                aniList[i].IsGet = true;
            else
                aniList[i].IsGet = false;
        }
    }




    //三个调用函数
    private void GetMoney(long money)
    {
        MoneyManager.Instance.CallBackAMoney(money,1);
    }
    private void GetNewHook()
    {

    }
    private void GetGoleHook()
    {
        BSetBoxData.Instance.SetIsDouble = true;
        Player.Instance.HookToGold();
    }
}
