using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadingCheckPP : MonoBehaviour
{
    [SerializeField]
    Toggle agreeToggle;

    [SerializeField]
    GameObject notAgreeObj, isAgreeObj;

    [SerializeField]
    GameObject window;


    private BoolData isAgree = new BoolData("LoadingCheckPP_isAgree",false);
    private UnityAction callClickBack;

    private void Start()
    {

        if (!isAgree.Value)
        {
            agreeToggle.isOn = true;
        }
        window.SetActive(false);
        Refresh();



    }

    public void ChangeAgreeToggle(bool isOn)
    {
        agreeToggle.isOn = isOn;
    }


    /// <summary>
    /// Loading 100%是檢查調用
    /// </summary>
    /// <param name="call">傳入中斷後續操作</param>
    public void Check(UnityAction call)
    {
        callClickBack = call;

        if (!agreeToggle.isOn)
        {
            window.SetActive(true);
            AnalysisController.TraceEvent(EventName.gameName + "login_notagree");
        }
        else
        {
            ClickYes();
        }

    }

    /// <summary>
    /// 按鈕點擊No
    /// </summary>
    public void ClickNo()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// 按鈕點擊Yes
    /// </summary>
    public void ClickYes()
    {
        
        isAgree.Value = true;
        Refresh();

        window.SetActive(false);

        callClickBack?.Invoke();
    }


    private void Refresh()
    {
        notAgreeObj.SetActive(!isAgree.Value);
        isAgreeObj.SetActive(isAgree.Value);
    }


    /*
     
    僅添加到Android部分開屏
        var _sc = this.transform.GetChild(this.transform.childCount-1).GetComponent<LoadingCheckPP>();

        if (_sc)
        {
            _sc.Check(()=> {
                slider.value = 1f;
                Hide();
            });
        }
     
     */
}
