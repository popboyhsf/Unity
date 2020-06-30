using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System.Linq;


[Serializable]
public class WheelInfo
{
    public string gameMode;
    public int weight;
    public int limitWeight;
}
//转盘数据存取
public static class WheelData 
{
    public static List<WheelInfo> WheelInfoList { get; private set; } = new List<WheelInfo>();

    //定义表数据
    static WheelData()
    {
        LoadData();
        
    }

    public static void LoadData()
    {

        WheelInfoList = JsonStructureLoader.Load<WheelInfo>();
    }

    /// <summary>
    /// 返回随机出来的游戏模式枚举
    /// </summary>
    /// <returns>游戏模式枚举</returns>
    public static modeEnum GetRandomItemID()
    {
        var weightMax = 0;
        foreach (var item in WheelInfoList)
        {
            //TODO 减去当前累计数额
            item.weight = item.weight > item.limitWeight ? item.weight : 0;
            weightMax += item.weight;
        }


        WheelInfoList.Sort((x,y)=> {

            if (x.weight > y.weight)
                return -1;
            else 
            if (x.weight == y.weight)
                return 0;
            else
                return 1;

        });

        int rankA = UnityEngine.Random.Range(1, weightMax);
        ////Debug.Log("rankA ===== " + rankA);

        int _index = 0;

        while (rankA > 0)
        {
            rankA -= WheelInfoList[_index].weight;
            _index++;
        }

        var i = WheelInfoList[_index - 1].gameMode;

        modeEnum type = (modeEnum)Enum.Parse(typeof(modeEnum), i);

        Debug.Log("Random Mode == " + type);

        return type;

    }

}
    
