using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatBezier
{
    private int vertexCount;

    private List<Vector3> positions;

    private List<Vector3> pointList;

    public List<Vector3> PointList { get => pointList; }

    /// <summary>
    /// 创造线
    /// </summary>
    /// <param name="obj">物体</param>
    /// <param name="xOffset">X轴偏移</param>
    /// <param name="zOffset">Z轴偏移</param>
    /// <param name="vertexCount">插值 （越大精度越高）</param>
    public CreatBezier(GameObject obj,float xOffset,float zOffset,int vertexCount)
    {
        pointList = new List<Vector3>();
        positions = new List<Vector3>();
        this.vertexCount = vertexCount;

        positions.Add(obj.transform.position);
        positions.Add(new Vector3(obj.transform.position.x + xOffset, obj.transform.position.y, obj.transform.position.z + zOffset));
        positions.Add(new Vector3(obj.transform.position.x - xOffset, obj.transform.position.y, obj.transform.position.z + zOffset));
        positions.Add(new Vector3(obj.transform.position.x , obj.transform.position.y, obj.transform.position.z + zOffset * 2));
    
        BezierCurveWithUnlimitPoints();
    }


    public void BezierCurveWithUnlimitPoints()
    {
        pointList.Clear();
        for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount)
        {
            pointList.Add(UnlimitBezierCurve(positions, ratio));
        }
        pointList.Add(positions[positions.Count - 1]);
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
