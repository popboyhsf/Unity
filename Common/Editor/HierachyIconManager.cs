using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[InitializeOnLoad]
public class HierachyIconManager
{
    // 层级窗口项回调
    private static readonly EditorApplication.HierarchyWindowItemCallback hiearchyItemCallback;

    private static Texture2D hierarchyIcon;
    private static Texture2D HierarchyIcon
    {
        get
        {
            if (HierachyIconManager.hierarchyIcon == null)
            {
                HierachyIconManager.hierarchyIcon = (Texture2D)Resources.Load("icon_1");
            }
            return HierachyIconManager.hierarchyIcon;
        }
    }

    /// <summary>
    /// 静态构造
    /// </summary>
    static HierachyIconManager()
    {
        HierachyIconManager.hiearchyItemCallback = new EditorApplication.HierarchyWindowItemCallback(HierachyIconManager.DrawHierarchyIcon);
        EditorApplication.hierarchyWindowItemOnGUI = (EditorApplication.HierarchyWindowItemCallback)Delegate.Combine(
            EditorApplication.hierarchyWindowItemOnGUI,
            HierachyIconManager.hiearchyItemCallback);

        //EditorApplication.update += Update;
    }

    // 绘制icon方法
    private static void DrawHierarchyIcon(int instanceID, Rect selectionRect)
    {
        Rect rectCheck = new Rect(selectionRect);
        rectCheck.x += rectCheck.width - 20;
        rectCheck.width = 18;
        
        Rect nameCheck = new Rect(selectionRect);
        nameCheck.x += 18.5f;
        nameCheck.y += 2.2f;
        //nameCheck.width = 2;

        GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (!go) return;
        go.SetActive(GUI.Toggle(rectCheck, go.activeSelf, string.Empty));

        var index = 0;
        GUIStyle style = null;
        if (go.isStatic)
        {
            index += 1;
            Rect rectIcon = GetRect(selectionRect, index);
            GUI.Label(rectIcon, "S");
        }

        var script = go.GetComponent<IUIListener>();

        if (script != null)
        {
            index += 1;
            Rect rectIcon = GetRect(selectionRect, index);
            GUI.Label(rectIcon, "U");
        }

        var name = go.name.IndexOf("Manager"); ;

        if (name >= 0)
        {
            index += 1;
            Rect rectIcon = GetRect(selectionRect, index);
            GUI.Label(rectIcon, "M");
        }

        // 文字颜色定义 
        var colorMesh = new Color(42 / 255f, 210 / 255f, 235 / 255f);
        var colorSkinMesh = new Color(0.78f, 0.35f, 0.78f);
        var colorLight = new Color(251 / 255f, 244 / 255f, 124 / 255f);
        var colorPhysic = new Color(0.35f, 0.75f, 0f);
        var colorCollider = new Color(0.35f, 0.75f, 0.196f);
        var colorAnimation = new Color(175 / 255f, 175 / 255f, 218 / 255f);
        var colorCamera = new Color(111 / 255f, 121 / 255f, 212 / 255f);
        var colorParticle = new Color(130 / 255f, 124 / 255f, 251 / 255f);
        var colorNav = new Color(217 / 255f, 80 / 255f, 62 / 255f);
        var colorNetwork = new Color(42 / 255f, 129 / 255f, 235 / 255f);
        var colorAudio = new Color(255 / 255f, 126 / 255f, 0f);

        DrawRectIcon(selectionRect, HierarchyIcon, ref index);


        DrawRectIcon<MeshRenderer>(selectionRect, go, colorMesh, ref index, ref style);
        DrawRectIcon<SkinnedMeshRenderer>(selectionRect, go, colorSkinMesh, ref index, ref style);

        // Colliders
        DrawRectIcon<BoxCollider>(selectionRect, go, colorCollider, ref index, ref style);
        DrawRectIcon<SphereCollider>(selectionRect, go, colorCollider, ref index, ref style);
        DrawRectIcon<CapsuleCollider>(selectionRect, go, colorCollider, ref index, ref style);
        DrawRectIcon<MeshCollider>(selectionRect, go, colorCollider, ref index, ref style);
        DrawRectIcon<CharacterController>(selectionRect, go, colorCollider, ref index, ref style);

        // RigidBody
        DrawRectIcon<Rigidbody>(selectionRect, go, colorPhysic, ref index, ref style);

        // Lights
        DrawRectIcon<Light>(selectionRect, go, colorLight, ref index, ref style);

        // Joints

        // Animation / Animator
        DrawRectIcon<Animator>(selectionRect, go, colorAnimation, ref index, ref style);
        DrawRectIcon<Animation>(selectionRect, go, colorAnimation, ref index, ref style);

        // Camera / Projector
        DrawRectIcon<Camera>(selectionRect, go, colorCamera, ref index, ref style);
        DrawRectIcon<Projector>(selectionRect, go, colorCamera, ref index, ref style);

        // NavAgent
        DrawRectIcon<UnityEngine.AI.NavMeshAgent>(selectionRect, go, colorNav, ref index, ref style);
        DrawRectIcon<UnityEngine.AI.NavMeshObstacle>(selectionRect, go, colorNav, ref index, ref style);

        // Network
        DrawRectIcon<NetworkIdentity>(selectionRect, go, colorNetwork, ref index, ref style);
        DrawRectIcon<NetworkAnimator>(selectionRect, go, colorNetwork, ref index, ref style);
        DrawRectIcon<NetworkTransform>(selectionRect, go, colorNetwork, ref index, ref style);
        DrawRectIcon<NetworkBehaviour>(selectionRect, go, colorNetwork, ref index, ref style);
        DrawRectIcon<NetworkManager>(selectionRect, go, colorNetwork, ref index, ref style);

        // Particle
        DrawRectIcon<ParticleSystem>(selectionRect, go, colorParticle, ref index, ref style);

        // Audio
        DrawRectIcon<AudioSource>(selectionRect, go, colorAudio, ref index, ref style);
        DrawRectIcon<AudioReverbZone>(selectionRect, go, colorAudio, ref index, ref style);
        //UI
        DrawRectIcon<Image>(selectionRect, go, colorParticle, ref index, ref style);
        DrawRectIcon<Text>(selectionRect, go, colorParticle, ref index, ref style);

        DrawRectIcon<SpriteRenderer>(selectionRect, go, colorParticle, ref index, ref style);

        // 绘制Label来覆盖原有的名字
        if (style != null && go.activeInHierarchy)
        {
            GUI.Label(nameCheck, go.name, style);
        }
    }

