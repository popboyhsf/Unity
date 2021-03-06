﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class CusEditor
{
    [MenuItem("CusEditor/GameData/ClearPlayerPrefs(清理玩家数据) &c")]
    public static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    [MenuItem("CusEditor/GameObject/SwithcGameObjectVisible(切换物体显隐状态) &q")]
    public static void SetObjActive()
    {
        GameObject[] selectObjs = Selection.gameObjects;
        int objCtn = selectObjs.Length;
        for (int i = 0; i < objCtn; i++)
        {
            bool isAcitve = selectObjs[i].activeSelf;
            selectObjs[i].SetActive(!isAcitve);
        }
    }

    /// <summary>
    /// 该方法能将Controller外部的 Animation Clip嵌入 Controller中
    /// 
    /// 注意:
    /// 1.若Animator内部已有Clip资源,则无效
    /// 2.执行成功后会删除外部Clip资源
    /// 
    /// 使用方法:
    /// 右键Controller选择CusTool/Nest AnimClips in Controller即可
    /// </summary>
    [MenuItem("Assets/CusEditor/Animation/Merge AnimClips to Controller(将AnimClips移到Animator Controller中)")]
    static public void MergeAnimClips()
    {
        UnityEditor.Animations.AnimatorController anim_controller = null;
        AnimationClip[] clips = null;

        if (Selection.activeObject.GetType() == typeof(UnityEditor.Animations.AnimatorController))
        {
            anim_controller = (UnityEditor.Animations.AnimatorController)Selection.activeObject;
            clips = anim_controller.animationClips;

            if (anim_controller != null && clips.Length > 0)
            {
                foreach (AnimationClip ac in clips)
                {
                    var acAssetPath = AssetDatabase.GetAssetPath(ac);
                    // Check if this ac is not in the controller
                    if (acAssetPath.EndsWith(".anim"))
                    {
                        var new_ac = UnityEngine.Object.Instantiate(ac) as AnimationClip;
                        new_ac.name = ac.name;

                        AssetDatabase.AddObjectToAsset(new_ac, anim_controller);
                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(new_ac));
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(ac));
                    }
                }
                Debug.Log("<color=orange>Added " + clips.Length.ToString() + " clips to controller: </color><color=yellow>" + anim_controller.name + "</color>");
            }
            else
            {
                Debug.Log("<color=red>Nothing done. Select a controller that has anim clips to nest.</color>");
            }
        }

    }

    #region FindReferences
    [MenuItem("Assets/CusEditor/Find References", false, 10)]
    static private void Find()
    {
        EditorSettings.serializationMode = SerializationMode.ForceText;
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!string.IsNullOrEmpty(path))
        {
            string guid = AssetDatabase.AssetPathToGUID(path);
            List<string> withoutExtensions = new List<string>() { ".prefab", ".unity", ".mat", ".asset" };
            string[] files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories)
                .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
            int startIndex = 0;
            EditorApplication.update = delegate ()
            {
                string file = files[startIndex];
                bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", file, (float)startIndex / (float)files.Length);
                if (Regex.IsMatch(File.ReadAllText(file), guid))
                {
                    Debug.Log(file, AssetDatabase.LoadAssetAtPath<Object>(GetRelativeAssetsPath(file)));
                }
                startIndex++;
                if (isCancel || startIndex >= files.Length)
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    startIndex = 0;
                    Debug.Log("匹配结束");
                }
            };
        }
    }

    [MenuItem("Assets/CusEditor/Find References", true)]
    static private bool VFind()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        return (!string.IsNullOrEmpty(path));
    }

    static private string GetRelativeAssetsPath(string path)
    {
        return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }
    #endregion

}
