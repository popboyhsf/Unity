using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class DateShowManager : EditorWindow
{
    [MenuItem("Tools/YuanJi/DateShowManager &m")]
    private static void AddWindow()
    {
        Rect wr = new Rect(0, 0, 450, 500);
        DateShowManager window = (DateShowManager)EditorWindow.GetWindowWithRect(typeof(DateShowManager), wr, true, "DateShowManager");
        window.Show();
        Init();
    }

    private void OnGUI()
    {
        Draw();

        EditorGUILayout.LabelField("Power By YuanJI");
    }

    private Vector2 _scrollLogView = Vector2.zero;
    private static Dictionary<string, DebugDataManager.DebugData> debugDataDic = new Dictionary<string, DebugDataManager.DebugData>();


    private static void Init()
    {
        foreach (var item in DebugDataManager.debugDataDic)
        {
            if(!debugDataDic.ContainsKey(item.Key)) 
                debugDataDic.Add(item.Key,item.Value);
        }
    }

    private void Draw()
    {
        _scrollLogView = GUILayout.BeginScrollView(_scrollLogView, "Box", GUILayout.Height(450));

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

        GUILayout.EndScrollView();
    }



}
