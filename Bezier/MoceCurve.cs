using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoceCurve : MonoBehaviour
{
    int vertexCount;
    int xOffset;
    int zOffset;
    /// <summary>
    /// 开始Sin形式移动
    /// </summary>
    /// <param name="vertexCount">精度（越高速度越慢）</param>
    /// <param name="xOffset">X轴偏移</param>
    /// <param name="zOffset">Z轴偏移</param>
    public void StartSinMove(int xOffset, int zOffset,int vertexCount)
    {
        this.xOffset = xOffset;
        this.zOffset = zOffset;

        this.vertexCount = vertexCount;
        StartCoroutine(StartMoveSin());
    }

    IEnumerator StartMoveSin()
    {
        int index = 0;
        float offsetZ = 0;

        var obj = new CreatBezier(this.gameObject, xOffset, zOffset, vertexCount);

        while (true)
        {
            
            index++;
            index = Mathf.Min(obj.PointList.Count - 1, index);
            if (index == obj.PointList.Count - 1)
            {
                offsetZ += zOffset*2;
                index = 0;
            }
            this.transform.position = new Vector3(obj.PointList[index].x, this.transform.position.y, obj.PointList[index].z + offsetZ);
            this.transform.LookAt(new Vector3(obj.PointList[index+1].x, this.transform.position.y, obj.PointList[index + 1].z + offsetZ ));
            yield return null;
        }
    }
}
