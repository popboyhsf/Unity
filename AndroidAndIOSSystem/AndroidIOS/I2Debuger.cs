using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class I2Debuger : MonoBehaviour, IDebuger
{
    private bool allowDebug = false;
    public bool AllowDebug
    {
        get => allowDebug;
        set
        {
            allowDebug = value;
            if (allowDebug)
            {
                Init();
                self.SetActive(true);
            }
            else
            {
                self.SetActive(false);
            }
        }
    }

    public string AllowName => "显示语言切换";

    [SerializeField]
    GameObject self;
    [SerializeField]
    Button param;


    private bool inited = false;
    private void Init()
    {
        if (inited) return;
        
        foreach (I2Language.LanguageEnum item in Enum.GetValues(typeof(I2Language.LanguageEnum)))
        {
            var _button = Instantiate(param, param.transform.parent);
            _button.onClick.AddListener(()=> {
                I2Language.Instance.ApplyLanguage(item);
                I2Language.Instance.ChangeUI(item);
                
            });
            _button.transform.GetChild(0).GetComponent<Text>().text = item.ToString();
            _button.gameObject.SetActive(true);
        }
        inited = true;

    }

    private void Start()
    {
        self.SetActive(false);
        param.gameObject.SetActive(false);
    }
}
