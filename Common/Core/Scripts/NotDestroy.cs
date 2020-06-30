using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotDestroy : MonoBehaviour
{

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
