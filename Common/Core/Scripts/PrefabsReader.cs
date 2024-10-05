using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabsReader : SingletonMonoBehaviour<PrefabsReader>
{
    private Dictionary<string, GameObject> prebabsDic = new Dictionary<string, GameObject>();
    private Dictionary<string, List<PrefabPackInfo>> packInfos = new Dictionary<string, List<PrefabPackInfo>>();

    public void LoadPrefabAES(string name, Action<GameObject> callBack)
    {
        var _prefabName = name;

        var _url = @"/" + "Prefabs" + @"/" + "wibng.IS";
        PrefabPackInfo _info = new PrefabPackInfo(_url, _prefabName, callBack);
        GetPrefabByABAES(_info);
    }


    public void GetPrefabByABAES(PrefabPackInfo info)
    {
        if (!prebabsDic.ContainsKey(info.name))
        {
            //Debuger.LogWarning("Lost == " + info.pathName);
            if (!packInfos.ContainsKey(info.name))
            {
                packInfos.Add(info.name, new List<PrefabPackInfo>());
                StartCoroutine(GetABByAESI(info));
            }
            packInfos[info.name].Add(info);

            return;
        }

        var _obj = prebabsDic[info.name];

        if (_obj)
        {

            Debuger.Log("Spawn " + info.name + " Image Name === " + info.name);
            info.callBack?.Invoke(_obj);
        }
        else
        {
            Debuger.LogWarning("Missing " + info.name + " Image Name === " + info.name);

        }


    }

    private IEnumerator GetABByAESI(PrefabPackInfo info)
    {
        var _url = info.url;
        var _prefabName = info.name;
#if UNITY_EDITOR
        _url = Application.dataPath + "/StreamingAssets" + _url;
        _url = new Uri(_url, UriKind.Absolute).AbsoluteUri;
#elif UNITY_IPHONE
        _url = "file://"+Application.dataPath +"/Raw"+_url; 
#elif UNITY_ANDROID
        _url = Application.streamingAssetsPath + _url; 
#endif

        WWW www = new WWW(_url);

        yield return www;

        byte[] bytes = www.bytes;

        bytes = Utils.AESDecrypt(bytes);
        AssetBundle headsAB = AssetBundle.LoadFromMemory(bytes);

        GameObject _obj = headsAB.LoadAsset<GameObject>(_prefabName);

        //AssetBundle.UnloadAllAssetBundles(false);
        headsAB.Unload(false);

        Debuger.Log("LoadABByAESOK  ==== " + _prefabName);

        if (!prebabsDic.ContainsKey(_prefabName))
        {
            prebabsDic.Add(_prefabName, _obj);
        }

        foreach (var item in packInfos[info.name])
        {
            GetPrefabByABAES(item);
        }

        packInfos[info.name].Clear();

    }
}






public struct PrefabPackInfo
{
    public string url;
    public string name;
    public Action<GameObject> callBack;

    public PrefabPackInfo(string url, string name, Action<GameObject> callBack)
    {
        this.url = url;
        this.name = name;
        this.callBack = callBack;
    }
}
