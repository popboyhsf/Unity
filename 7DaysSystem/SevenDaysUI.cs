using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class SevenDaysUI : PopUIBase
{


    [SerializeField]
    MyButton closs;

    public override string thisPopUIEnum => "signUI";

    private void Awake()
    {
        closs.onClick.AddListener(()=> {
            HiddenUIAI();
        });
    }


    public override void AfterHiddenUI()
    {
        
    }

    public override void BeforShow()
    {
        
    }



}
