using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class FackAF : MonoBehaviour
{
    [SerializeField]
    bool isAF;
    public static bool isFackAF = false;

    private void Awake()
    {
        isFackAF = isAF;
    }
    private void Update()
    {
        if (isFackAF != isAF)
        {
            isFackAF = isAF;
            if (isFackAF)
            {
                AnalysisController.OnAFStatusChanged?.Invoke();
                AnalysisController.AfStatus = AnalysisController.AFStatus.NonOrganic;
            }
            else
            {
                AnalysisController.OffAFStatusChanged?.Invoke();
                AnalysisController.AfStatus = AnalysisController.AFStatus.Organic;

            }
        }
        
    }
}
