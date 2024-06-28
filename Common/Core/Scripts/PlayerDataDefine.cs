using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;

public sealed class CheatableAttribute : Attribute
{
    public string displayName;

    public CheatableAttribute(string name)
    {
        displayName = name;
    }
}


public static class DebugDataManager
{
    public static Dictionary<string, DebugData> debugDataDic { get; private set; } = new Dictionary<string, DebugData>();

    public class DebugData
    {
        public string displayName;
        public PlayerPrefsData data;
        public bool isTrigger;
        public string content;

        public DebugData(string displayName, object data)
        {
            this.displayName = displayName;
            this.data = (PlayerPrefsData)data;
            content = data.ToString();
        }

        public void SaveValue()
        {
            data.ResetValue(content);
        }

        public override string ToString()
        {
            return data.ToString();
        }
    }

    public static void Init()
    {
        debugDataDic.Clear();

        //1.遍历运行程序集下的所有类 
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();
        //2.看是否包含这个特性，如果有就获取了这个类型 
        foreach (var type in types)
        {
            if(type.IsDefined(typeof(CheatableAttribute), false))
            {
                PropertyInfo[] propertyinfos = type.GetProperties();
                foreach (var item in propertyinfos)
                {
                    if (item.IsDefined(typeof(CheatableAttribute)))
                    {
                        if (item.PropertyType.IsSubclassOf(typeof(PlayerPrefsData)))
                        {
                            string displayName = item.GetCustomAttribute<CheatableAttribute>().displayName;
                            DebugData debugData = new DebugData(displayName, item.GetValue(null));
                            debugDataDic.Add(displayName, debugData);
                        }
                    }
                }
            }
        }

        
    }

    public static void Draw()
    {
        foreach (var debugData in debugDataDic.Values)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(debugData.displayName);
            GUILayout.Label(debugData.ToString());
            debugData.content = GUILayout.TextField(debugData.content);
            if (GUILayout.Button("修改"))
            {
                debugData.SaveValue();
            }
            GUILayout.EndHorizontal();

        }
    }
}


public abstract class PlayerPrefsData
{
    protected string key;
    protected object defaultValue;

    public PlayerPrefsData(string key, object defaultValue)
    {
        this.key = key;
        this.defaultValue = defaultValue;
    }

    public abstract void ResetValue(string obj);
}

public class BoolData : PlayerPrefsData
{
    public bool Value
    {
        get
        {
            return PlayerPrefs.GetInt(key, (bool)defaultValue ? 1 : 0) > 0;
        }
        set
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }
    }

    public BoolData(string key, bool defaultValue = false) : base(key, defaultValue)
    {
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public override void ResetValue(string obj)
    {
        Value = obj != "0";
    }
}

public class IntData : PlayerPrefsData
{
    public int Value
    {
        get
        {
            return PlayerPrefs.GetInt(key, (int)defaultValue);
        }
        set
        {
            PlayerPrefs.SetInt(key, value);
        }
    }

    public IntData(string key, int defaultValue = 0) : base(key, defaultValue)
    {
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public override void ResetValue(string obj)
    {
        Value = int.Parse(obj);
    }
}

public class LongData : PlayerPrefsData
{
    public long Value
    {
        get
        {
            return long.Parse(PlayerPrefs.GetString(key, defaultValue.ToString()));
        }
        set
        {
            PlayerPrefs.SetString(key, value.ToString());
        }
    }

    public LongData(string key, long defaultValue = 0) : base(key, defaultValue)
    {
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public override void ResetValue(string obj)
    {
        Value = long.Parse(obj);
    }
}



public class FloatData : PlayerPrefsData
{
    public float Value
    {
        get
        {
            return PlayerPrefs.GetFloat(key, (float)defaultValue);
        }
        set
        {
            PlayerPrefs.SetFloat(key, value);
        }
    }

    public FloatData(string key, float defaultValue = 0) : base(key, defaultValue)
    {
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public override void ResetValue(string obj)
    {
        Value = float.Parse(obj);
    }
}

public class StringData : PlayerPrefsData
{
    public string Value
    {
        get
        {
            return PlayerPrefs.GetString(key, (string)defaultValue);
        }
        set
        {
            PlayerPrefs.SetString(key, value);
        }
    }

    public StringData(string key, string defaultValue = "") : base(key, defaultValue)
    {
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public override void ResetValue(string obj)
    {
        Value = obj;
    }
}

public class DateTimeData : PlayerPrefsData
{
    public DateTime Value
    {
        get
        {
            return DateTime.Parse(PlayerPrefs.GetString(key, DateTime.MinValue.ToString(CultureInfo.InvariantCulture)), CultureInfo.InvariantCulture);
        }
        set
        {
            PlayerPrefs.SetString(key, value.ToString(CultureInfo.InvariantCulture));
        }
    }

