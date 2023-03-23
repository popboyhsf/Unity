using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ABManager : EditorWindow
{
    [MenuItem("Tools/YuanJi/ABManager", false, 0)]
    private static void AddWindow()
    {
        Rect wr = new Rect(0, 0, 400, 150);
        ABManager window = (ABManager)EditorWindow.GetWindowWithRect(typeof(ABManager), wr, true, "ABManager");
        window.Show();
    }


    private enum encryption
    {
        NONE,
        FirstOrderInverse,
        AES,
    }

    private encryption Encryption = encryption.AES;

    private string ABName = "headimg.abc";

    private string PackPatch = "UnkonwPack";

    private string PackName = "wibng.IS"; //TODO 

#if UNITY_STANDALONE_WIN
    private BuildTarget buildTarget = BuildTarget.StandaloneWindows;
#elif UNITY_ANDROID
    private BuildTarget buildTarget = BuildTarget.Android;
#elif UNITY_IPHONE
    private BuildTarget buildTarget = BuildTarget.iOS;
#endif

    private Object[] SelectObjs;


    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        SelectObjs = Selection.objects;

        EditorGUILayout.LabelField("选择的合法物体数量为：" + SelectObjs.Length);



        ABName = EditorGUILayout.TextField("AB包名字:", ABName);

        PackPatch = EditorGUILayout.TextField("转换包地址:", PackPatch);
        PackName = EditorGUILayout.TextField("转换包名字:", PackName);

        Encryption = (encryption)EditorGUILayout.EnumPopup("加密方式：", Encryption);

        buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("AB包环境：", buildTarget);



        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("开始作业"))
        {
            BuildAllAssetBundles();
        }

        if (GUILayout.Button("关闭"))
        {
            this.Close();
        }
        if (GUILayout.Button("清理全部AB相关"))
        {
            Delete();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        EditorGUILayout.LabelField("Power By YuanJI");
    }

    private void OnSelectionChange()
    {
        this.Repaint();
    }

    void BuildAllAssetBundles()
    {
        if (SelectObjs.Length <= 0)
        {
            if (EditorUtility.DisplayDialog("ABManager", "打包失败：请选取至少一个物体！", "OK"))
            {
                return;
            }
        }

        var ABNameList = ABName.Split('.');

        string _url = Application.streamingAssetsPath + @"/AB";
        string dir = _url;
        if (Directory.Exists(dir) == false)
        {
            Directory.CreateDirectory(dir);
        }

        string _newUrl = Application.streamingAssetsPath + @"/" + PackPatch;
        if (Directory.Exists(_newUrl) == false)
        {
            Directory.CreateDirectory(_newUrl);
        }

        for (int i = 0; i < SelectObjs.Length; i++)
        {
            AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(SelectObjs[i])).assetBundleName = ABNameList[0];
            AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(SelectObjs[i])).assetBundleVariant = ABNameList[1];
        }



        var ab = BuildPipeline.BuildAssetBundles(_url, BuildAssetBundleOptions.None, buildTarget);

        if (!ab)
        {
            if (EditorUtility.DisplayDialog("ABManager", "打包失败：没有AB资源，是否删除错误目录？", "是", "否"))
            {
                goto A;
            }
            else
            {
                goto B;
            }

        }


        FileStream fs = new FileStream(_newUrl + @"/" + PackName, FileMode.Create);
        string _oldUrl = _url + @"/" + ABName;
        byte[] bytes = File.ReadAllBytes(_oldUrl);


        switch (Encryption)
        {
            case encryption.NONE:
                break;
            case encryption.FirstOrderInverse:
                bytes[0] = (byte)~bytes[0];
                break;
            case encryption.AES:
                bytes = Utils.AESEncrypt(bytes);
                break;
            default:
                break;
        }


        fs.Write(bytes, 0, bytes.Length);
        fs.Close();


    A: if (Directory.Exists(dir) == true)
        {
            DirectoryInfo file1 = new DirectoryInfo(dir);
            deleteDirs(file1);
            file1 = null;

        }

        if (ab)
            if (EditorUtility.DisplayDialog("ABManager", "打包成功", "OK"))
            {
                this.Close();
            }

        B: AssetDatabase.Refresh();
    }

    void Delete()
    {
        string _url = Application.streamingAssetsPath + @"/AB";
        DirectoryInfo file1 = new DirectoryInfo(_url);
        deleteDirs(file1);
        file1 = null;
        string _newUrl = Application.streamingAssetsPath + @"/" + PackPatch;
        DirectoryInfo file2 = new DirectoryInfo(_newUrl);
        deleteDirs(file2);
        file2 = null;
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 删除资源（包含目录）
    /// </summary>
    /// <param name="dirs">目录Info</param>
    static void deleteDirs(DirectoryInfo dirs)
    {
        if (dirs == null || (!dirs.Exists))
        {
            return;
        }

        DirectoryInfo[] subDir = dirs.GetDirectories();
        if (subDir != null)
        {
            for (int i = 0; i < subDir.Length; i++)
            {
                if (subDir[i] != null)
                {
                    deleteDirs(subDir[i]);
                }
            }
            subDir = null;
        }

        FileInfo[] files = dirs.GetFiles();
        if (files != null)
        {
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i] != null)
                {
                    Debug.Log("删除文件:" + files[i].FullName + "__over");
                    files[i].Delete();
                    files[i] = null;
                }
            }
            files = null;
        }

        Debug.Log("删除文件夹:" + dirs.FullName + "__over");
        dirs.Delete();
    }



}



