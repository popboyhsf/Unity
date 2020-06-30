using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlopUI : PopUIBase
{


    [SerializeField]
    FlopButton param;
    [SerializeField]
    Button watchVideo,getAll,getAllDouble;

    private void Awake()
    {
        watchVideo.onClick.AddListener(ADButtonGetThree);
        getAll.onClick.AddListener(()=> { this.Sum(1); });
        getAllDouble.onClick.AddListener(ADButtonGetDouble);
    }

    public override void FSetBeforShow()
    {
        Init();
    }

    public override void FSetAfterHiddenUI()
    {
        
    }

    /// <summary>
    /// 初始化按钮与对应属性
    /// </summary>
    private void Init()
    {
        List<int> list = new List<int>();
        FlopData.RedomList(out list);

        foreach (var item in list)
        {
            var obj = Instantiate(param, param.transform);
            var i = FlopData.FlopInfoD[item].content;

            FlopData.flopEnum type = (FlopData.flopEnum)Enum.Parse(typeof(FlopData.flopEnum), i);

            var j = Utils.Random(FlopData.FlopInfoD[item].min, FlopData.FlopInfoD[item].max);

            obj.Init(type, j);
        }

    }


    /// <summary>
    /// 结算
    /// </summary>
    /// <param name="dou">倍数 默认单倍</param>
    private void Sum(int dou = 1)
    {
        foreach (var item in FlopData.SumList)
        {
            Send(item.type,item.num * dou);
        }
        FlopData.SumList.Clear();

        HiddenUIAI();
        PopUIManager.Instance.ShowUI(PopUIEnum.wheel);
    }

    /// <summary>
    /// 处理结算队列每项
    /// </summary>
    /// <param name="type">结算种类</param>
    /// <param name="num">数量</param>

    private void Send(FlopData.flopEnum type,int num)
    {
        switch (type)
        {
            case FlopData.flopEnum.money:

                break;
            default:
                break;
        }

    }


    private void ADButtonGetThree()
    {
        ADCallBackGetThree();
    }

    private void ADCallBackGetThree()
    {
        FlopData.Count += 3;
    }

    private void ADButtonGetDouble()
    {
        Sum(2);
    }

}
