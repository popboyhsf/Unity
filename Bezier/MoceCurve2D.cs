using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoceCurve2D : MonoBehaviour
{
    int vertexCount;
    int xOffset;
    int yOffset;

    /// <summary>
    /// 开始Sin形式移动
    /// </summary>
    /// <param name="vertexCount">精度（越高速度越慢）</param>
    ///<param name="xOffset">X轴偏移</param>
    /// <param name="yOffset">Y轴偏移</param>
    public void StartSinMove(int xOffset,int yOffset, int vertexCount)
    {
        this.xOffset = xOffset;
        this.yOffset = yOffset;

        this.vertexCount = vertexCount;
        StartCoroutine(StartMoveSin());
    }

    /// <summary>
    /// 开始Sin形式移动
    /// </summary>
    /// <param name="speed">速度 默认0.2f</param>
    ///<param name="xOffset">X轴偏移</param>
    /// <param name="yOffset">Y轴偏移</param>
    public void StartSinMove(int xOffset, int yOffset, float speed = 0.2f)
    {
        this.xOffset = xOffset;
        this.yOffset = yOffset;

        float vertexCount = (500f / (20f / 4.4f)) * (1/ speed); 

        this.vertexCount = (int)vertexCount;
        StartCoroutine(StartMoveSin());
    }

    IEnumerator StartMoveSin()
    {
        int index = 0;
        float offsetY = 0;

        var obj = new CreatBezier2D(this.gameObject, xOffset, yOffset, vertexCount);

        while (true)
        {

            index++;
            index = Mathf.Min(obj.PointList.Count - 1, index);
            if (index == obj.PointList.Count - 1)
            {
                offsetY += yOffset*2;
                index = 0;
            }
            this.transform.position = new Vector3(obj.PointList[index].x, obj.PointList[index].y + offsetY, this.transform.position.z);

            Vector3 target = new Vector3(obj.PointList[index + 1].x, obj.PointList[index + 1].y + offsetY, this.transform.position.z);

            var directionTo = (target - this.transform.position).normalized;

            this.transform.up = -directionTo;

            yield return null;
        }
    }
}
