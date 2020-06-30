using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class FlopButton : MonoBehaviour
{
    private FlopData.flopEnum flopType;
    private int num;

    private void Awake()
    {
        this.GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void Init(FlopData.flopEnum flop,int num)
    {
        this.flopType = flop;
        this.num = num;
    }

    private void OnClick()
    {
        if (FlopData.Count > 0)
        {
            FlopData.SumList.Add(new FlopData.flopS(flopType, num));
            Show();
            --FlopData.Count;
        }
        else
        {
            Debuger.Log("次数不足");
        }

    }

    /// <summary>
    /// 被点击时候的表现效果
    /// </summary>
    private void Show()
    {

    }

   
}
