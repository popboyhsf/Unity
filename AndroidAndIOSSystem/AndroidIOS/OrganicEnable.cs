using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganicEnable : MonoBehaviour
{
    [Header("是否是买量显示")]
    public bool IsNonOrganicShow = true;

    void Start()
    {
        AnalysisController.OnAFStatusChanged += RefreshDisplay;
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        if (this != null) gameObject.SetActive(!(AnalysisController.IsNonOrganic ^ IsNonOrganicShow));
    }
}
