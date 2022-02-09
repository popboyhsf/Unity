﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Serialization;
using UnityEngine;

/// <summary>
/// 自己重写的 Button 按钮
/// 1、单击
/// 2、双击
/// 3、长按
/// </summary>    
public class MyButton : Selectable, IPointerClickHandler, ISubmitHandler
{
    [Header("冷却时间")]
    [SerializeField]
    float cdTime = 1f;

    [Serializable]
    /// <summary>
    /// Function definition for a button click event.
    /// </summary>
    public class ButtonClickedEvent : UnityEvent { }

    // Event delegates triggered on click.
    [FormerlySerializedAs("onClick")]
    [SerializeField]
    private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

    protected MyButton()
    { }


    public ButtonClickedEvent onClick
    {
        get { return m_OnClick; }
        set { m_OnClick = value; }
    }

    private void Press()
    {
        if (!IsActive() || !IsInteractable())
            return;

        UISystemProfilerApi.AddMarker("Button.onClick", this);
        m_OnClick.Invoke();
    }


    [Serializable]
    /// <summary>
    /// Function definition for a button click event.
    /// </summary>
    public class ButtonLongPressEvent : UnityEvent { }

    [FormerlySerializedAs("onLongPress")]
    [SerializeField]
    private ButtonLongPressEvent m_onLongPress = new ButtonLongPressEvent();
    public ButtonLongPressEvent onLongPress
    {
        get { return m_onLongPress; }
        set { m_onLongPress = value; }
    }

    [FormerlySerializedAs("OnDoubleClick")]
    public ButtonClickedEvent m_onDoubleClick = new ButtonClickedEvent();
    public ButtonClickedEvent OnDoubleClick
    {
        get { return m_onDoubleClick; }
        set { m_onDoubleClick = value; }
    }

    private bool my_isStartPress = false;

    private float my_curPointDownTime = 0f;

    private float my_longPressTime = 0.6f;

    private bool my_longPressTrigger = false;

    private float my_cdPressTime = 0f;

    private bool my_ISCDing = false;


    protected override void OnEnable()
    {
        base.OnEnable();

        my_cdPressTime = 0f;
        my_ISCDing = false;
    }

    void Update()
    {
        CheckIsLongPress();
        CheckCD();
    }

    void CheckCD()
    {
        if (my_ISCDing)
        {
            if (my_cdPressTime <= 0)
            {
                my_ISCDing = false;
            }
            else
            {
                my_cdPressTime -= Time.deltaTime;


            }
        }
    }

    void InitCD()
    {
        my_ISCDing = true;
        my_cdPressTime = cdTime;
    }

    void CheckIsLongPress()
    {
        if (my_ISCDing) return;
        if (my_isStartPress && !my_longPressTrigger)
        {
            if (Time.time > my_curPointDownTime + my_longPressTime)
            {
                //Debuger.Log("长按");
                InitCD();
                my_longPressTrigger = true;
                my_isStartPress = false;
                if (m_onLongPress != null)
                {
                    m_onLongPress.Invoke();
                }
            }
        }
    }



    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (my_ISCDing) return;
        //(避免已經點擊進入長按后，擡起的情況)
        if (!my_longPressTrigger)
        {
            
            if (eventData.clickCount == 2)// 双击
            {
                //Debuger.Log("双击");
                
                if (m_onDoubleClick.GetPersistentEventCount() > 0)
                {
                    m_onDoubleClick?.Invoke();
                    InitCD();
                }

            }
            else if (eventData.clickCount == 1)// 正常单击 
            {
                //Debuger.Log("单击");
                if (m_onDoubleClick.GetPersistentEventCount() <= 0) InitCD();
                else return;

                onClick?.Invoke();
            }
        }
    }



    public virtual void OnSubmit(BaseEventData eventData)
    {
        Press();

        // if we get set disabled during the press
        // don't run the coroutine.
        if (!IsActive() || !IsInteractable())
            return;

        DoStateTransition(SelectionState.Pressed, false);
        StartCoroutine(OnFinishSubmit());
    }

    private IEnumerator OnFinishSubmit()
    {
        var fadeTime = colors.fadeDuration;
        var elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        DoStateTransition(currentSelectionState, false);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        // 按下刷新當前時間
        base.OnPointerDown(eventData);
        my_curPointDownTime = Time.time;
        my_isStartPress = true;
        my_longPressTrigger = false;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        // 指針擡起，結束開始長按
        base.OnPointerUp(eventData);
        my_isStartPress = false;

    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        // 指針移出，結束開始長按，計時長按標志
        base.OnPointerExit(eventData);
        my_isStartPress = false;

    }
}