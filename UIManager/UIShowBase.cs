using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShowBase : MonoBehaviour
{
    [SerializeField]
    GameObject self;
    [SerializeField]
    Animator ani;

    protected float aniTime = 0.65f;

    public virtual void ShowUI()
    {
        self.SetActive(true);
    }

    public virtual void HiddenUI()
    {
        if (ani)
        {
            ani.SetTrigger("Closs");
            Invoke("HiddenAftor", aniTime);
        }
        else
        {
            HiddenAftor();
        }

    }

    public virtual void HiddenAftor()
    {
        self.SetActive(false);
    }

    public virtual void ChangeUI()
    {

    }
}
