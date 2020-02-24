using System.Collections.Generic;
using System.Data;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine.Events;

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

}


