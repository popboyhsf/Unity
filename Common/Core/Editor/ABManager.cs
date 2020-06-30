using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ABManager : EditorWindow
{
    [MenuItem("Tools/YuanJi/ABManager")]
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
        FirstRemoval,
    }

    private encryption Encryption;

    private string ABName = "headimg.abc";

    private string PackPatch = "UnkonwPack";

    private string PackName = "HeadCode.YJ";

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

    [System.Obsolete]
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

        for (int i = 0; i < SelectObjs.Length; i++)
        {
            AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(SelectObjs[i])).assetBundleName = ABNameList[0];
            AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(SelectObjs[i])).assetBundleVariant = ABNameList[1];
        }


        string _url = Application.streamingAssetsPath + @"/AB";
        string dir = _url;
        if (Directory.Exists(dir) == false)
        {
            Directory.CreateDirectory(dir);
        }
        var ab = BuildPipeline.BuildAssetBundles(_url, BuildAssetBundleOptions.None, BuildTarget.Android);

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

        string _newUrl = Application.streamingAssetsPath +@"/"+ PackPatch;
        if (Directory.Exists(_newUrl) == false)
        {
            Directory.CreateDirectory(_newUrl);
        }
        FileStream fs = new FileStream(_newUrl + @"/" + PackName, FileMode.Create);
        string _oldUrl = @"file://" + _url +@"/"+ ABName;
        WWW www = new WWW(_oldUrl);
        byte[] bytes = www.bytes;

        #region OldCord
        //if (bytes.Length == 0)
        //{
        //    if (EditorUtility.DisplayDialog("ABManager", "打包失败：AB包名字错误，是否删除资源？", "是", "否"))
        //    {
        //        fs.Close();
        //        Delete();
        //        return;
        //    }
        //    else
        //    {
        //        goto B;
        //    }

        //}
        #endregion


        byte byteHead = (byte)' ';

        switch (Encryption)
        {
            case encryption.NONE:
                break;
            case encryption.FirstOrderInverse:
                bytes[0] = (byte)~bytes[0];
                break;
            case encryption.FirstRemoval:
                byteHead = bytes[0];
                break;
            default:
                break;
        }

        if (byteHead == bytes[0])
        {
            FileStream fs2 = new FileStream(_newUrl + @"/" + "EncryptionBpack", FileMode.Create);
            fs2.Write(bytes, 0, 1);
            fs2.Close();
            fs.Write(bytes, 1, bytes.Length-1);
            fs.Close();
            goto A;
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

