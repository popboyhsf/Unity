using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_Text))]
public class LinkOpener : MonoBehaviour, IPointerClickHandler
{
    private TMP_Text m_textMeshPro;
    private Camera m_uiCamera;

    void Start()
    {
        m_textMeshPro = GetComponent<TMP_Text>();
        Camera[] cameras = FindObjectsOfType<Camera>();
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i].CompareTag("MainCamera")) //換成這個UI對應的相機
            {
                m_uiCamera = cameras[i];
                break;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(m_textMeshPro, eventData.position, m_uiCamera); 
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = m_textMeshPro.textInfo.linkInfo[linkIndex];

            if (linkInfo.GetLinkID().IndexOf("1") >= 0)
            {
#if UNITY_IPHONE
                CrossIos.IOSWebPageShow(About.PPUrlForIOS);
                return;
#endif
                Application.OpenURL(About.PPUrl);
            }
            else if (linkInfo.GetLinkID().IndexOf("2") >= 0)
            {
#if UNITY_IPHONE
                 CrossIos.IOSWebPageShow(About.TOSURlForIOS);
                return;
#endif
                Application.OpenURL(About.TOSURl);
            }


            //Debuger.Log("GetLinkID == " + linkInfo.GetLinkID());
        }
    }

}