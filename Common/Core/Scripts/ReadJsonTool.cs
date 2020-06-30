using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ReadJsonTool
{
    public static T GetJsonData<T>(string _name) where T : class
    {
        string json = Resources.Load<TextAsset>(_name).ToString();

        if (json.Length > 0)
        {
            var _js = JsonUtility.FromJson<T>(json);
            Debuger.Log("Load   "+ _js + " ==== " + "OK");
            return _js;
        }
        else
        {
            Debuger.Log("LoadJson ==== " + "NO File");
            return default(T);
        }
    }


}
