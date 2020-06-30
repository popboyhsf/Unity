using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class JsonStructure<T> where T : class
{
    public List<T> data;
}

public static class JsonStructureLoader
{
    public static List<T> Load<T>(string fileName) where T : class
    {
        Type type = typeof(T);
        if (!type.IsDefined(typeof(System.SerializableAttribute), true))
        {
            Debuger.LogError(type.Name + " doesn't has the SerializableAttribute." +
                "please add it!");
        }
#if ENCRYPT
        string encrptName = Utils.AESEncrypt(fileName);
        TextAsset jsonText = Resources.Load<TextAsset>("Jsons\\" + encrptName);
        string decryptJson = Utils.AESDecrypt(jsonText.text);
#else
        string encrptName = fileName; 
        TextAsset jsonText = Resources.Load<TextAsset>("Jsons\\" + encrptName);
        string decryptJson = "";
        try
        {
            decryptJson = jsonText.text;
        }
        catch (NullReferenceException)
        {

            Debug.LogError("Json Read Name Is Null By " + encrptName);
            return null;
        }
#endif
        return JsonUtility.FromJson<JsonStructure<T>>(decryptJson).data;
    }

    public static List<T> Load<T>() where T : class
    {
        return Load<T>(typeof(T).Name);
    }

}
