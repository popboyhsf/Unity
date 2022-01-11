using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine.Events;


/// <summary>
/// 拓展类
/// </summary>
public static class Extensions
{
    #region 中文字符串
    /// <summary>
    /// 获取中英文混排字符串的实际长度(字节数)
    /// </summary>
    /// <param name="str">要获取长度的字符串</param>
    /// <returns>字符串的实际长度值（字节数）</returns>
    public static int LengthASCIIEncoding(this string str)
    {
        if (str.Equals(string.Empty))
            return 0;
        int strlen = 0;
        ASCIIEncoding strData = new ASCIIEncoding();
        //将字符串转换为ASCII编码的字节数字
        byte[] strBytes = strData.GetBytes(str);
        for (int i = 0; i <= strBytes.Length - 1; i++)
        {
            //中文都将编码为ASCII编码63,即"?"号
            if (strBytes[i] == 63)
                strlen += 2;
            strlen++;
        }
        return strlen;
    }


    /// <summary>截取指定字节长度的字符串</summary> 
    /// <param name="str">原字符串</param>
    ///<param name="len">截取字节长度</param> 
    /// <returns>string</returns>
    public static string SubASCIIEncodingString(this string str, int len)
    {
        string result = string.Empty;// 最终返回的结果
        if (string.IsNullOrEmpty(str))
        {
            return result;
        }
        int byteLen = System.Text.Encoding.Default.GetByteCount(str);
        // 单字节字符长度
        int charLen = str.Length;
        // 把字符平等对待时的字符串长度
        int byteCount = 0;
        // 记录读取进度 
        int pos = 0;
        // 记录截取位置 
        if (byteLen > len)
        {
            for (int i = 0; i < charLen; i++)
            {
                if (Convert.ToInt32(str.ToCharArray()[i]) > 255)
                // 按中文字符计算加 2 
                {
                    byteCount += 2;
                }
                else
                // 按英文字符计算加 1 
                {
                    byteCount += 1;
                }
                if (byteCount > len)
                // 超出时只记下上一个有效位置
                {
                    pos = i;
                    break;
                }
                else if (byteCount == len)// 记下当前位置
                {
                    pos = i + 1; break;
                }
            }
            if (pos >= 0)
            {
                result = str.Substring(0, pos);
            }
        }
        else { result = str; }
        return result;
    }
    #endregion

