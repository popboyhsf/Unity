using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoHight : MonoBehaviour
{
    [SerializeField]
    float height;
    RectTransform rectSelf;
    RectTransform RectSelf 
    {
        get
        {
            if (rectSelf == null)
                rectSelf = this.GetComponent<RectTransform>();
            return rectSelf;
        }
    }


    private void Start()
    {
        RectSelf.sizeDelta = new Vector2(RectSelf.sizeDelta.x, height);
    }
}
