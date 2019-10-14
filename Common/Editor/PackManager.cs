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
        if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android) Load2Android();
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS) Load2IOS();
    }

    private static BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;

    private static BuildTargetGroup buildTargetGroup;

    private static string companyName;

    private static string prodectName;

    private static string packName;

    private static AndroidSdkVersions targetSdkVersion;
    private static AndroidSdkVersions minSdkVersion;

    private static string targetOSVersion;
    private static iOSSdkVersion sdkVersion;
    private static iOSTargetDevice targetDevice;

    private static ScriptingRuntimeVersion scriptingRuntimeVersion;
    private static ScriptingImplementation scriptingBackend;
    private static Il2CppCompilerConfiguration il2CppCompilerConfiguration;

    private static int qualityLevel;

    private static string targetPatch;

    private static string script;

    private void OnGUI()
    {
        buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("项目环境：", buildTarget);

        switch (buildTarget)
        {
            case BuildTarget.StandaloneOSX:
                break;
            case BuildTarget.StandaloneWindows:
                break;
            case BuildTarget.iOS:
                buildTargetGroup = BuildTargetGroup.iOS;
                ShowIOS();
                break;
            case BuildTarget.Android:
                buildTargetGroup = BuildTargetGroup.Android;
                ShowAndroid();
                break;
            case BuildTarget.StandaloneLinux:
                break;
            case BuildTarget.StandaloneWindows64:
                break;
            case BuildTarget.WebGL:
                break;
            case BuildTarget.WSAPlayer:
                break;
            case BuildTarget.StandaloneLinux64:
                break;
            case BuildTarget.StandaloneLinuxUniversal:
                break;
            case BuildTarget.PS4:
                break;
            case BuildTarget.XboxOne:
                break;
            case BuildTarget.tvOS:
                break;
            case BuildTarget.Switch:
                break;
            case BuildTarget.Lumin:
                break;
            case BuildTarget.NoTarget:
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
        targetPatch = EditorGUILayout.TextField("项目路径:", targetPatch);

        qualityLevel = EditorGUILayout.IntField("图形质量：" + QualitySettings.names[qualityLevel] + "最大：" + (QualitySettings.names.Length-1) , qualityLevel);
        qualityLevel = qualityLevel <= QualitySettings.names.Length - 1 ? qualityLevel : QualitySettings.names.Length - 1;

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
    /// IOS部分对应显示
    /// </summary>
    private void ShowIOS()
    {

        EditorGUILayout.BeginVertical();

        prodectName = EditorGUILayout.TextField("项目名:", prodectName);
        packName = EditorGUILayout.TextField("包名:", packName);

        EditorGUILayout.LabelField("路径是否正确 : " + Directory.Exists(targetPatch));
        targetPatch = EditorGUILayout.TextField("项目路径:", targetPatch);

        
        EditorGUILayout.LabelField("IOS设置：");

        script = EditorGUILayout.TextField("项目注入:", script);
        targetOSVersion = EditorGUILayout.TextField("IOS版本支援：", targetOSVersion);
        sdkVersion = (iOSSdkVersion)EditorGUILayout.EnumPopup("目标SDK：", sdkVersion);
        targetDevice = (iOSTargetDevice)EditorGUILayout.EnumPopup("目标设备：", targetDevice);



        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("读取"))
        {
            Load2IOS();
            this.Repaint();
        }

        if (GUILayout.Button("写入"))
        {
            Write2IOS();
        }

        if (GUILayout.Button("打包（如有变更先写入）"))
        {
            var file = Build2IOS();
            if (file.Length > 0)
            {
                if (EditorUtility.DisplayDialog("PackManager", "导出成功,是否导入IOS工程", "Yep", "Closs"))
                {
                    var ass = targetPatch + @"\Classes";
                    var jl = targetPatch + @"\Data";
                    var lb = targetPatch + @"\Libraries";

                    var assnew = @"ForIOS\" + prodectName + @"\Classes";
                    var jlnew = @"ForIOS\" + prodectName + @"\Data";
                    var lbnew = @"ForIOS\" + prodectName + @"\Libraries";

                    try
                    {
                        if (Directory.Exists(ass) && Directory.Exists(jl) && Directory.Exists(lb))
                        {
                            Directory.Delete(ass, true);
                            Directory.Delete(jl, true);
                            Directory.Delete(lb, true);
                        }

                        Directory.Move(assnew, ass);
                        Directory.Move(jlnew, jl);
                        Directory.Move(lbnew, lb);

                        Directory.Delete("ForIOS", true);

                        var file01 = ass + @"\UnityAppController.h";
                        var file02 = ass + @"\UnityAppController.mm";

                        if (File.Exists(file01) && File.Exists(file02))
                        {
                            File.Delete(file01);
                            File.Delete(file02);
                        }

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

        targetPatch = PlayerPrefs.GetString("Editor_targetPatch" , "../../" + @"\Android\FishZoo\");

        packName = PlayerSettings.GetApplicationIdentifier(buildTargetGroup);

        script = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);

        targetSdkVersion = PlayerSettings.Android.targetSdkVersion;
        minSdkVersion = PlayerSettings.Android.minSdkVersion;

        scriptingRuntimeVersion = PlayerSettings.scriptingRuntimeVersion;
        scriptingBackend = PlayerSettings.GetScriptingBackend(buildTargetGroup);
        il2CppCompilerConfiguration = PlayerSettings.GetIl2CppCompilerConfiguration(buildTargetGroup);
    }
    private static void Load2IOS()
    {
        companyName = Application.companyName;
        prodectName = Application.productName;
        qualityLevel = QualitySettings.GetQualityLevel();

        targetPatch = PlayerPrefs.GetString("Editor_targetPatch", "../../" + @"\Android\FishZoo\");

        packName = PlayerSettings.GetApplicationIdentifier(buildTargetGroup);

        script = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);

        targetOSVersion = PlayerSettings.iOS.targetOSVersionString;
        targetDevice = PlayerSettings.iOS.targetDevice;
        sdkVersion = PlayerSettings.iOS.sdkVersion;
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
    private static void Write2IOS()
    {
        PlayerSettings.companyName = companyName;
        PlayerSettings.productName = prodectName;
        QualitySettings.SetQualityLevel(qualityLevel);

        PlayerPrefs.SetString("Editor_targetPatch", targetPatch);

        PlayerSettings.SetApplicationIdentifier(buildTargetGroup, packName);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, script);
        PlayerSettings.iOS.sdkVersion = sdkVersion;
        PlayerSettings.iOS.targetDevice = targetDevice;
        PlayerSettings.iOS.targetOSVersionString = targetOSVersion;



    }


    /// <summary>
    /// 调用系统打包
    /// </summary>
    /// <returns>打包完成后的文件</returns>
    private static UnityEditor.Build.Reporting.BuildFile[] Build2Android()
    {
        return BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "ForAndroid", BuildTarget.Android, BuildOptions.StrictMode).files;
    }
    private static UnityEditor.Build.Reporting.BuildFile[] Build2IOS()
    {
        return BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, @"ForIOS\" + prodectName, BuildTarget.iOS, BuildOptions.StrictMode).files;
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
