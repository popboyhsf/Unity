using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganicEnable : MonoBehaviour
{
    void Start()
    {
        AnalysisController.OnAFStatusChanged += RefreshDisplay;
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        gameObject.SetActive(!AnalysisController.IsNonOrganic);
    }
}
