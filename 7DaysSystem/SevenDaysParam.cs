using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SevenDaysParam : MonoBehaviour
{
    private bool isGet = false;

    public GameObject on, off;

    public Animator ani;

    public bool IsGet
    {
        get => isGet;
        set
        {
            isGet = value;
            on.SetActive(!isGet);
            off.SetActive(isGet);
        }
    }

    public void PlayGetAni()
    {
        ani.SetTrigger("Get");
    }

}
