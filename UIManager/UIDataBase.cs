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

    public void ShowUI()
    {
        UIContor.ShowUI();
        UIStat = UIStatus.Showing;
    }

    public void HiddenUI()
    {
        UIContor.HiddenUI();
        UIStat = UIStatus.Hidden;
    }
}
