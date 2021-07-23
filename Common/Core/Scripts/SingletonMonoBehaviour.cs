using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单例类
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonMonoBehaviour<T> : OptimizedMonoBehaviour where T : Component
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject(typeof(T).Name).AddComponent<T>();
            }
            return _instance;
        }
    }

    protected void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
    }

}
