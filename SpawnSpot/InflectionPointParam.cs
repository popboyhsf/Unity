using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InflectionPointParam : MonoBehaviour
{
    private List<Vector3> objs;
    private int vertexCount;
    private CreatCurve tempCurve;

    /// <summary>
    /// 开始移动
    /// </summary>
    /// <param name="objs">点阵组</param>
    /// <param name="speed">速度 默认0.2f</param>
    public void StartSinMove(List<Vector3> objs,UnityAction callBack, float speed = 0.2f)
    {
        this.objs = new List<Vector3>(objs);
        float vertexCount = (500f / (20f / 4.4f)) * (1 / speed);
        this.vertexCount = (int)vertexCount;
        StartCoroutine(StartMoveSin(callBack));
    }

    private void MoveEnd(UnityAction callBack)
    {
        callBack?.Invoke();
        Destroy(this.gameObject);
    }

    private IEnumerator StartMoveSin(UnityAction callBack)
    {
        int index = 0;

        var bezier = tempCurve = new CreatCurve(objs, vertexCount);

        while (index <= bezier.PointList.Count - 1)
        {
            this.transform.position = bezier.PointList[index++];

            yield return null;
        }


        MoveEnd(callBack);


    }


    private void OnDrawGizmos()
    {
        #region 无限制顶点数

        Gizmos.color = Color.green;

        for (int i = 0; i < objs.Count - 1; i++)
        {
            Gizmos.DrawLine(objs[i], objs[i + 1]);
        }

        Gizmos.color = Color.red;

        Vector3[] temp = new Vector3[tempCurve.PointList.Count];
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = tempCurve.PointList[i];
        }
        int n = temp.Length - 1;
        for (float ratio = 0.5f / vertexCount; ratio < 1; ratio += 1.0f / vertexCount)
        {
            for (int i = 0; i < n - 2; i++)
            {
                Gizmos.DrawLine(Vector3.Lerp(temp[i], temp[i + 1], ratio), Vector3.Lerp(temp[i + 2], temp[i + 3], ratio));
            }

        }
        #endregion
    }
}
