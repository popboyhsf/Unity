using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADDebuger : MonoBehaviour, IDebuger
{
    public bool AllowDebug 
    {
        get => AdController.isDebug;
        set => AdController.isDebug = value;
    }

    public string AllowName => "屏蔽所有广告";


}
