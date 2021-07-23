using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public static class AtlasReader
{

    private static Dictionary<string, SpriteAtlas> atlasDic = new Dictionary<string, SpriteAtlas>();

    public static Sprite GetIteHead(string atlas, string name)
    {
        if(!atlasDic.ContainsKey(atlas))
        {
            var atlasString = atlas;
            var atlasSprits = Resources.Load<SpriteAtlas>(atlas);
            atlasDic.Add(atlasString, atlasSprits);
        }

        var spr = atlasDic[atlas].GetSprite(name);
        if (spr) return spr;
        else Debuger.LogWarning("Missing "+ atlas + " Image Name === " + name);
        return null;
    }
}
