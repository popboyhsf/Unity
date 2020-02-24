using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUIManager : MonoBehaviour
{
    [SerializeField]
    GameObject self;
    [SerializeField]
    List<PopUIListC> popUIList = new List<PopUIListC>();


    private Dictionary<int, PopUIBase> popUIDic = new Dictionary<int, PopUIBase>();

    private static PopUIManager _instance;

    public static PopUIManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public Dictionary<int, PopUIBase> GetPopUIDic { get => popUIDic;}

    private void Awake()
    {
        _instance = this;
        foreach (var item in popUIList)
        {
            popUIDic.Add((int)item.UIEnum,item.UIBase);
        }

        self.SetActive(false);
    }

    public void ShowUI(PopUIEnum uIEnum)
    {
        self.SetActive(true);
        PopUISound(uIEnum);
        UIBase(uIEnum).ShowUI();
    }

    public void HiddenUI(PopUIEnum uIEnum)
    {
        UIBase(uIEnum).HiddenUI();
    }

    public void HiddenUIForAni()
    {
        self.SetActive(false);
    }

    public T SpawnUI<T>(PopUIEnum uIEnum) where T : PopUIBase
    {
        return UIBase(uIEnum) as T;
    }

    private PopUIBase UIBase(PopUIEnum uIEnum)
    {
        return popUIDic[(int)uIEnum];
    }

    private void PopUISound(PopUIEnum uIEnum)
    {

    }

    public int ActiveUINum()
    {
        var i = 0;
        foreach (var item in popUIList)
        {
            if (item.UIBase.gameObject.activeSelf) i++;
        }
        return i;
    }

}

[Serializable]
public class PopUIListC
{
    public PopUIEnum UIEnum;
    public PopUIBase UIBase;
}