    private static Rect GetRect(Rect selectionRect, int index)
    {
        Rect rect = new Rect(selectionRect);
        rect.x += rect.width - 20 - (20 * index);
        rect.width = 18;
        return rect;
    }

    /// <summary>
    /// 绘制一个Unity原声图标
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rect"></param>
    private static void DrawIcon<T>(Rect rect)
    {
        var icon = EditorGUIUtility.ObjectContent(null, typeof(T)).image;
        GUI.Label(rect, icon);
    }

    private static void DrawRectIcon<T>(Rect selectionRect, GameObject go, Color textColor, ref int order, ref GUIStyle style) where T : Component
    {
        if (go.HasComponent<T>())
        {
            order += 1;
            var rect = GetRect(selectionRect, order);
            DrawIcon<T>(rect);
            GUIStyle titlestyle = new GUIStyle();
            titlestyle.normal.textColor = textColor;
            style = titlestyle;
        }
    }

    private static void DrawRectIcon(Rect selectionRect, Texture2D texture, ref int order)
    {
        order += 1;
        var rect = GetRect(selectionRect, order);
        GUI.Label(rect, texture);
    }
}

public static class ExtensionMethods
{
    public static bool HasComponent<T>(this GameObject go) where T : Component
    {
        return go.GetComponent<T>() != null;
    }
}