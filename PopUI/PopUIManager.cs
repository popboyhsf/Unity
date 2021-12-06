using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PopUIManager : MonoBehaviour
{
    [SerializeField]
    GameObject self;


    private Dictionary<string, PopUIBase> popUIDic = new Dictionary<string, PopUIBase>();

    private static PopUIManager _instance;

    public static PopUIManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public Dictionary<string, PopUIBase> GetPopUIDic { get => popUIDic;}

    public UnityAction BeforShowAction;
    public UnityAction AfterHiddenAction;

    private void Awake()
    {
        _instance = this;
        foreach (Transform item in self.transform)
        {
            PopUIBase _base = item.GetComponent<PopUIBase>();
            popUIDic.Add(_base.thisPopUIEnum.ToString(), _base);
        }

        self.SetActive(false);
    }

    public void ShowUI(PopUIEnum uIEnum)
    {
        self.SetActive(true);
        if (UIBase(uIEnum) == null) return;
        if (UIBase(uIEnum).UseBaseBeforActive) BeforShowAction?.Invoke();
        UIBase(uIEnum).ShowUI();
    }

    public void ShowUI(string uIEnum)
    {
        self.SetActive(true);
        if (UIBase(uIEnum) == null) return;
        if (UIBase(uIEnum).UseBaseBeforActive) BeforShowAction?.Invoke();
        UIBase(uIEnum).ShowUI();
    }

    public void HiddenUI(PopUIEnum uIEnum)
    {
        if (UIBase(uIEnum) == null) return;
        if (UIBase(uIEnum).UseBaseAfterActive) AfterHiddenAction?.Invoke();
        UIBase(uIEnum).HiddenUIAI();
    }
    public void HiddenUI(string uIEnum)
    {
        if (UIBase(uIEnum) == null) return;
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

    public T SpawnUI<T>(string uIEnum) where T : PopUIBase
    {
        return UIBase(uIEnum) as T;
    }

    private PopUIBase UIBase(PopUIEnum uIEnum)
    {
        if (!popUIDic.ContainsKey(uIEnum.ToString()))
        {
            Debuger.LogError("不含有："+ uIEnum.ToString());
            return null;
        }
        return popUIDic[uIEnum.ToString()];
    }

    private PopUIBase UIBase(string uIEnum)
    {
        if (!popUIDic.ContainsKey(uIEnum))
        {
            Debuger.LogError("不含有：" + uIEnum);
            return null;
        }
        return popUIDic[uIEnum.ToString()];
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
