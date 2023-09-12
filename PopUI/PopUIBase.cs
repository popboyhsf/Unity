using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PopUIBase : MonoBehaviour
{
    [SerializeField]
    protected Animator ani;
    [SerializeField]
    protected GameObject self;

    protected List<Button> buttonlist = new List<Button>();

    protected float aniTime = 0.38f;

    public abstract string thisPopUIEnum { get; }
    public abstract string thisUIType { get; }

    public virtual bool UseBaseBeforActive { get; } = false;
    public virtual bool UseBaseAfterActive { get; } = false;

    /// <summary>
    /// 打开之前
    /// </summary>
    public abstract void BeforShow(object[] value);


    public virtual void ShowUI(params object[] value)
    {
        BeforShow(value);
        foreach (var item in buttonlist)
        {
            item.interactable = true;
        }
        self.SetActive(true);
    }

    /// <summary>
    /// 有超过一个（不包含自我）存在时则走WithOut路线 
    /// </summary>
    public void HiddenUIAI()
    {
        var i = PopUIManager.Instance.ActiveUINum();
        if (i >= 2)
        {
            HiddenUIWithOutHiddeParent();
        }
        else
        {
            HiddenUI();

        }

    }
    private void HiddenUIWithOutHiddeParent()
    {
        if (!ani)
        {
            AniCallBack2();
        }
        else
        {
            ani.SetTrigger("Closs");
            foreach (var item in buttonlist)
            {
                item.interactable = false;
            }
            Invoke("AniCallBack2", aniTime);
        }

    }

    private void HiddenUI()
    {
        if (!ani)
        {
            AniCallBack();
        }
        else
        {
            ani.SetTrigger("Closs");
            foreach (var item in buttonlist)
            {
                item.interactable = false;
            }
            Invoke("AniCallBack", aniTime);
        }
       
    }

    private void AniCallBack()
    {
        self.SetActive(false);
        PopUIManager.Instance.HiddenUIForAni(this);
        AfterHiddenUI();
    }
    private void AniCallBack2()
    {
        self.SetActive(false);
        AfterHiddenUI();
    }



    /// <summary>
    /// 关闭之后
    /// </summary>
    public abstract void AfterHiddenUI();
}
