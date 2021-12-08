using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetWorkStateController : MonoBehaviour
{

    private static NetWorkStateController _instance;

    public static NetWorkStateController Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField]
    GameObject self;
    [SerializeField]
    Button retryB;


    private void Awake()
    {
        _instance = this;
        retryB.onClick.AddListener(()=> {
            retryB.interactable = false;
            StartCoroutine(openStart());

        });
    }

    public void Show()
    {

        self.SetActive(true);
        retryB.interactable = false;
        StartCoroutine(openStart());
    }

    public void Hidden()
    { 

        self.SetActive(false);

    }

    private IEnumerator openStart()
    {
        yield return new WaitForSecondsRealtime(15.0f);
        retryB.interactable = true;
    }

}
