using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Profiling;
using UnityEngine.Events;
using System.Diagnostics;

public class DebugerManager : MonoBehaviour
{

    
    bool AllowDebugging = false;

    [SerializeField]
    DataKeyType[] PlayerPrefsKey;

    [SerializeField]
    List<DebugerStruct> DebugerBoolList;

    [SerializeField]
    UnityEvent CallChangeUI;

    private int _mapIndex;

    internal static readonly float DefaultWindowScale = 2.5f;

    private DebugType _debugType = DebugType.Data;
    private List<LogData> _logInformations = new List<LogData>();
    private int _currentLogIndex = -1;
    private int _infoLogCount = 0;
    private int _warningLogCount = 0;
    private int _errorLogCount = 0;
    private int _fatalLogCount = 0;
    private bool _showInfoLog = true;
    private bool _showWarningLog = true;
    private bool _showErrorLog = true;
    private bool _showFatalLog = true;
    private Vector2 _scrollLogView = Vector2.zero;
    private Vector2 _scrollCurrentLogView = Vector2.zero;
    private Vector2 _scrollSystemView = Vector2.zero;
    private Vector2 _scrollPlayerDataView = Vector2.zero;
    private bool _expansion = false;
    private Rect _windowRect = new Rect(0, 0, 100, 60);

    private int _fps = 0;
    private Color _fpsColor = Color.white;
    private int _frameNumber = 0;
    private float _lastShowFPSTime = 0f;
    private float m_WindowScale = DefaultWindowScale;


    
    private void Awake()
    {
        string filePath = $"{Application.persistentDataPath}/Test.txt";

        if (System.IO.File.Exists(filePath))
        {
            var io = System.IO.File.ReadAllLines(filePath);

            if (io[0] == "greedisgood")
            {
                AllowDebugging = true;
                Debuger.EnableLog = true;
            }
            else
            {
                AllowDebugging = false;
                Debuger.EnableLog = false;
                UnityEngine.Debug.LogWarning("World Is Not Good");
            }
        }
        else
        {
            AllowDebugging = false;
            Debuger.EnableLog = false;
            UnityEngine.Debug.LogWarning("DebugPatch ==== " + filePath);
        }

        DestroySelf();
        if(!AllowDebugging)
            Destroy(this.gameObject);
        
    }

    [Conditional("DEBUG")]
    private void DestroySelf()
    {
        AllowDebugging = true;
        Debuger.EnableLog = true;

        //TODO
    }

    private void Start()
    {
        if (AllowDebugging)
        {
            Application.logMessageReceived += LogHandler;
        }

    }
    private void Update()
    {
        if (AllowDebugging)
        {
            _frameNumber += 1;
            float time = Time.realtimeSinceStartup - _lastShowFPSTime;
            if (time >= 1)
            {
                _fps = (int)(_frameNumber / time);
                _frameNumber = 0;
                _lastShowFPSTime = Time.realtimeSinceStartup;
            }
        }
    }
    private void OnDestory()
    {
        if (AllowDebugging)
        {
            Application.logMessageReceived -= LogHandler;
        }
    }
    private void LogHandler(string condition, string stackTrace, LogType type)
    {
        LogData log = new LogData();
        log.time = DateTime.Now.ToString("HH:mm:ss");
        log.message = condition;
        log.stackTrace = stackTrace;

        if (type == LogType.Assert)
        {
            log.type = "Fatal";
            _fatalLogCount += 1;
        }
        else if (type == LogType.Exception || type == LogType.Error)
        {
            log.type = "Error";
            _errorLogCount += 1;
        }
        else if (type == LogType.Warning)
        {
            log.type = "Warning";
            _warningLogCount += 1;
        }
        else if (type == LogType.Log)
        {
            log.type = "Info";
            _infoLogCount += 1;
        }

        _logInformations.Add(log);

        if (_warningLogCount > 0)
        {
            _fpsColor = Color.yellow;
        }
        if (_errorLogCount > 0)
        {
            _fpsColor = Color.red;
        }
    }

