using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class PackManager : EditorWindow
{
    [MenuItem("Tools/YuanJi/PackManager &b")]
    private static void AddWindow()
    {
        Rect wr = new Rect(0, 0, 450, 500);
        PackManager window = (PackManager)EditorWindow.GetWindowWithRect(typeof(PackManager), wr, true, "PackManager");
        window.Show();
        Load2Android();
    }

    private static BuildTargetGroup buildTargetGroup = BuildTargetGroup.Android;

    private static string companyName;

    private static string prodectName;

    private static string packName;

    private static AndroidSdkVersions targetSdkVersion;
    private static AndroidSdkVersions minSdkVersion;

    private static ScriptingRuntimeVersion scriptingRuntimeVersion;
    private static ScriptingImplementation scriptingBackend;
    private static Il2CppCompilerConfiguration il2CppCompilerConfiguration;

    private static int qualityLevel;

    private static string targetPatch;

    private static string script;

    private void OnGUI()
    {
        buildTargetGroup = (BuildTargetGroup)EditorGUILayout.EnumPopup("项目环境：", buildTargetGroup);

        switch (buildTargetGroup)
        {
            case BuildTargetGroup.Unknown:
                break;
            case BuildTargetGroup.Standalone:
                break;
            case BuildTargetGroup.iOS:
                break;
            case BuildTargetGroup.Android:
                ShowAndroid();
                break;
            case BuildTargetGroup.WebGL:
                break;
            case BuildTargetGroup.WSA:
                break;
            case BuildTargetGroup.PS4:
                break;
            case BuildTargetGroup.XboxOne:
                break;
            case BuildTargetGroup.tvOS:
                break;
            case BuildTargetGroup.Facebook:
                break;
            case BuildTargetGroup.Switch:
                break;
            case BuildTargetGroup.Lumin:
                break;
            default:
                break;
        }

    }

    /// <summary>
    /// Android部分对应显示
    /// </summary>
    private void ShowAndroid()
    {

        EditorGUILayout.BeginVertical();

        companyName = EditorGUILayout.TextField("公司名:", companyName);
        prodectName = EditorGUILayout.TextField("项目名:", prodectName);
        packName = EditorGUILayout.TextField("包名:", packName);

        EditorGUILayout.LabelField("路径是否正确 : " + Directory.Exists(targetPatch));
        targetPatch = EditorGUILayout.TextField("Android项目路径:", targetPatch);

        qualityLevel = EditorGUILayout.IntField("图形质量：" + QualitySettings.names[qualityLevel] + "最大：" + (QualitySettings.names.Length-1) , qualityLevel);
        qualityLevel = qualityLevel <= 5 ? qualityLevel : 5;

        EditorGUILayout.LabelField("Android设置：");

        script = EditorGUILayout.TextField("项目注入:", script);
        targetSdkVersion = (AndroidSdkVersions)EditorGUILayout.EnumPopup("目标SDK：", targetSdkVersion);
        minSdkVersion = (AndroidSdkVersions)EditorGUILayout.EnumPopup("最小SDK：", minSdkVersion);
        scriptingRuntimeVersion = (ScriptingRuntimeVersion)EditorGUILayout.EnumPopup(".Net环境：", scriptingRuntimeVersion);
        scriptingBackend = (ScriptingImplementation)EditorGUILayout.EnumPopup("压缩方式：", scriptingBackend);

        if (scriptingBackend == ScriptingImplementation.IL2CPP)
        {
            il2CppCompilerConfiguration = (Il2CppCompilerConfiguration)EditorGUILayout.EnumPopup("打包方式：", il2CppCompilerConfiguration);
        }

       

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("读取"))
        {
            Load2Android();
            this.Repaint();
        }

        if (GUILayout.Button("写入"))
        {
            Write2Android();
        }

        if (GUILayout.Button("打包（如有变更先写入）"))
        {
            var file = Build2Android();
            if (file.Length > 0)
            {
                if (EditorUtility.DisplayDialog("PackManager", "导出成功,是否导入Android工程", "Yep", "Closs"))
                {
                    var ass = targetPatch + @"\app\src\main\assets";
                    var jl = targetPatch + @"\app\src\main\jniLibs";

                    var assnew = @"ForAndroid\" + prodectName + @"\src\main\assets";
                    var jlnew = @"ForAndroid\" + prodectName + @"\src\main\jniLibs";

                    try
                    {
                        if (Directory.Exists(ass) && Directory.Exists(jl))
                        {
                            Directory.Delete(ass, true);
                            Directory.Delete(jl, true);
                        }

                        Directory.Move(assnew, ass);
                        Directory.Move(jlnew, jl);



                        Directory.Delete("ForAndroid", true);

                        Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
                        Type logEntries = assembly.GetType("UnityEditor.LogEntries");
                        MethodInfo clearConsoleMethod = logEntries.GetMethod("Clear");
                        clearConsoleMethod.Invoke(new object(), null);

                        Debug.Log("打包完成！");

                        this.Close();

                    }
                    catch (System.Exception e)
                    {

                        if (EditorUtility.DisplayDialog("PackManager", e.Message, "Closs"))
                        {
                            this.Close();
                        }
                    }
                }
                else
                {
                    this.Close();
                }
            }
        }

        if (GUILayout.Button("关闭"))
        {
            this.Close();
        }

        try
        {
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
        catch (Exception)
        {

            Debug.Log("Why is always consle ?");
        }

        EditorGUILayout.LabelField("Power By YuanJI");
    }

    /// <summary>
    /// 读取相关参数
    /// </summary>
    private static void Load2Android()
    {
        companyName = Application.companyName;
        prodectName = Application.productName;
        qualityLevel = QualitySettings.GetQualityLevel();

        targetPatch = PlayerPrefs.GetString("Editor_targetPatch" , "../../" + @"\Android\DrillMaster\");

        packName = PlayerSettings.GetApplicationIdentifier(buildTargetGroup);

        script = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);

        targetSdkVersion = PlayerSettings.Android.targetSdkVersion;
        minSdkVersion = PlayerSettings.Android.minSdkVersion;
        scriptingRuntimeVersion = PlayerSettings.scriptingRuntimeVersion;
        scriptingBackend = PlayerSettings.GetScriptingBackend(buildTargetGroup);
        il2CppCompilerConfiguration = PlayerSettings.GetIl2CppCompilerConfiguration(buildTargetGroup);
    }
    /// <summary>
    /// 写入相关数据
    /// </summary>
    private static void Write2Android()
    {
        PlayerSettings.companyName = companyName;
        PlayerSettings.productName = prodectName;
        QualitySettings.SetQualityLevel(qualityLevel);

        PlayerPrefs.SetString("Editor_targetPatch", targetPatch);

        PlayerSettings.SetApplicationIdentifier(buildTargetGroup,packName);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, script);
        PlayerSettings.Android.targetSdkVersion = targetSdkVersion;
        PlayerSettings.Android.minSdkVersion = minSdkVersion;
        PlayerSettings.scriptingRuntimeVersion = scriptingRuntimeVersion;
        PlayerSettings.SetScriptingBackend(buildTargetGroup, scriptingBackend);
        PlayerSettings.SetIl2CppCompilerConfiguration(buildTargetGroup, il2CppCompilerConfiguration);

        
    }

    /// <summary>
    /// 调用系统打包
    /// </summary>
    /// <returns>打包完成后的文件</returns>
    private static UnityEditor.Build.Reporting.BuildFile[] Build2Android()
    {
        return BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "ForAndroid", BuildTarget.Android, BuildOptions.StrictMode).files;
    }


    /// <summary>
    /// 外部调用函数
    /// </summary>
    public static void BuildProjectByBat()
    {
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "ForAndroid", BuildTarget.Android, BuildOptions.StrictMode);

        var ass = "../../" + @"\Android\DrillMaster\" + @"\app\src\main\assets";
        var jl = "../../" + @"\Android\DrillMaster\" + @"\app\src\main\jniLibs";

        var assnew = @"ForAndroid\" + Application.productName + @"\src\main\assets";
        var jlnew = @"ForAndroid\" + Application.productName + @"\src\main\jniLibs";

        if (Directory.Exists(ass) && Directory.Exists(jl))
        {
            Directory.Delete(ass, true);
            Directory.Delete(jl, true);
        }

        Directory.Move(assnew, ass);
        Directory.Move(jlnew, jl);
        Directory.Delete("ForAndroid", true);
        
    }
}
