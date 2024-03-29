﻿using System.IO;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class ConfigMenu
{
    public static void Load<T>() where T : ScriptableObject //SerializedScriptableObject
    {
        var type       = typeof(T);
        var configPath = $"Assets/DataBase/{type.Name}.asset";
        if (!File.Exists(configPath))
        {
            var obj = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(obj, configPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        var asset = AssetDatabase.LoadAssetAtPath(configPath, type);
        AssetDatabase.OpenAsset(asset);
    }

    [MenuItem("Tools/Config/GameConfig")]
    public static void LoadGameCfg()
    {
        Load<GameConfig>();
    }

    /**
     #if UNITY_EDITOR

    [Button("保存", ButtonSizes.Medium)]
    private void Save()
    {
        AssetDatabase.SaveAssets();
    }
    #endif
     */
}