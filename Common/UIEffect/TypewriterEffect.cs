using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

#if TMPRO
using TMPro;
#endif

/// <summary>  
/// 此脚本是能够将文本字符串随着时间打字或褪色显示。  
/// </summary>  
#if TMPRO
[RequireComponent(typeof(TextMeshProUGUI))]
#else
[RequireComponent(typeof(Text))]
#endif

[AddComponentMenu("Typewriter Effect")]
public class TypewriterEffect : MonoBehaviour
{
    enum TypeState
    {
        Null,
        Typing,
        TypeEnd,
        Ended
    }

    int charsPerSecond = 10;

    public bool isTyping;
    private float timer;
    private string words;

    private UnityAction onTypeEnded;

#if TMPRO
    private TextMeshProUGUI _text;
    private TextMeshProUGUI mText { get { if (_text == null) _text = GetComponent<TextMeshProUGUI>(); return _text; } }
#else    
    private Text _text;
    private Text mText { get { if (_text == null) _text = GetComponent<Text>(); return _text; } }
#endif

    public void Show(string words,UnityAction onTalkEnd, int charsPerSecond=10)
    {
        this.words = words;
        this.onTypeEnded = onTalkEnd;
        timer = 0;
        isTyping = true;
        this.charsPerSecond = Mathf.Max(1, charsPerSecond);
        mText.text = "";
    }

    void Update()
    {
        if (isTyping)
        {
            try
            {
                mText.text = words.Substring(0, (int)(charsPerSecond * timer));
                timer += Time.unscaledDeltaTime;
            }
            catch (Exception)
            {
                FinishType();
            }
        }
    }

    public void FinishType()
    {
        if (isTyping)
        {
            isTyping = false;
            timer = 0;
            mText.text = words;
            onTypeEnded?.Invoke();
        }
    }


}