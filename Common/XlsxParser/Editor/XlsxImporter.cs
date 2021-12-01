using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine.Events;
using UnityEngine;

namespace XlsxParser
{
    //监听的后缀名
    [ScriptedImporter(1, "xlsx")]
    public class XlsxImporter : ScriptedImporter
    {
        //监听自定义资源导入
        public override void OnImportAsset(AssetImportContext ctx)
        {
            XlsxReader.Excute(ctx.assetPath);
        }
    }

    public class XlsxImporterEditer : EditorWindow
    {
        const string fullPath = "Assets/Designs/Xlsx" + "/";

        [MenuItem("Tools/YuanJi/XlsxImporter")]
        public static void ImportAsset()
        {
            //获取指定路径下面的所有资源文件  
            if (Directory.Exists(fullPath))
            {
                DirectoryInfo direction = new DirectoryInfo(fullPath);
                FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);


                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".xlsx"))
                    {
                        XlsxReader.Excute(files[i].FullName);
                        Debug.Log("重新生成："+ files[i].FullName);
                    }
                    
                }

                Debug.Log("重新根据xlsx文件生成Json结束");
            }

            
        }
    }
}


