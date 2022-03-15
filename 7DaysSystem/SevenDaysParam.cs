using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using I2.Loc;

public class SevenDaysParam : MonoBehaviour
{

    [SerializeField]
    List<GameObject> now ,on, off = new List<GameObject>();
    [SerializeField]
    Animator ani;
    [SerializeField]
    Image icon;
    [SerializeField]
    TextMeshProUGUI numT,dayT;

    public void Init(Sprite sprite,int num,int day)
    {
        if (icon) icon.sprite = sprite;
        if (numT) numT.text = num.ToPriceString();
        if (dayT)
        {
            if (dayT.GetComponent<LocalizationParamsManager>())
                dayT.GetComponent<LocalizationParamsManager>().SetParameterValue("X", day.ToString("0"));
            else
                dayT.text = "Day " + day;
        }
    }

    public void IsGet()
    {

        on.ForEach(i => i.SetActive(true));
        off.ForEach(i => i.SetActive(false));
        now.ForEach(i => i.SetActive(false));

    }

    public void IsNotGet()
    {
        on.ForEach(i => i.SetActive(false));
        off.ForEach(i => i.SetActive(true));
        now.ForEach(i => i.SetActive(false));
    }
    
    public void IsNow()
    {
        on.ForEach(i => i.SetActive(false));
        off.ForEach(i => i.SetActive(false));
        now.ForEach(i => i.SetActive(true));
    }

    public void PlayGetAni()
    {
        if(ani)ani.SetTrigger("Get");
        IsGet();
    }

}
