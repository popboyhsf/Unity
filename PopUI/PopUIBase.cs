using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PopUIBase : MonoBehaviour
{
    [SerializeField]
    Animator ani;
    [SerializeField]
    protected GameObject self;

    protected List<Button> buttonlist = new List<Button>();

    protected float aniTime = 0.38f;

    /// <summary>
    /// 打开之前
    /// </summary>
    public abstract void BeforShow();
    /// <summary>
    /// 打开之后
    /// </summary>

    public virtual void AfterShow()
    {

    }

    public void ShowUI()
    {
        BeforShow();
        foreach (var item in buttonlist)
        {
            item.interactable = true;
        }
        self.SetActive(true);
        AfterShow();
    }

    public void HiddenUIWithOutHiddeParent()
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

    public void HiddenUI()
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

    private void AniCallBack()
    {
        self.SetActive(false);
        PopUIManager.Instance.HiddenUIForAni();
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
