using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.Animations;

//namespace Core
//{
/// <summary>
/// 编辑器拓展工具类
/// </summary>
public class ToolsFuck
    {
    [MenuItem("Assets/Tools/RenameOrder", false, 100)]
    public static void RenameOrder()
    {
        Object[] objects = Selection.objects;
        List<Object> objectList = new List<Object>(objects);
        objectList.Sort((a, b) => { return (int.Parse(a.name) - int.Parse(b.name)); });
        for (int i = 0; i < objectList.Count; i++)
        {
            var acAssetPath = AssetDatabase.GetAssetPath(objectList[i]);
            AssetDatabase.RenameAsset(acAssetPath, (i + 1).ToString());
        }
    }

    [MenuItem("Assets/Tools/Animation/Separate")]
    public static void SeparateFromController()
    {
        if (Selection.activeObject)
        {
            AnimationClip clip = (AnimationClip)Selection.activeObject;
            if (clip)
            {
                string clipPath = AssetDatabase.GetAssetPath(clip);
                if (clipPath.EndsWith(".controller"))
                {
                    AnimatorController controller = (AnimatorController)AssetDatabase.LoadAssetAtPath(clipPath, typeof(AnimatorController));
                    string controllerPath = AssetDatabase.GetAssetPath(controller);
                    //寻找引用该Clip的State

                    string newPath = clipPath.Remove(clipPath.LastIndexOf("/") + 1);
                    newPath += clip.name + ".anim";
                    var newClip = Object.Instantiate(clip);
                    AssetDatabase.CreateAsset(newClip, newPath);
                    AssetDatabase.RemoveObjectFromAsset(clip);
                    GameObject.DestroyImmediate(clip);
                    AssetDatabase.SaveAssets();
                }
            }
        }
    }



    [MenuItem("Assets/Tools/EncryptName(加密资源名字)", false, 100)]
    public static void EncryptName()
    {
        Object[] objects = Selection.objects;
        for (int i = 0; i < objects.Length; i++)
        {
            var acAssetPath = AssetDatabase.GetAssetPath(objects[i]);

            string name = Utils.AESEncrypt(objects[i].name).Replace("/", "_");
            AssetDatabase.RenameAsset(acAssetPath, name);
        }
    }

    [MenuItem("Assets/Tools/DecryptName(解密资源名字)", false, 101)]
    public static void DecryptName()
    {
        Object[] objects = Selection.objects;
        for (int i = 0; i < objects.Length; i++)
        {
            var acAssetPath = AssetDatabase.GetAssetPath(objects[i]);

            string name = Utils.AESDecrypt(objects[i].name.Replace("_", "/"));
            Debug.Log(name);
            AssetDatabase.RenameAsset(acAssetPath, name);
        }
    }
    /// <summary>
    /// 将Controller外部的 Animation Clip嵌入 Controller中
    /// 
    /// 注意:
    /// 1.若Animator内部已有Clip资源,则无效
    /// 2.执行成功后会删除外部Clip资源
    /// 
    /// 使用方法:
    /// 右键Controller选择CusTool/Nest AnimClips in Controller即可
    /// </summary>
    [MenuItem("Assets/Tools/Animation/MergeAnimatorClipsToController", false, 1000)]
        public static void MergeAnimClips()
        {
            UnityEditor.Animations.AnimatorController animatorController = null;
            AnimationClip[] clips = null;

            if (Selection.activeObject.GetType() == typeof(UnityEditor.Animations.AnimatorController))
            {
                animatorController = (UnityEditor.Animations.AnimatorController)Selection.activeObject;
                clips = animatorController.animationClips;

                if (animatorController != null && clips.Length > 0)
                {
                    foreach (AnimationClip clip in clips)
                    {
                        var acAssetPath = AssetDatabase.GetAssetPath(clip);
                        if (acAssetPath.EndsWith(".anim"))
                        {
                            var newClip = UnityEngine.Object.Instantiate(clip) as AnimationClip;
                            newClip.name = clip.name;

                            AssetDatabase.AddObjectToAsset(newClip, animatorController);
                            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newClip));
                            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(clip));
                        }
                    }
                    Debuger.Log("<color=orange>Added " + clips.Length.ToString() + " clips to controller: </color><color=yellow>" + animatorController.name + "</color>");
                }
                else
                {
                    Debuger.Log("<color=red>Nothing done. Select a controller that has anim clips to nest.</color>");
                }
            }

        }

        //#region FindReferences
        //[MenuItem("Assets/Tools/Find References", false, 100)]
        //static private void Find()
        //{
        //    EditorSettings.serializationMode = SerializationMode.ForceText;
        //    string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        //    if (!string.IsNullOrEmpty(path))
        //    {
        //        string guid = AssetDatabase.AssetPathToGUID(path);
        //        List<string> withoutExtensions = new List<string>() { ".prefab", ".unity", ".mat", ".asset" };
        //        string[] files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories)
        //            .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
        //        int startIndex = 0;
        //        EditorApplication.update = delegate ()
        //        {
        //            string file = files[startIndex];
        //            bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", file, (float)startIndex / (float)files.Length);
        //            if (Regex.IsMatch(File.ReadAllText(file), guid))
        //            {
        //                Dbg.Log(file, AssetDatabase.LoadAssetAtPath<Object>(GetRelativeAssetsPath(file)));
        //            }
        //            startIndex++;
        //            if (isCancel || startIndex >= files.Length)
        //            {
        //                EditorUtility.ClearProgressBar();
        //                EditorApplication.update = null;
        //                startIndex = 0;
        //                Dbg.Log("匹配结束");
        //            }
        //        };
        //    }
        //}

        //[MenuItem("Assets/Tools/Find References", true)]
        //static private bool VFind()
        //{
        //    string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        //    return (!string.IsNullOrEmpty(path));
        //}

        //static private string GetRelativeAssetsPath(string path)
        //{
        //    return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
        //}
        //#endregion

    }

//}