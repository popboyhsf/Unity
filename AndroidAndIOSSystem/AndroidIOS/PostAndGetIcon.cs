using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PostAndGetIcon : MonoBehaviour
{
    [SerializeField]
    Image icon;
    [SerializeField]
    Button button;

    private static PostAndGetIcon _instance;

    public static PostAndGetIcon Instance { get => _instance; }

    private void Awake()
    {
        _instance = this;
        button.onClick.AddListener(ButtonClick);
    }

    private void Start()
    {


    }

    private void ButtonClick()
    {
#if UNITY_EDITOR || NoAd || SafeMode


#elif UNITY_ANDROID && !UNITY_EDITOR && PAG

        CrossAndroid.PostInt(SkillData.Instance.GetPostDeep);
        
#elif UNITY_IPHONE && !UNITY_EDITOR

        CrossIos.PostInt(SkillData.Instance.GetPostDeep);
#endif
    }

    public void PostUrlForIcon()
    {

        if (!AnalysisController.IsNonOrganic) return;

#if UNITY_EDITOR || NoAd || SafeMode


#elif UNITY_ANDROID && !UNITY_EDITOR && PAG

        CrossAndroid.PostUrlForIcon();
        
#elif UNITY_IPHONE && !UNITY_EDITOR
        CrossIos.PostUrlForIcon();
#endif
    }


    public void GetIcon(string url)
    {
        StartCoroutine(GetI(url));
    }

    IEnumerator GetI(string url)
    {
        UnityWebRequest www = www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();


        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("请求Icon网络出现问题 === " + www.error);
        }

        else
        {
            int width = 271;
            int height = 236;
            byte[] results = www.downloadHandler.data;
            Texture2D texture = new Texture2D(width, height);
            texture.LoadImage(results);
            yield return new WaitForSeconds(0.01f);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            icon.sprite = sprite;
            yield return new WaitForSeconds(0.01f);
            Resources.UnloadUnusedAssets();
        }
    }
}
