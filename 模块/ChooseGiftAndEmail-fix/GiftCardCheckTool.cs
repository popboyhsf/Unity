using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiftCardCheckTool : MonoBehaviour
{
    [SerializeField]
    int ID;
    [SerializeField]
    bool isUseWait = false;
    [SerializeField]
    Sprite localIcon;
    [Space]
    [SerializeField]
    bool usResize = false;

    private Image icon;
    private Image Icon
    {
        get
        {
            if (icon == null)
                icon = this.GetComponent<Image>();

            return icon;
        }
    }


    private void OnEnable()
    {
        StartCoroutine(Refresh());
    }

    private void Start()
    {
        AnalysisController.OnAFStatusChanged += RefreshFunction;
        I2Language.Instance.OnChangeLanguage += RefreshFunction;
    }

    public IEnumerator Refresh()
    {
        while (GiftCardChosseManager.Instance == null)
        {
            yield return null;
        }

        if (isUseWait && GiftCardChosseManager.Instance.RemeberGiftCardTypeID.Value < 0)
        {
            if (localIcon)
            {
                Icon.sprite = localIcon;
                if (usResize) Icon.SetNativeSize();
            }
            yield break;
        }

        if (GiftCardChosseManager.Instance.GetGiftCardType(ID) != null)
        {

            var _sc = this.GetComponent<Localize>();

            if (_sc != null)
            {
                _sc.enabled = false;
            }

            Icon.sprite = GiftCardChosseManager.Instance.GetGiftCardType(ID);
            if (usResize) Icon.SetNativeSize();
        }
    }

    public void RefreshFunction()
    {
        if (GiftCardChosseManager.Instance == null)
        {
            return;
        }

        if (isUseWait && GiftCardChosseManager.Instance.RemeberGiftCardTypeID.Value < 0)
        {
            if (localIcon) Icon.sprite = localIcon;
            return;
        }

        if (GiftCardChosseManager.Instance.GetGiftCardType(ID) != null)
        {

            var _sc = this.GetComponent<Localize>();

            if (_sc != null)
            {
                _sc.enabled = false;
            }

            Icon.sprite = GiftCardChosseManager.Instance.GetGiftCardType(ID);
        }
    }
}