    private void OnGUI()
    {
        m_WindowScale = DefaultWindowScale * Screen.width / 1080f;

        GUI.matrix = Matrix4x4.Scale(new Vector3(m_WindowScale, m_WindowScale, 1f));

        if (AllowDebugging)
        {
            if (_expansion)
            {
                _windowRect = GUI.Window(0, _windowRect, ExpansionGUIWindow, "DEBUGGER");
            }
            else
            {
                _windowRect = GUI.Window(0, _windowRect, ShrinkGUIWindow, "DEBUGGER");
            }
        }
    }
    private void ExpansionGUIWindow(int windowId)
    {
        GUI.DragWindow(new Rect(0, 0, 10000 , 20 ));

        #region title
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUI.contentColor = _fpsColor;
        if (GUILayout.Button("FPS:" + _fps, GUILayout.Height(30)))
        {
            _expansion = false;
            _windowRect.width = 100;
            _windowRect.height = 60;
        }
        GUI.contentColor = (_debugType == DebugType.Data ? Color.white : Color.gray);
        if (GUILayout.Button("数据", GUILayout.Height(30)))
        {
            _debugType = DebugType.Data;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUI.contentColor = (_debugType == DebugType.Console ? Color.white : Color.gray);
        if (GUILayout.Button("输出", GUILayout.Height(30)))
        {
            _debugType = DebugType.Console;
        }
        GUI.contentColor = (_debugType == DebugType.Memory ? Color.white : Color.gray);
        if (GUILayout.Button("内存", GUILayout.Height(30)))
        {
            _debugType = DebugType.Memory;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUI.contentColor = (_debugType == DebugType.System ? Color.white : Color.gray);
        if (GUILayout.Button("系统", GUILayout.Height(30)))
        {
            _debugType = DebugType.System;
        }
        GUI.contentColor = (_debugType == DebugType.Screen ? Color.white : Color.gray);
        if (GUILayout.Button("屏幕", GUILayout.Height(30)))
        {
            _debugType = DebugType.Screen;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUI.contentColor = (_debugType == DebugType.Quality ? Color.white : Color.gray);
        if (GUILayout.Button("画质", GUILayout.Height(30)))
        {
            _debugType = DebugType.Quality;
        }
        GUI.contentColor = (_debugType == DebugType.Environment ? Color.white : Color.gray);
        if (GUILayout.Button("环境", GUILayout.Height(30)))
        {
            _debugType = DebugType.Environment;
        }
        GUI.contentColor = (_debugType == DebugType.Debug ? Color.white : Color.gray);
        if (GUILayout.Button("调试", GUILayout.Height(30)))
        {
            _debugType = DebugType.Debug;
        }
        GUI.contentColor = Color.white;
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
        #endregion

        #region console
        if (_debugType == DebugType.Console)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear"))
            {
                _logInformations.Clear();
                _fatalLogCount = 0;
                _warningLogCount = 0;
                _errorLogCount = 0;
                _infoLogCount = 0;
                _currentLogIndex = -1;
                _fpsColor = Color.white;
            }
            GUI.contentColor = (_showInfoLog ? Color.white : Color.gray);
            _showInfoLog = GUILayout.Toggle(_showInfoLog, "Info [" + _infoLogCount + "]");
            GUI.contentColor = (_showWarningLog ? Color.white : Color.gray);
            _showWarningLog = GUILayout.Toggle(_showWarningLog, "Warning [" + _warningLogCount + "]");
            GUI.contentColor = (_showErrorLog ? Color.white : Color.gray);
            _showErrorLog = GUILayout.Toggle(_showErrorLog, "Error [" + _errorLogCount + "]");
            GUI.contentColor = (_showFatalLog ? Color.white : Color.gray);
            _showFatalLog = GUILayout.Toggle(_showFatalLog, "Fatal [" + _fatalLogCount + "]");
            GUI.contentColor = Color.white;
            GUILayout.EndHorizontal();

            _scrollLogView = GUILayout.BeginScrollView(_scrollLogView, "Box", GUILayout.Height(165));
            for (int i = 0; i < _logInformations.Count; i++)
            {
                bool show = false;
                Color color = Color.white;
                switch (_logInformations[i].type)
                {
                    case "Fatal":
                        show = _showFatalLog;
                        color = Color.red;
                        break;
                    case "Error":
                        show = _showErrorLog;
                        color = Color.red;
                        break;
                    case "Info":
                        show = _showInfoLog;
                        color = Color.white;
                        break;
                    case "Warning":
                        show = _showWarningLog;
                        color = Color.yellow;
                        break;
                    default:
                        break;
                }

                try
                {
                    if (show)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Toggle(_currentLogIndex == i, ""))
                        {
                            _currentLogIndex = i;
                        }
                        GUI.contentColor = color;
                        GUILayout.Label("[" + _logInformations[i].type + "] ");
                        GUILayout.Label("[" + _logInformations[i].time + "] ");
                        GUILayout.Label(_logInformations[i].message);
                        GUILayout.FlexibleSpace();
                        GUI.contentColor = Color.white;
                        GUILayout.EndHorizontal();
                    }
                }
                catch (Exception)
                {
                    _logInformations.Clear();
                    _fatalLogCount = 0;
                    _warningLogCount = 0;
                    _errorLogCount = 0;
                    _infoLogCount = 0;
                    _currentLogIndex = -1;
                    _fpsColor = Color.white;
                    show = false;
                    UnityEngine.Debug.LogError("ShowMessageError");
                }
            }
            GUILayout.EndScrollView();

            _scrollCurrentLogView = GUILayout.BeginScrollView(_scrollCurrentLogView, "Box", GUILayout.Height(100));
            if (_currentLogIndex != -1)
            {
                GUILayout.Label(_logInformations[_currentLogIndex].message + "\r\n\r\n" + _logInformations[_currentLogIndex].stackTrace);
            }
            GUILayout.EndScrollView();
        }
        #endregion

        #region memory
        else if (_debugType == DebugType.Memory)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Memory Information");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box");
#if UNITY_5
            GUILayout.Label("总内存：" + Profiler.GetTotalReservedMemory() / 1000000 + "MB");
            GUILayout.Label("已占用内存：" + Profiler.GetTotalAllocatedMemory() / 1000000 + "MB");
            GUILayout.Label("空闲中内存：" + Profiler.GetTotalUnusedReservedMemory() / 1000000 + "MB");
            GUILayout.Label("总Mono堆内存：" + Profiler.GetMonoHeapSize() / 1000000 + "MB");
            GUILayout.Label("已占用Mono堆内存：" + Profiler.GetMonoUsedSize() / 1000000 + "MB");
#endif
#if UNITY_7
            GUILayout.Label("总内存：" + Profiler.GetTotalReservedMemoryLong() / 1000000 + "MB");
            GUILayout.Label("已占用内存：" + Profiler.GetTotalAllocatedMemoryLong() / 1000000 + "MB");
            GUILayout.Label("空闲中内存：" + Profiler.GetTotalUnusedReservedMemoryLong() / 1000000 + "MB");
            GUILayout.Label("总Mono堆内存：" + Profiler.GetMonoHeapSizeLong() / 1000000 + "MB");
            GUILayout.Label("已占用Mono堆内存：" + Profiler.GetMonoUsedSizeLong() / 1000000 + "MB");
#endif
#if UNITY_2018
            GUILayout.Label("总内存：" + Profiler.GetTotalReservedMemoryLong() / 1000000 + "MB");
            GUILayout.Label("已占用内存：" + Profiler.GetTotalAllocatedMemoryLong() / 1000000 + "MB");
            GUILayout.Label("空闲中内存：" + Profiler.GetTotalUnusedReservedMemoryLong() / 1000000 + "MB");
            GUILayout.Label("总Mono堆内存：" + Profiler.GetMonoHeapSizeLong() / 1000000 + "MB");
            GUILayout.Label("已占用Mono堆内存：" + Profiler.GetMonoUsedSizeLong() / 1000000 + "MB");
#endif
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("卸载未使用的资源"))
            {
                Resources.UnloadUnusedAssets();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("使用GC垃圾回收"))
            {
                GC.Collect();
            }
            GUILayout.EndHorizontal();
        }
        #endregion

        #region system
        else if (_debugType == DebugType.System)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("System Information");
            GUILayout.EndHorizontal();

            _scrollSystemView = GUILayout.BeginScrollView(_scrollSystemView, "Box");
            GUILayout.Label("操作系统：" + SystemInfo.operatingSystem);
            GUILayout.Label("系统内存：" + SystemInfo.systemMemorySize + "MB");
            GUILayout.Label("处理器：" + SystemInfo.processorType);
            GUILayout.Label("处理器数量：" + SystemInfo.processorCount);
            GUILayout.Label("显卡：" + SystemInfo.graphicsDeviceName);
            GUILayout.Label("显卡类型：" + SystemInfo.graphicsDeviceType);
            GUILayout.Label("显存：" + SystemInfo.graphicsMemorySize + "MB");
            GUILayout.Label("显卡标识：" + SystemInfo.graphicsDeviceID);
            GUILayout.Label("显卡供应商：" + SystemInfo.graphicsDeviceVendor);
            GUILayout.Label("显卡供应商标识码：" + SystemInfo.graphicsDeviceVendorID);
            GUILayout.Label("设备模式：" + SystemInfo.deviceModel);
            GUILayout.Label("设备名称：" + SystemInfo.deviceName);
            GUILayout.Label("设备类型：" + SystemInfo.deviceType);
            GUILayout.Label("设备标识：" + SystemInfo.deviceUniqueIdentifier);
            GUILayout.EndScrollView();
        }
        #endregion

        #region screen
        else if (_debugType == DebugType.Screen)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Screen Information");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box");
            GUILayout.Label("DPI：" + Screen.dpi);
            GUILayout.Label("分辨率：" + Screen.currentResolution.ToString());
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("全屏"))
            {
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, !Screen.fullScreen);
            }
            GUILayout.EndHorizontal();
        }
        #endregion

        #region Quality
        else if (_debugType == DebugType.Quality)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Quality Information");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box");
            string value = "";
            if (QualitySettings.GetQualityLevel() == 0)
            {
                value = " [最低]";
            }
            else if (QualitySettings.GetQualityLevel() == QualitySettings.names.Length - 1)
            {
                value = " [最高]";
            }

            GUILayout.Label("图形质量：" + QualitySettings.names[QualitySettings.GetQualityLevel()] + value);
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("降低一级图形质量"))
            {
                QualitySettings.DecreaseLevel();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("提升一级图形质量"))
            {
                QualitySettings.IncreaseLevel();
            }
            GUILayout.EndHorizontal();
        }
        #endregion

        #region Environment
        else if (_debugType == DebugType.Environment)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Environment Information");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box");
            GUILayout.Label("项目名称：" + Application.productName);
