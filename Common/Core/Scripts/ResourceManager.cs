using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.U2D;

public static class ResourceManager
{
    private static Dictionary<string, string> paths = new Dictionary<string, string>();

    //容器键值对集合
    private static Hashtable ht = new Hashtable();

    /// <summary>
    /// 调用资源（带对象缓冲技术）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <param name="isCatch"></param>
    /// <returns></returns>
    public static T LoadResource<T>(string path, bool isCatch) where T : UnityEngine.Object
    {
        //if(typeof(T) == typeof(SpriteAtlas))
        //{
        //    path = "Atlas\\" + path;
        //}
        if (ht.Contains(path))
        {
            return ht[path] as T;
        }

        T TResource = Resources.Load<T>(path);
        if (TResource == null)
        {
            Debuger.LogError("ResourceManager 提取的资源找不到，请检查。 path=" + path);
        }
        else if (isCatch)
        {
            ht.Add(path, TResource);
        }

        return TResource;
    }

    /// <summary>
    /// 调用资源（带对象缓冲技术）
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="isCatch"></param>
    /// <returns></returns>
    public static T LoadAsset<T>(string assetName, bool isCatch)
        where T : Object
    {
        string path;
        if (paths.ContainsKey(assetName))
        {
            path = paths[assetName];
        }
        else
        {
            path = assetName;
        }
        GameObject goObj = LoadResource<GameObject>(path, isCatch);
        GameObject goObjClone = GameObject.Instantiate<GameObject>(goObj);
        if (goObjClone == null)
        {
            Debuger.LogError("ResourceManager 克隆资源不成功，请检查。 path=" + assetName);
        }
        return goObjClone.GetComponent<T>();
    }

    public static T LoadAsset<T>(bool isCatch)
        where T : Object
    {
        return LoadAsset<T>(typeof(T).Name, isCatch);
    }
}

