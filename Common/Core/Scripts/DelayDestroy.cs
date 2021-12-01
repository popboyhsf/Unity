using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 延迟销毁
/// </summary>
public class DelayDestroy : MonoBehaviour
{
    private UnityAction delayAction;
    public void Init(float delayTime,UnityAction delayAction)
    {
        Invoke("Destroy", delayTime);
        this.delayAction = delayAction;
    }

    public void Destroy()
    {
        delayAction?.Invoke();
        Destroy(gameObject);
    }
}
