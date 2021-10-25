using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasHandle : SingletonMonoBehaviour<CanvasHandle>
{
    // Start is called before the first frame update

    [SerializeField]
    bool isListScale = false;

    [SerializeField]
    List<Transform> scaleList = new List<Transform>();

    [SerializeField]
    float listScaleOffset = 1f;
    [Tooltip("初始宽度")]
    [SerializeField]
    float standard_width = 1080f;
    [Tooltip("初始高度 ")]
    [SerializeField]
    float standard_height = 1920f;

    public float standard_aspect = -1f;
    public float device_aspect = -1f;
    public float adjustor = -1f;

    //void Fix

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        float device_width = 0f;            //当前设备宽度
        float device_height = 0f;           //当前设备高度

        //获取设备宽高        
        device_width = Screen.width;
        device_height = Screen.height;

        //计算宽高比例        
        standard_aspect = standard_width / standard_height;
        device_aspect = device_width / device_height;



        //计算矫正比例        
        if (device_aspect < standard_aspect)
        {
            adjustor = standard_aspect / device_aspect;
        }

        Debuger.Log("standard_aspect == " + standard_aspect);
        Debuger.Log("device_aspect == " + device_aspect);
        Debuger.Log("adjustor == " + adjustor);

        if (isListScale)
        {
            if (adjustor > 0)
            {
                foreach (var item in scaleList)
                {
                    item.localScale = new Vector3(item.localScale.x * adjustor * listScaleOffset, item.localScale.y * adjustor * listScaleOffset, item.localScale.z);
                }
            }
        }
        else
        {
            CanvasScaler canvasScalerTemp = transform.GetComponent<CanvasScaler>();

            if (device_aspect > standard_aspect || adjustor > 0)
            {
                canvasScalerTemp.matchWidthOrHeight = 1;
            }
            else
            {
                canvasScalerTemp.matchWidthOrHeight = 0;
            }
        }


    }

}