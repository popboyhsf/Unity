using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class AtlasReader : SingletonMonoBehaviour<AtlasReader>
{

    private Dictionary<string, SpriteAtlas> atlasDic = new Dictionary<string, SpriteAtlas>();

    private Dictionary<string, List<AtlasPackInfo>> packInfos = new Dictionary<string, List<AtlasPackInfo>>();
    public Sprite GetItemHead(string atlas, string name)
    {
        if (!atlasDic.ContainsKey(atlas))
        {
            var atlasString = atlas;
            var atlasSprits = Resources.Load<SpriteAtlas>(atlas);
            atlasDic.Add(atlasString, atlasSprits);
        }

        var spr = atlasDic[atlas].GetSprite(name);
        if (spr) return spr;
        else Debuger.LogWarning("Missing " + atlas + " Image Name === " + name);
        return null;
    }

    private void Start()
    {
        //InitSelf();
        //I2Language.Instance.onApplyLanguage += InitSelf;
    }

    /// <summary>
    /// 从AB包图集中读取图片
    /// </summary>
    /// <param name="_url">图集地址</param>
    /// <param name="_atlasName">图集名字</param>
    /// <param name="_name">图片名字</param>
    /// <returns></returns>
    public void GetGetItemHeadByABAES(AtlasPackInfo info)
    {
        if (!atlasDic.ContainsKey(info.atlasName))
        {
            //Debuger.LogWarning("Lost == " + info.atlasName);
            if (!packInfos.ContainsKey(info.atlasName))
            {
                packInfos.Add(info.atlasName, new List<AtlasPackInfo>());
                StartCoroutine(GetABByAESI(info));
            }
            packInfos[info.atlasName].Add(info);

            return;
        }

        var spr = atlasDic[info.atlasName].GetSprite(info.name);

        if (info.img)
            if (spr)
            {

                Debuger.Log("Spawn " + info.atlasName + " Image Name === " + info.name);
                info.img.sprite = spr;
            }
            else
            {
                Debuger.LogWarning("Missing " + info.atlasName + " Image Name === " + info.name);
                info.img.sprite = null;
            }

    }

    private IEnumerator GetABByAESI(AtlasPackInfo info)
    {
        var _url = info.url;
        var _atlasName = info.atlasName;
#if UNITY_EDITOR
        _url = Application.dataPath + "/StreamingAssets" + _url;
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

        var spriteAtlas = headsAB.LoadAsset<SpriteAtlas>(_atlasName);

        //AssetBundle.UnloadAllAssetBundles(false);
        headsAB.Unload(false);

        Debuger.Log("LoadABByAESOK  ==== " + _atlasName);

        if (!atlasDic.ContainsKey(_atlasName))
        {
            atlasDic.Add(_atlasName, spriteAtlas);
        }

        foreach (var item in packInfos[info.atlasName])
        {
            GetGetItemHeadByABAES(item);
        }

        packInfos[info.atlasName].Clear();

    }

}


public struct AtlasPackInfo
{
    public string url;
    public string atlasName;
    public string name;
    public Image img;

    public AtlasPackInfo(string url, string atlasName, string name, Image img)
    {
        this.url = url;
        this.atlasName = atlasName;
        this.name = name;
        this.img = img;
    }
}

/**
 *   public void LoadImg(string name,Image img)
    {

#if ENCRYPT
        var _atlasName = name.Split('/')[0];
        var _iconName = name.Split('/')[1];
        var _url = @"/" + path + @"/" + _atlasName;
        AtlasPackInfo _info = new AtlasPackInfo(_url, _atlasName, _iconName, img);
        AtlasReader.Instance.GetGetItemHeadByABAES(_info);

#else
        img.sprite = ResourceManager.LoadResource<Sprite>(path + "/" + name, true);
#endif



    }
 * **/
