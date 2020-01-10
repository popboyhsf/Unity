using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnClick : MonoBehaviour
{
    public SpawnMoney from;
    public Transform to;
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            from.BoombToCollectCurrency(from.transform, 5, to.position);

        }
    }
}
