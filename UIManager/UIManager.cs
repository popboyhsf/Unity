using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIManager
{
    private static void InitUIDataBase()
    {
        AddUIDataBaseToList(UIType.Setting, new ImageData());
    }






    private static Dictionary<UIType, UIDataBase> UIDataBaseDictionary = new Dictionary<UIType, UIDataBase>();


    private static void AddUIDataBaseToList(UIType iType,UIDataBase uIData)
    {
        if (!UIDataBaseDictionary.ContainsKey(iType))
            UIDataBaseDictionary.Add(iType, uIData);
        else
            Debug.LogWarning("This UIDataBase Hased By Name === " + iType);
    }


    public static UIDataBase VisitUIDataBase(UIType iType)
    {
        if (UIDataBaseDictionary.Count <= 0) InitUIDataBase();

        var item = UIDataBaseDictionary[iType];

        return item;
    }
}
