using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

/// <summary>
/// IPC½Ó¿Ú
/// </summary>
public static class IpcInterface
{
    public enum IPC_CS_TYPE
    {
        CLIENT = 0,
        SERVER = 1,
        UNKNOWN = 2,
    }

    [DllImport("kuaishou_ipc.dll")]
    public static extern int InitIpc(string ipcName, uint size, IPC_CS_TYPE type);

    [DllImport("kuaishou_ipc.dll")]
    public static extern int ReleaseIpc();

    [DllImport("kuaishou_ipc.dll")]
    public static extern int SendData(string content, uint size);

    [DllImport("kuaishou_ipc.dll")]
    public static extern int SetDataReceivedCallback(Action<IntPtr, uint, IntPtr> callBack, IntPtr ptr);

    [DllImport("kuaishou_ipc.dll")]
    public static extern int SetConnectedCallback(Action<IntPtr> callBack, IntPtr ptr);

    [DllImport("kuaishou_ipc.dll")]
    public static extern int SetDisconnectCallback(Action<IntPtr> callBack, IntPtr ptr);

    [DllImport("kuaishou_ipc.dll")]
    public static extern int SetLogCallback(Action<IntPtr, uint, IntPtr> callBack, IntPtr ptr);

}