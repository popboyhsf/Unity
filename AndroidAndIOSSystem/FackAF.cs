using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class FackAF : MonoBehaviour, IDebuger
{
    [SerializeField]
    bool isAF;
    public static bool isFackAF = false;

    public bool AllowDebug { get => isAF; set => isAF = value; }

    void Update()
    {
        if (isFackAF != isAF)
        {
            isFackAF = isAF;
            if (isFackAF) AnalysisController.OnAFStatusChanged?.Invoke();
            else AnalysisController.OffAFStatusChanged?.Invoke();
        }
        
    }
}
