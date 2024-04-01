using UnityEditor;
using UnityEngine;

public class StringAESWindow : EditorWindow
{
    [MenuItem("Tools/YuanJi/StringAESWindow", false, 1)]
    private static void AddWindow()
    {
        Rect wr = new Rect(0, 0, 400, 150);
        StringAESWindow window = (StringAESWindow)EditorWindow.GetWindowWithRect(typeof(StringAESWindow), wr, true, "ResAESManager");
        window.Show();
    }

    private string TargetString = "";
    private string AESString;

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();



        TargetString = EditorGUILayout.TextField("Target：", TargetString);

        AESString = EditorGUILayout.TextField("AES：", AESString);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("开始作业"))
        {
            BuildAllAssetBundles();
        }

        if (GUILayout.Button("关闭"))
        {
            this.Close();
        }


        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        EditorGUILayout.LabelField("Power By YuanJI");
    }

    void BuildAllAssetBundles()
    {



        AESString = Utils.AESEncrypt(TargetString);

    }
}
