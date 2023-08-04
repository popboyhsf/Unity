using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Mask))]
[RequireComponent(typeof(CanvasGroup))]
public class TextScroll : MonoBehaviour
{
    [Range(0.1f, 10)]
    public float speed = 3;
    public float scrollDelayTime = 2;
    public bool loop = true;
    private RectTransform contentTransform; 
    Vector2 StartPoint;
    TextMeshProUGUI content;
    Color myColor = Color.clear;
    bool activate = false;
    CanvasGroup myAlpha;
    Coroutine coroutine;
    bool LoopDirection = true;

    private void Awake()
    {
        myAlpha = GetComponent<CanvasGroup>();
        content = GetComponentInChildren<TextMeshProUGUI>();
        if (!content.GetComponent<ContentSizeFitter>())
        {
            content.gameObject.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        contentTransform = content.GetComponent<RectTransform>();
        myColor = content.color;
    }
    private void Update()
    {
        if (activate)
        {

            if (LoopDirection)
            {
                contentTransform.anchoredPosition -= new Vector2(speed * 100 * Time.deltaTime, 0f);
                if (contentTransform.anchoredPosition.x < -StartPoint.x)
                {
                    activate = false;
                    LoopDirection = false;
                    if(loop) coroutine = StartCoroutine(DelayTextScroll(scrollDelayTime));
                }
            }
            else
            {
                contentTransform.anchoredPosition += new Vector2(speed * 100 * Time.deltaTime, 0f);
                if (contentTransform.anchoredPosition.x > StartPoint.x)
                {
                    activate = false;
                    LoopDirection = true;
                    coroutine = StartCoroutine(DelayTextScroll(scrollDelayTime));
                }
            }
        }

    }
    public void ShowContent(string mContent = "")
    {
        if (myAlpha.alpha != 1)
            myAlpha.alpha = 1;
        if (mContent != "")
            content.text = mContent;

        if (coroutine != null)
            StopCoroutine(coroutine);
        StartCoroutine(Delay());

        activate = false;
        content.color = Color.clear;
    }
    public void HideContent()
    {
        if (myAlpha.alpha != 0)
            myAlpha.alpha = 0;
        if (myColor == Color.clear)
            myColor = content.color;

        activate = false;
        content.color = Color.clear;
    }
    private void TextReset()
    {
        if (contentTransform.sizeDelta.x < GetComponent<RectTransform>().sizeDelta.x)//文字不够长，显示在中间
        {
            StartPoint = contentTransform.anchoredPosition;
            StartPoint.x = 0;
            contentTransform.anchoredPosition = StartPoint;
        }
        else//文字太长，来回滚动显示
        {
            StartPoint = contentTransform.anchoredPosition;
            StartPoint.x = (contentTransform.sizeDelta.x / 2) - (GetComponent<RectTransform>().sizeDelta.x / 2);
            contentTransform.anchoredPosition = StartPoint;
            LoopDirection = true;
            coroutine = StartCoroutine(DelayTextScroll(scrollDelayTime));
        }
        content.color = myColor;
    }
    private IEnumerator DelayTextScroll(float mDelayTime)
    {
        yield return new WaitForSeconds(mDelayTime);
        activate = true;
    }
    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.2f);
        TextReset();
    }
    private void OnDestroy()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
    }
}
