using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ARelationshipB
{
    /// <summary>
    /// A是否在B的前面
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <returns></returns>
    public static bool AIsForwardB(Transform A, Transform B, out float dis)
    {
        var _self = B;

        var _target = A;

        var _dot = Vector3.Dot(_self.forward, _target.position - _self.position);

        dis = _dot;

        if (_dot >= 0)
            return true;
        else 
            return false;
    }

    /// <summary>
    /// A是否在B的右面
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <returns></returns>
    public static bool AIsRightB(Transform A, Transform B, out float dis)
    {
        var _self = B;

        var _target = A;

        var _cross = Vector3.Cross(_self.forward, _target.position - _self.position);

        dis = _cross.y;

        if (_cross.y >= 0)
            return true;
        else
            return false;
    }

    /// <summary>
    /// A是否在B的上面
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <returns></returns>
    public static bool AIsTopB(Transform A, Transform B, out float dis)
    {
        var _self = B;

        var _target = A;

        var _cross = Vector3.Cross(_self.forward, _target.position - _self.position);

        dis = _cross.x * -1f;

        if (_cross.x <= 0)
            return true;
        else
            return false;
    }
}