#if UNITY_5
            GUILayout.Label("项目ID：" + Application.bundleIdentifier);
#endif
#if UNITY_7
            GUILayout.Label("项目ID：" + Application.identifier);
#endif
#if UNITY_2018
            GUILayout.Label("项目ID：" + Application.identifier);
#endif
            GUILayout.Label("项目版本：" + Application.version);
            GUILayout.Label("Unity版本：" + Application.unityVersion);
            GUILayout.Label("公司名称：" + Application.companyName);
            //GUILayout.Label("公司名称：" + Application);
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("退出程序"))
            {
                Application.Quit();
            }
            GUILayout.EndHorizontal();
        }
        #endregion

        #region PlayerData
        else if (_debugType == DebugType.Data)
        {
            _scrollPlayerDataView = GUILayout.BeginScrollView(_scrollPlayerDataView, "Box", GUILayout.Height(300));

            for (int i = 0; i < PlayerPrefsKey.Length; i++)
            {
                GUILayout.BeginHorizontal();
                if (PlayerPrefs.HasKey(PlayerPrefsKey[i].key))
                {
                    GUILayout.Label("数据（" + PlayerPrefsKey[i].key + "）：" + GetValueByType(PlayerPrefsKey[i].type, PlayerPrefsKey[i].key));
                    if (GUILayout.RepeatButton("增加"))
                    {
                        ChangeValueAdd(PlayerPrefsKey[i].type, PlayerPrefsKey[i].key);
                    }
                    if (GUILayout.Button("减少"))
                    {
                        ChangeValueDec(PlayerPrefsKey[i].type, PlayerPrefsKey[i].key);
                    }

                }
                else
                {
                    GUILayout.Label("不存在数据：" + PlayerPrefsKey[i].key);
                    if(PlayerPrefsKey[i].type != DataKeyTypeEnum.stringType)
                    if (GUILayout.Button("初始化"))
                    {
                        InitValue(PlayerPrefsKey[i].type, PlayerPrefsKey[i].key);
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            if (GUILayout.Button("刷新数据"))
            {
                CallChangeUI?.Invoke();
            }
            if (GUILayout.Button("清除数据"))
            {
                PlayerPrefs.DeleteAll();
            }
        }
        #endregion

        #region 调试
        else if (_debugType == DebugType.Debug)
        {
            GUILayout.BeginVertical("Box");

            if (DebugerBoolList.Count == 0)
            {
                var i = GameObject.FindObjectsOfType<GameObject>();
                foreach (var item in i)
                {
                    var j = item.gameObject.GetComponent<IDebuger>();
                    if (j != null)
                    {
                        DebugerBoolList.Add(new DebugerStruct(j.AllowName, item));
                    }
                }
            }

            foreach (var item in DebugerBoolList)
            {
                if(item.debugerObject == null) continue;
                var script = item.debugerObject.GetComponent<IDebuger>();
                if (script == null)
                {
                    UnityEngine.Debug.LogWarning("Not Found IDebuger From " + item.name);
                    continue;
                }

                script.AllowDebug = GUILayout.Toggle(script.AllowDebug, item.name);
            }
            GUILayout.EndVertical();

        }
        #endregion
    }
    private void ShrinkGUIWindow(int windowId)
    {
        GUI.DragWindow(new Rect(0, 0, 10000, 20));

        GUI.contentColor = _fpsColor;
        if (GUILayout.Button("FPS:" + _fps, GUILayout.Width(80), GUILayout.Height(30)))
        {
            _expansion = true;
            _windowRect.width = 360;
            _windowRect.height = 600;
        }
        GUI.contentColor = Color.white;
    }

    private void ChangeValueAdd(DataKeyTypeEnum _type,string _key)
    {
        switch (_type)
        {
            case DataKeyTypeEnum.stringType:
                var _tempString = PlayerPrefs.GetString(_key, "0");
                PlayerPrefs.SetString(_key, _tempString + "0");
                break;
            case DataKeyTypeEnum.floatType:
                var _tempFloat = PlayerPrefs.GetFloat(_key, 0);
                PlayerPrefs.SetFloat(_key, _tempFloat + 0.2f);
                break;
            case DataKeyTypeEnum.intType:
                var _tempInt = PlayerPrefs.GetInt(_key, 0);
                PlayerPrefs.SetInt(_key, _tempInt + 1);
                break;
            default:
                break;
        }
    }

    private void ChangeValueDec(DataKeyTypeEnum _type, string _key)
    {
        switch (_type)
        {
            case DataKeyTypeEnum.stringType:
                var _tempString = PlayerPrefs.GetString(_key, "0");
                PlayerPrefs.SetString(_key, _tempString.Remove(_tempString.Length - 1, 1));
                break;
            case DataKeyTypeEnum.floatType:
                var _tempFloat = PlayerPrefs.GetFloat(_key, 0);
                PlayerPrefs.SetFloat(_key, _tempFloat - 0.2f);
                break;
            case DataKeyTypeEnum.intType:
                var _tempInt = PlayerPrefs.GetInt(_key, 0);
                PlayerPrefs.SetInt(_key, _tempInt - 1);
                break;
            default:
                break;
        }
    }

    private string GetValueByType(DataKeyTypeEnum _type, string _key)
    {
        string _tempString = "";
        switch (_type)
        {
            case DataKeyTypeEnum.stringType:
                _tempString = PlayerPrefs.GetString(_key, "0");
                break;
            case DataKeyTypeEnum.floatType:
                _tempString = PlayerPrefs.GetFloat(_key, 0).ToString();
                break;
            case DataKeyTypeEnum.intType:
                _tempString = PlayerPrefs.GetInt(_key, 0).ToString();
                break;
            default:
                _tempString = "NullType";
                break;
        }
        return _tempString;
    }

    private void InitValue(DataKeyTypeEnum _type, string _key)
    {
        switch (_type)
        {
            case DataKeyTypeEnum.stringType:
                var _tempString = PlayerPrefs.GetString(_key, "0");
                PlayerPrefs.SetString(_key, "");
                break;
            case DataKeyTypeEnum.floatType:
                var _tempFloat = PlayerPrefs.GetFloat(_key, 0);
                PlayerPrefs.SetFloat(_key, 0);
                break;
            case DataKeyTypeEnum.intType:
                var _tempInt = PlayerPrefs.GetInt(_key, 0);
                PlayerPrefs.SetInt(_key, 0);
                break;
            default:
                break;
        }
    }
}
public struct LogData
{
    public string time;
    public string type;
    public string message;
    public string stackTrace;
}
public enum DebugType
{
    Console,
    Data,
    Memory,
    System,
    Screen,
    Quality,
    Environment,
    Debug,
}

[Serializable]
public struct DataKeyType
{
    public string key;
    public DataKeyTypeEnum type;
}

[Serializable]
public enum DataKeyTypeEnum
{
    stringType,
    floatType,
    intType,
}

[Serializable]
public struct DebugerStruct
{
    public string name;
    public GameObject debugerObject;

    public DebugerStruct(string name, GameObject debugerObject)
    {
        this.name = name;
        this.debugerObject = debugerObject;
    }
}

public interface IDebuger
{
    bool AllowDebug { get; set; }
    string AllowName { get; }
}