using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RateUs : PopUIBase
{
    public static bool Rated { get; private set; } = false;

    public override string thisPopUIEnum => PopUIEnum.RateUS.ToString();

    public override string thisUIType => PopUIType.POP.ToString();

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
        btnSubmit.onClick.AddListener(() => {

#if UNITY_ANDROID
            OnClickSubmit();
#elif UNITY_IPHONE
            OnClickSubmitIOS();
#endif


        });
        btnClose.onClick.AddListener(HiddenUIAI);
        RefreshDisplay();
        Rated = true;

    }

    public override void BeforShow(object[] value)
    {
#if UNITY_ANDROID
            
#elif UNITY_IPHONE
        CrossIos.RateUSShow();
#endif
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
            Application.OpenURL(About.GPUrl);
            HiddenUIAI();
        }
        else
        {
            HiddenUIAI();
        }
    }

    private void OnClickSubmitIOS()
    {
        CrossIos.RateUS(selectStartCount, btnStars.Length, About.IOSID);
        HiddenUIAI();
    }

    public override void AfterHiddenUI()
    {

    }
}
