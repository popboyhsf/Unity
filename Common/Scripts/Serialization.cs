using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class Serialization<T>
{
    [SerializeField]
    List<T> values = new List<T>();
    public List<T> ToList() { return values; }

    public Serialization(List<T> values)
    {
        this.values = values;
    }
}

[Serializable]
public class Serialization<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField]
    List<TKey> keys = new List<TKey>();
    [SerializeField]
    List<TValue> values = new List<TValue>();

    Dictionary<TKey, TValue> target = new Dictionary<TKey, TValue>();
    public Dictionary<TKey, TValue> ToDictionary() { return target; }

    public Serialization(Dictionary<TKey, TValue> target)
    {
        this.target = target;
    }

    public void OnBeforeSerialize()
    {
        keys = new List<TKey>(target.Keys);
        values = new List<TValue>(target.Values);
    }

    public void OnAfterDeserialize()
    {
        var count = Math.Min(keys.Count, values.Count);
        target = new Dictionary<TKey, TValue>(count);
        for (var i = 0; i < count; ++i)
        {
            target.Add(keys[i], values[i]);
        }
    }
}