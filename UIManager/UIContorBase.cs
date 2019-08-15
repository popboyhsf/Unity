using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIContorBase : MonoBehaviour
{
    protected abstract UIType UIType { get; }

    [SerializeField]
    UIShowBase UIShowBase;

    private void Awake()
    {
        UIManager.VisitUIDataBase(UIType).UIContor = this;
    }

    public virtual void ShowUI()
    {
        UIShowBase.ShowUI();
    }

    public virtual void HiddenUI()
    {
        UIShowBase.HiddenUI();
    }

    public virtual void ChangeUI()
    {
        UIShowBase.ChangeUI();
    }
}
