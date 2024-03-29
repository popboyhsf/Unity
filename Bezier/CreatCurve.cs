﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatCurve
{
    private int vertexCount;
    private List<Vector3> pointList;

    public List<Vector3> PointList { get => pointList; }

    /// <summary>
    /// 创造线
    /// </summary>
    /// <param name="obj">物体</param>
    /// <param name="vertexCount">插值 （越大精度越高）</param>
    public CreatCurve(List<Transform> objs, int vertexCount)
    {
        pointList = new List<Vector3>();
        var positions = new List<Transform>(objs);
        this.vertexCount = vertexCount;
        BezierCurveWithUnlimitPoints(positions);
    }

    public CreatCurve(List<Vector3> objs, int vertexCount)
    {
        pointList = new List<Vector3>();
        var positionsV3 = new List<Vector3>(objs);
        this.vertexCount = vertexCount;
        BezierCurveWithUnlimitPoints(positionsV3);
    }

    public void BezierCurveWithUnlimitPoints(List<Transform> positions)
    {
        pointList.Clear();
        for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount)
        {
            pointList.Add(UnlimitBezierCurve(positions, ratio));
        }
        pointList.Add(positions[positions.Count - 1].position);
    }

    public void BezierCurveWithUnlimitPoints(List<Vector3> positions)
    {
        pointList.Clear();
        for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount)
        {
            pointList.Add(UnlimitBezierCurve(positions, ratio));
        }
        pointList.Add(positions[positions.Count - 1]);
    }

    
    public Vector3 UnlimitBezierCurve(List<Transform> trans, float t)
    {
        Vector3[] temp = new Vector3[trans.Count];
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = trans[i].position;
        }
        int n = temp.Length - 1;
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n - i; j++)
            {
                temp[j] = Vector3.Lerp(temp[j], temp[j + 1], t);
            }
        }
        return temp[0];
    }
    public Vector3 UnlimitBezierCurve(List<Vector3> trans, float t)
    {
        Vector3[] temp = new Vector3[trans.Count];
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = trans[i];
        }
        int n = temp.Length - 1;
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n - i; j++)
            {
                temp[j] = Vector3.Lerp(temp[j], temp[j + 1], t);
            }
        }
        return temp[0];
    }
}
