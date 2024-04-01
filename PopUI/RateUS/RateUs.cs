using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RateUs : PopUIBase
{
    public static bool Rated { get; private set; } = false;
    [SerializeField]
    GameObject guideHand;
    [SerializeField]
    Button[] btnStars;
    [SerializeField]
    Image[] starts;
    [SerializeField]
    Button btnSubmit, btnClose;

    int selectStartCount = 0;

    private void Start()
    {
        for (int i = 0; i < btnStars.Length; i++)
        {
            int showCount = i + 1;
            btnStars[i].onClick.AddListener(() => { OnClickStar(showCount); });
        }
        btnSubmit.interactable = false;
        btnSubmit.onClick.AddListener(OnClickSubmit);
        btnClose.onClick.AddListener(HiddenUIAI);
        RefreshDisplay();
        Rated = true;

    }

    private void OnClickStar(int showCount)
    {
        selectStartCount = showCount;
        btnSubmit.interactable = true;
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {

        for (int i = 0; i < selectStartCount; i++)
        {
            starts[i].gameObject.SetActive(true);
        }
        for (int i = selectStartCount; i < starts.Length; i++)
        {
            starts[i].gameObject.SetActive(false);
        }
    }


    private void OnClickSubmit()
    {
        if (selectStartCount == btnStars.Length)
        {
#if UNITY_ANDROID
            Application.OpenURL(About.GPUrl);
#elif UNITY_IPHONE
            CrossIos.RateUS(selectStartCount, btnStars.Length, About.IOSID);
#endif
            HiddenUIAI();
        }
        else
        {
            HiddenUIAI();
        }
    }



    public override void FSetBeforShow()
    {
#if UNITY_ANDROID
            
#elif UNITY_IPHONE
        CrossIos.RateUSShow();
#endif
    }

    public override void FSetAfterHiddenUI()
    {
        
    }

    public void PlayOpen()
    {
        ani.SetTrigger("Open");
    }
}
