using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class FindMiss : EditorWindow
{
    [MenuItem("Tools/FindMiss")]
    static void ShowWindow()
    {
        prefabs.Clear();
        refPaths.Clear();
        GetWindow<FindMiss>("FindMiss").Show();
    }

    static Dictionary<Object, List<Object>> prefabs = new Dictionary<Object, List<Object>>();
    static Dictionary<Object, List<string>> refPaths = new Dictionary<Object, List<string>>();

    //寻找missing的资源
    static void Find(int id)
    {
        prefabs.Clear();
        refPaths.Clear();

        string[] paths = AssetDatabase.GetAllAssetPaths();
        //加载所有prefab资源
        var gos = id == 0 ? 
            paths.Where(path => path.EndsWith("prefab")).Select(path => AssetDatabase.LoadAssetAtPath<GameObject>(path))
            :
            Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];

        foreach (var item in gos)
        {
            GameObject go = item as GameObject;
            if (go == null)
            {
                continue;
            }
            Component[] cps = go.GetComponentsInChildren<Component>(true);
            foreach (var cp in cps)
            {
                if (cp == null)  //组件为空
                {
                    if (!prefabs.ContainsKey(go)) prefabs.Add(go, new List<Object>());
                    prefabs[go].Add(cp);
                }
                else   //组件不为空
                {
                    SerializedObject so = new SerializedObject(cp);
                    SerializedProperty iterator = so.GetIterator();
                    //获取所有属性
                    while (iterator.NextVisible(true))
                    {
                        if (iterator.propertyType == SerializedPropertyType.ObjectReference)
                        {
                            //引用对象是null 并且 引用ID不是0 说明丢失了引用
                            if (iterator.objectReferenceValue == null && iterator.objectReferenceInstanceIDValue != 0)
                            {
                                if (!refPaths.ContainsKey(cp)) refPaths.Add(cp, new List<string>());
                                refPaths[cp].Add(iterator.propertyPath);

                                if (!prefabs.ContainsKey(go)) prefabs.Add(go, new List<Object>());
                                prefabs[go].Add(cp);
                            }

                        }
                    }
                }
            }
        }
    }

    //以下只是将查找结果显示
    private Vector3 scroll = Vector3.zero;
    private bool showAll = false;
    GUIStyle lossStyle = new GUIStyle();
    private void OnGUI()
    {
        lossStyle.normal.textColor = Color.red;
        scroll = EditorGUILayout.BeginScrollView(scroll);
        EditorGUILayout.BeginVertical();
        showAll = GUILayout.Toggle(showAll,"显示全部引用");
        foreach (var item in prefabs)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(item.Key, typeof(GameObject), true, GUILayout.Width(200));
            EditorGUILayout.BeginVertical();
            foreach (var cp in item.Value)
            {
                EditorGUILayout.BeginHorizontal();
                if (cp)
                {
                    if (!showAll) goto End;

                    EditorGUILayout.ObjectField(cp, cp.GetType(), true, GUILayout.Width(200));
                    if (refPaths.ContainsKey(cp))
                    {
                        
                        string missingPath = null;
                        foreach (var path in refPaths[cp])
                        {
                            missingPath += path + "|";
                        }
                        if (missingPath != null)
                            missingPath = missingPath.Substring(0, missingPath.Length - 1);
                        GUILayout.Label(missingPath);
                    }
                }
                else
                {
                    GUILayout.Label("丢失脚本！", lossStyle);
                }
                End: EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("FindPrefab"))
        {
            prefabs.Clear();
            refPaths.Clear();
            Find(0);
        }
        if (GUILayout.Button("FindScene"))
        {
            prefabs.Clear();
            refPaths.Clear();
            Find(1);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }
}