    public DateTimeData(string key, string defaultValue = "") : base(key, defaultValue)
    {
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }

    public override void ResetValue(string obj)
    {
        Value = DateTime.Parse(obj, CultureInfo.InvariantCulture);
    }
}



public class ListData<T> : PlayerPrefsData
{
    public int Count { get { return Value.Count; } }
    public List<T> Value
    {
        get
        {
            string str = PlayerPrefs.GetString(key);
            if (str == "")
            {
                if (defaultValue != null)
                {
                    Value = (List<T>)defaultValue;
                }
                else
                {
                    Value = new List<T>();
                }
                str = PlayerPrefs.GetString(key);
            }
            Serialization<T> zz = JsonUtility.FromJson<Serialization<T>>(str);
            return zz.ToList();
        }
        private set
        {
            string str = JsonUtility.ToJson(new Serialization<T>(value));
            PlayerPrefs.SetString(key, str);
        }
    }

    public ListData(string key, List<T> dic = null) : base(key, dic)
    {

    }

    public bool ContainsKey(T t)
    {
        return Value.Contains(t);
    }

    public void Add(T t)
    {
        List<T> temp = Value;
        temp.Add(t);
        Value = temp;
    }

    public void Remove(T t)
    {
        List<T> temp = Value;
        temp.Remove(t);
        Value = temp;
    }

    public void Clear()
    {
        List<T> temp = Value;
        temp.Clear();
        Value = temp;
    }

    public T this[int i]
    {
        get
        {
            return Value[i];
        }
        set
        {
            List<T> temp = Value;
            temp[i] = value;
            Value = temp;
        }
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(new Serialization<T>(Value));
    }

    public override void ResetValue(string obj)
    {
        PlayerPrefs.SetString(key, obj);
    }
}

public class DictionaryData<TKey, TValue> : PlayerPrefsData, ICollection<KeyValuePair<TKey, TValue>>
{
    public int Count { get { return Value.Count; } }
    public Dictionary<TKey, TValue>.ValueCollection Values => Value.Values;
    public Dictionary<TKey, TValue>.KeyCollection Keys => Value.Keys;
    public Dictionary<TKey, TValue> Value
    {
        get
        {
            string str = PlayerPrefs.GetString(key);
            if (str == "")
            {
                if (defaultValue != null)
                {
                    Value = (Dictionary<TKey, TValue>)defaultValue;
                }
                else
                {
                    Value = new Dictionary<TKey, TValue>();
                }
                str = PlayerPrefs.GetString(key);
            }
            return (JsonUtility.FromJson<Serialization<TKey, TValue>>(str)).ToDictionary();
        }
        private set
        {
            string str = JsonUtility.ToJson(new Serialization<TKey, TValue>(value));
            PlayerPrefs.SetString(key, str);
        }
    }

    public bool IsReadOnly => false;

    public DictionaryData(string key, Dictionary<TKey, TValue> dic = null) : base(key, dic)
    {
    }

    public bool ContainsKey(TKey key)
    {
        return Value.ContainsKey(key);
    }

    public bool ContainsValue(TValue value)
    {
        return Value.ContainsValue(value);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return Value.TryGetValue(key, out value);
    }

    public void Add(TKey key, TValue value)
    {
        Dictionary<TKey, TValue> temp = Value;
        temp.Add(key, value);
        Value = temp;
    }

    public void Remove(TKey key)
    {
        Dictionary<TKey, TValue> temp = Value;
        temp.Remove(key);
        Value = temp;
    }

    public TValue this [TKey key]
    {
        get
        {
            return Value[key];
        }
        set
        {
            Remove(key);
            Add(key, value);
        }
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(new Serialization<TKey, TValue>(Value));
    }

    public override void ResetValue(string obj)
    {
        PlayerPrefs.SetString(key, obj);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Dictionary<TKey, TValue> temp = Value;
        temp.Add(item.Key, item.Value);
        Value = temp;
    }

    public void Clear()
    {
        Dictionary<TKey, TValue> temp = Value;
        temp.Clear();
        Value = temp;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return Value.ContainsKey(item.Key) && Value[item.Key].Equals(item.Value);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        Dictionary<TKey, TValue> temp = Value;
        foreach (var item in array)
        {
            temp.Add(item.Key, item.Value);
        }
        Value = temp;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        Dictionary<TKey, TValue> temp = Value;
        if (temp.ContainsKey(item.Key) && temp[item.Key].Equals(item.Value))
        {
            temp.Remove(item.Key);
            Value = temp;
            return true;
        }
        else return false;
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return Value.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Value.GetEnumerator();
    }
}

