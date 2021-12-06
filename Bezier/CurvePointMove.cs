using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CurvePointMove : MonoBehaviour
{

    int vertexCount;
    List<Transform> objs;
    [SerializeField]
    Transform[] list;

    private void Start()
    {
        List<Transform> objs = new List<Transform>(list.ToArray());

        StartSinMove(objs,2.2f);
    }

    /// <summary>
    /// 开始移动
    /// </summary>
    /// <param name="objs">点阵组</param>
    /// <param name="vertexCount">（越高速度越慢）</param>

    public void StartSinMove(List<Transform> objs, int vertexCount)
    {
        this.objs = new List<Transform>(objs);
        this.vertexCount = vertexCount;
        StartCoroutine(StartMoveSin());
    }

    /// <summary>
    /// 开始移动
    /// </summary>
    /// <param name="objs">点阵组</param>
    /// <param name="speed">速度 默认0.2f</param>
    public void StartSinMove(List<Transform> objs, float speed = 0.2f)
    {
        this.objs = new List<Transform>(objs);
        float vertexCount = (500f / (20f / 4.4f)) * (1 / speed);

        this.vertexCount = (int)vertexCount;
        StartCoroutine(StartMoveSin());
    }

    IEnumerator StartMoveSin()
    {
        int index = 0;

        var bezier = new CreatCurve(objs, vertexCount);

        while (index <= bezier.PointList.Count - 1)
        {
            this.transform.position = bezier.PointList[index++];

            yield return null;
        }
    }
}
