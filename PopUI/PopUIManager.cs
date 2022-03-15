using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PopUIManager : MonoBehaviour
{
    [SerializeField]
    GameObject self;
    [SerializeField]
    Transform Top, Pop;


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

    public UnityAction BeforShowAction { get; set; }
    public UnityAction AfterHiddenAction { get; set; }

    private void Awake()
    {
        _instance = this;

        List<Transform> childrenTr = new List<Transform>(self.transform.childCount);
        for (int i = 0; i < self.transform.childCount; i++)
        {
            childrenTr.Add(self.transform.GetChild(i));
        }

        foreach (Transform item in childrenTr)
        {
            PopUIBase _base = item.GetComponent<PopUIBase>();
            if (_base == null)
            {
                Debuger.LogError(item.name + " 没有包含UI脚本");
            }
            else
            {
                if (!popUIDic.ContainsKey(_base.thisPopUIEnum))
                    popUIDic.Add(_base.thisPopUIEnum, _base);
                else
                    Debuger.LogError("重复添加：" + _base.thisPopUIEnum);


                if (_base.thisUIType.ToLower().Equals("pop"))
                {
                    _base.transform.SetParent(Pop);
                }

                if (_base.thisUIType.ToLower().Equals("top"))
                {
                    _base.transform.SetParent(Top);
                }
            }
        }

        self.SetActive(false);
        Top.gameObject.SetActive(false);
        Pop.gameObject.SetActive(false);
    }

    public void ShowUI(PopUIEnum uIEnum)
    {
        var _ui = UIBase(uIEnum);
        if (_ui == null) return;

        _ui.transform.parent.gameObject.SetActive(true);

        if (_ui.UseBaseBeforActive) BeforShowAction?.Invoke();
        _ui.ShowUI();
    }

    public void ShowUI(string uIEnum)
    {
        var _ui = UIBase(uIEnum);
        if (_ui == null) return;

        _ui.transform.parent.gameObject.SetActive(true);

        if (_ui.UseBaseBeforActive) BeforShowAction?.Invoke();
        _ui.ShowUI();
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

    public void HiddenUIForAni(PopUIBase uIEnum)
    {
        uIEnum.transform.parent.gameObject.SetActive(true);
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
