using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDataBase
{
    public UIContorBase UIContor;
    protected UIStatus UIStat;

    public UIStatus GetUIStatus
    {
        get => UIStat;
    }

    protected void ShowUI()
    {
        UIContor.ShowUI();
        UIStat = UIStatus.Showing;
    }

    protected void HiddenUI()
    {
        UIContor.HiddenUI();
        UIStat = UIStatus.Hidden;
    }
}
