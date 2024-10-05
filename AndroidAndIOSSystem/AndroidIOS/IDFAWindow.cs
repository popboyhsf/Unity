using System.Collections;
using System.Collections.Generic;
using UIFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class IDFAWindow : UIBaseWindow
{
 
    public override WindowName WindowName { get; } = WindowName.WWIDFA;
    public override WindowType WindowType { get; } = WindowType.Top;

    [SerializeField]
    Button clickAllow;


    private void Awake()
    {
        clickAllow.onClick.AddListener(()=> {

            IOSIDFA.Instance.ClickAllow();

            Hide();

        });
    }



   
}
