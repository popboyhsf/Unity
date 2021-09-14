using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PopUIManager : MonoBehaviour
{
    [SerializeField]
    GameObject self;


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

    public UnityAction BeforShowAction;
    public UnityAction AfterHiddenAction;

    private void Awake()
    {
        _instance = this;
        foreach (Transform item in self.transform)
        {
            PopUIBase _base = item.GetComponent<PopUIBase>();
            popUIDic.Add((int)_base.thisPopUIEnum, _base);
        }

        self.SetActive(false);
    }

    public void ShowUI(PopUIEnum uIEnum)
    {
        self.SetActive(true);
        PopUISound(uIEnum);
        if (UIBase(uIEnum).UseBaseBeforActive) BeforShowAction?.Invoke();
        UIBase(uIEnum).ShowUI();
    }

    public void HiddenUI(PopUIEnum uIEnum)
    {
        if (UIBase(uIEnum).UseBaseAfterActive) AfterHiddenAction?.Invoke();
        UIBase(uIEnum).HiddenUIAI();
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
        foreach (var item in popUIDic.Values)
        {
            if (item.gameObject.activeSelf) i++;
        }
        return i;
    }

}
