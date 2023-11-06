using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GRDPpop : PopUIBase
{
    [SerializeField] private Button ppBtn, tosBtn, okBTn;
    private UnityAction _action;

    public override string thisPopUIEnum => PopUIEnum.GRDPpop.ToString();

    public override string thisUIType => PopUIType.POP.ToString();

    private void Start()
    {
        ppBtn.onClick.AddListener((() =>
        {
            Application.OpenURL(About.PPUrlForIOS);
        }));
        tosBtn.onClick.AddListener((() =>
        {
            Application.OpenURL(About.TOSURlForIOS);
        }));
        okBTn.onClick.AddListener((() =>
        {
            _action?.Invoke();
            
            GiftCardData.FristCheckGRDPpop.Value = false;
            AnalysisController.TraceEvent(EventName.GRDP_allow);

            HiddenUIAI();
        }));

    }

    public void OkBtnDownCallBcak(UnityAction action)
    {
        _action = action;
    }

    public override void BeforShow(object[] value)
    {
        
    }

    public override void AfterHiddenUI()
    {
        
    }
}
