using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HierachryMenu : Editor
{

    static Component[] copiedComponents;


    [MenuItem("GameObject/YuanJi", false, 99)]
    [MenuItem("GameObject/YuanJi/Compoment/Copy All Compoment", false, 20)]
    private static void Copy()
    {
        GameObject body = Selection.activeGameObject;

        if (!body)
        {
            Debug.LogWarning("必须选择一个物体");
            return;
        }

        copiedComponents = body.GetComponents<Component>();
    }

    [MenuItem("GameObject/YuanJi/Compoment/Copy All Compoment New Object &d", false, 20)]
    private static void CopyAndNew()
    {
        GameObject body = Selection.activeGameObject;

        if (!body)
        {
            Debug.LogWarning("必须选择一个物体");
            return;
        }

        copiedComponents = body.GetComponents<Component>();

        GameObject go = new GameObject("TempCompomentGameObject");
        for (int i = 0; i < copiedComponents.Length; i++)
        {
            UnityEditorInternal.ComponentUtility.CopyComponent(copiedComponents[i]);
            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(go);
        }
        go.transform.SetAsLastSibling();
    }

    [MenuItem("GameObject/YuanJi/Compoment/Paste All Compoment", false, 20)]
    private static void Paste()
    {
        GameObject body = Selection.activeGameObject;

        if (!body)
        {
            Debug.LogWarning("必须选择一个物体");
            return;
        }

        for (int i = 0; i < copiedComponents.Length; i++)
        {
            UnityEditorInternal.ComponentUtility.CopyComponent(copiedComponents[i]);
            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(body);
        }
    }
}
