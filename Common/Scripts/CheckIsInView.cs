using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIsInView : MonoBehaviour
{

    private Camera targetcCamera;

    private float scenceOffset = 0.5f;

    public bool allowCheck = true;


    private float length = 0;
    public bool IsInView()
    {
        if (!targetcCamera) targetcCamera = Camera.main;
        var worldPos = this.transform.position;
        Transform camTransform = targetcCamera.transform;
        Vector2 viewPos = targetcCamera.WorldToViewportPoint(worldPos);
        Vector3 dir = (worldPos - camTransform.position).normalized;
        float dot = Vector3.Dot(camTransform.forward, dir);     //判断物体是否在相机前面
        float leftf = Vector3.Cross(camTransform.forward, dir).y;


        if (dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= -scenceOffset && viewPos.y <= (1 + scenceOffset))
            return true;
        else
            return false;
    }


    IEnumerator CheckIsIn()
    {
        while (allowCheck)
        {
            if (IsInView())
            {
                //Debug.Log("目前本物体在摄像机范围内 ==== " + this.name);
                if(!this.gameObject.activeSelf) this.gameObject.SetActive(true);
            }
            else
            {
                //Debug.Log("目前本物体不在摄像机范围内 ==== " + this.name);
                if (this.gameObject.activeSelf) this.gameObject.SetActive(false);
            }
            yield return null;
        }
        ani.enabled = false;
        yield break;
    }

    public void Init()
    {
        allowCheck = true;

        this.transform.parent.GetComponent<MonoBehaviour>().StartCoroutine(CheckIsIn());
    }
}
