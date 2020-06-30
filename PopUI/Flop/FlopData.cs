using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class FlopInfo
{
    public int id;
    public string content;
    public int min;
    public int max;
}

public static class FlopData
{
    static FlopData()
    {
        LoadData();
    }

    public enum flopEnum
    {
        money,
    }

    public struct flopS
    {
        public flopEnum type;
        public int num;

        public flopS(flopEnum a, int b)
        {
            type = a;
            num = b;
        }
    }


    private static Dictionary<int ,FlopInfo> flopInfoD = new Dictionary<int, FlopInfo>();

    public static Dictionary<int, FlopInfo> FlopInfoD { get => flopInfoD; }


    private static List<flopS> sumList = new List<flopS>();

    /// <summary>
    /// 结算队列
    /// </summary>
    public static List<flopS> SumList { get => sumList; }

    private static int count = 3;

    /// <summary>
    /// 可以选择次数
    /// </summary>
    public static int Count { get => count; set => count = value; }


    private static void LoadData()
    {
        var flopInfoList = JsonStructureLoader.Load<FlopInfo>();
        foreach (var item in flopInfoList)
        {
            flopInfoD.Add(item.id, item);
        }
    }

    /// <summary>
    /// 随机九个位置
    /// </summary>
    /// <param name="idList">Out位置队列</param>
    public static void RedomList(out List<int> idList)
    {
        idList = Utils.RandomUnduplicated(1, flopInfoD.Count, flopInfoD.Count);
    }



}