    #region Animator
    /// <summary>
    /// 是否包含该参数
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="parameterName"></param>
    /// <param name="parameterType"></param>
    /// <returns></returns>
    public static bool HasParameter(this Animator animator, string parameterName, AnimatorControllerParameterType parameterType)
    {
        AnimatorControllerParameter[] parameters = animator.parameters;
        if (parameters == null || parameters.Length < 1)
        {
            return false;
        }
        for (int i = 0; i < parameters.Length; i++)
        {
            if (parameters[i].type == parameterType
                && parameters[i].name.Equals(parameterName))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 获取某个动画片段的长度
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="clipName"></param>
    /// <returns></returns>
    public static float GetAnimationClipLength(this Animator animator, string clipName)
    {
        AnimationClip[] animationClips = animator.runtimeAnimatorController.animationClips;
        if (animationClips == null || animationClips.Length < 1)
        {
            return 0;
        }
        for (int i = 0; i < animationClips.Length; i++)
        {
            if (animationClips[i].name.Equals(clipName))
            {
                return animationClips[i].length;
            }
        }
        return 0;
    }

    /// <summary>
    /// 当前动画片段长度
    /// </summary>
    /// <param name="animator"></param>
    /// <returns></returns>
    public static float GetCurAnimationClipLength(this Animator animator)
    {
        AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return animatorStateInfo.length;
    }

    public static float GetNextAnimatorClipInfo(this Animator animator)
    {
        AnimatorStateInfo animatorStateInfo = animator.GetNextAnimatorStateInfo(0);
        return animatorStateInfo.length;
    }

    /// <summary>
    /// 按名称获取动画长度
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static float? GetAnimatorClipLengthWithName(this Animator animator, string name)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (var item in clips)
        {
            if (item.name == name || item.name.Equals(name))
            {
                return item.length;
            }
        }

        return null;

    }

    #endregion

    #region GameObject&Transform
    /// <summary>
    /// 递归查找查找子对象
    /// </summary>
    /// <param name="parent">父对象</param>
    /// <param name="childName">子对象名称</param>
    /// <returns></returns>
    public static Transform FindChildRecursive(this Transform parent, string childName)
    {
        Transform searchTrans = parent.Find(childName);
        if (searchTrans == null)
        {
            foreach (Transform trans in parent.transform)
            {
                searchTrans = trans.FindChildRecursive(childName);
                if (searchTrans != null)
                {
                    return searchTrans;
                }
            }
        }
        return searchTrans;
    }

    /// <summary>
    /// 递归查找查找子对象上的脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parent"></param>
    /// <param name="childName"></param>
    /// <returns></returns>
    public static T FindComponentRecursive<T>(this Transform parent, string childName)
    {
        return parent.FindChildRecursive(childName).GetComponent<T>();
    }

    /// <summary>
    /// 递归查找查找子对象上的脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parent"></param>
    /// <param name="childName"></param>
    /// <returns></returns>
    public static T FindComponentRecursive<T>(this GameObject parent, string childName)
    {
        return parent.transform.FindChildRecursive(childName).GetComponent<T>();
    }

    /// <summary>
    /// 获取子物体的脚本
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    /// <param name="parent">父对象</param>
    /// <param name="childName">子对象名称</param>
    /// <returns></returns>
    public static T GetComponentInChildren<T>(this Transform parent, string childName) where T : Component
    {
        Transform searchTrans = parent.FindChildRecursive(childName);
        if (searchTrans != null)
        {
            return searchTrans.gameObject.GetComponent<T>();
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 给子物体添加单脚本
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    /// <param name="parent">父对象</param>
    /// <param name="childName">子对象名称</param>
    /// <returns></returns>
    public static T AddSingleComponentToChild<T>(this Transform parent, string childName) where T : Component
    {
        Transform searchTrans = parent.FindChildRecursive(childName);
        if (searchTrans != null)
        {
            T[] theComponentsArr = searchTrans.GetComponents<T>();
            for (int i = 0; i < theComponentsArr.Length; i++)
            {
                if (theComponentsArr[i] != null)
                {
                    UnityEngine.Object.Destroy(theComponentsArr[i]);
                }
            }
            return searchTrans.gameObject.AddComponent<T>();
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 给子物体重置父对象
    /// </summary>
    /// <param name="parent">父对象的方位</param>
    /// <param name="child">子对象的方位</param>
    public static void ResetParent(this Transform child, Transform parent, bool normalize = false)
    {
        child.SetParent(parent, false);
        if (normalize)
        {
            child.localPosition = Vector3.zero;
            child.localScale = Vector3.one;
            child.localEulerAngles = Vector3.zero;
        }
    }

    public static void SetX(this Transform transform, float x)
    {
        Vector3 newPosition = new Vector3(x, transform.position.y, transform.position.z);
        transform.position = newPosition;
    }

    public static void SetY(this Transform transform, float y)
    {
        Vector3 newPosition = new Vector3(transform.position.x, y, transform.position.z);
        transform.position = newPosition;
    }

    public static void SetZ(this Transform transform, float z)
    {
        Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, z);
        transform.position = newPosition;
    }

    public static void SetPosition2D(this Transform transform, Vector3 target)
    {
        Vector3 newPostion = new Vector3(target.x, target.y, transform.position.z);
        transform.position = newPostion;
    }

    public static void SetLocalX(this Transform transform, float x)
    {
        Vector3 newPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
        transform.localPosition = newPosition;
    }

    public static void SetLocalY(this Transform transform, float y)
    {
        Vector3 newPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
        transform.localPosition = newPosition;
    }

    public static void SetLocalZ(this Transform transform, float z)
    {
        Vector3 newPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
        transform.localPosition = newPosition;
    }

    public static void MoveLocalX(this Transform transform, float deltaX)
    {
        Vector3 newPosition = new Vector3(transform.localPosition.x + deltaX, transform.localPosition.y, transform.localPosition.z);
        transform.localPosition = newPosition;
    }

    public static void MoveLocalY(this Transform transform, float deltaY)
    {
        Vector3 newPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + deltaY, transform.localPosition.z);
        transform.localPosition = newPosition;
    }

    public static void MoveLocalZ(this Transform transform, float deltaZ)
    {
        Vector3 newPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + deltaZ);
        transform.localPosition = newPosition;
    }

    public static void MoveLocalXYZ(this Transform transform, float deltaX, float deltaY, float deltaZ)
    {
        Vector3 newPosition = new Vector3(transform.localPosition.x + deltaX, transform.localPosition.y + deltaY, transform.localPosition.z + deltaZ);
        transform.localPosition = newPosition;
    }

    public static void SetLocalScaleX(this Transform transform, float x)
    {
        Vector3 newScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
        transform.localScale = newScale;
    }

    public static void SetLocalScaleY(this Transform transform, float y)
    {
        Vector3 newScale = new Vector3(transform.localScale.x, y, transform.localScale.z);
        transform.localScale = newScale;
    }

    public static void SetLocalScaleZ(this Transform transform, float z)
    {
        Vector3 newScale = new Vector3(transform.localScale.x, transform.localScale.y, z);
        transform.localScale = newScale;
    }

    public static void LookAt2D(this Transform transform, Vector3 target, float angle = 0)
    {
        Vector3 dir = target - transform.position;
        angle += Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
    }

    public static void LookAt2D(this Transform transform, Transform target, float angle = 0)
    {
        LookAt2D(transform, target.position, angle);
    }

    #endregion

    public static void AddListener(this Button btn, UnityAction onClick)
    {
        btn.onClick.AddListener(onClick);
    }

    public static float Delta(this float number, float delta)
    {
        return number + UnityEngine.Random.Range(-delta, delta);
    }

    public static float DeltaPercent(this float number, float percent)
    {
        return Delta(number, number * percent);
    }

    public static void Shuffle<T>(this IList<T> list, int? seed = null)
    {
        System.Random rng = seed != null ? new System.Random((int)seed) : new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static string ToPriceString(this long num)
    {
        if (num > 1000000000)
        {
            long head = num / 1000000000;
            long behind = (num % 1000000000) / 100000000;
            return head + "." + behind + "g";
        }
        else if (num > 1000000)
        {
            long head = num / 1000000;
            long behind = (num % 1000000) / 100000;
            return head + "." + behind + "m";
        }
        else if (num > 1000)
        {
            long head = num / 1000;
            long behind = (num % 1000) / 100;
            return head + "." + behind + "k";
        }
        else
        {
            return num.ToString();
        }
    }

    public static string ToCoinPrice(this long price)
    {
        return "c   " + price.ToString();
    }

    /// <summary>
    /// 为了方便统计打点后台的数据查看，格式化数字为特定的格式
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static string TOAFFormatStr(this int level)
    {
        string str = "null";
        if (level > 0)
        {
            if (level < 10)
            {
                str = "0" + level;
            }
            else if (level < 10000)
            {
                str = level.ToString();
            }
        }

        return str;
    }

    public static string ToCashString(this int cash)
    {
        return "$" + cash / 100 + "." + (cash % 100).ToString("D2");
    }

    public static float GetAngleBelow180(this float angle)
    {
        float belowAngle = angle > 180 ? angle - 360 : angle;
        return belowAngle;
    }

    public static string ToTimeString(this int num)
    {
        int hour = num / 3600;
        int minute = num / 60 % 60;
        int seconds = num % 60;
        return hour.ToString() + ":" + minute.ToString("D2") + ":" + seconds.ToString("D2");
    }

    public static string ToShortTimeString(this int num)
    {
        int minute = num / 60 % 60;
        int seconds = num % 60;
        return minute.ToString("D2") + ":" + seconds.ToString("D2");
    }

    public static string ToTimeString(this float num)
    {
        int intNum = (int)num;
        int hour = intNum / 3600;
        int minute = intNum / 60 % 60;
        int seconds = intNum % 60;
        int suffix = (int)((num - intNum) * 100);
        return hour.ToString() + ":" + minute.ToString("D2") + ":" + seconds.ToString("D2") /*+ "." + suffix.ToString("D2")*/;
    }

    public static string ToShortTimeString(this float num)
    {
        int intNum = (int)num;
        int minute = intNum / 60 % 60;
        int seconds = intNum % 60;
        int suffix = (int)((num - intNum) * 100);
        return minute.ToString("D2") + ":" + seconds.ToString("D2") /*+ "." + suffix.ToString("D2")*/;
    }

    public static string ToPercentage(this float num)
    {
        string prefix = Mathf.CeilToInt((num * 100)).ToString();
        int value = Mathf.CeilToInt((num * 10000));
        string subfix = (value % 100).ToString();
        return prefix + "." + subfix + "%";
    }

    public static string ToPercentageW(this int num)
    {
        string prefix = (num / 100).ToString();
        string subfix = num.ToString();
        if (num >= 100)
        {
            string numStr = num.ToString();
            subfix = numStr.Substring(numStr.Length - 2, 2);
        }
        return prefix + "." + subfix + "%";
    }

    public static string[] SplitChar(this string str, char c)
    {
        return str.Split(new char[] { c }, System.StringSplitOptions.RemoveEmptyEntries);
    }

    public static bool IntToBool(this int num)
    {
        if (num == 0) return false;
        else return true;
    }
    public static int BoolToInt(this bool num)
    {
        if (num) return 1;
        else return 0;
    }

}
