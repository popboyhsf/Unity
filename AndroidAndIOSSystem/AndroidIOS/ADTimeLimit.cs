using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADTimeLimit : MonoBehaviour
{
    private const float limitConst = 60f;
    public float limit = 0;

    IEnumerator startLimit()
    {
        limit = limitConst;
        while (limit > 0)
        {
            limit -= Time.deltaTime;
            yield return null;
        }
        yield break;
    }

    public void StartLimit()
    {
        if (limit > 0)
            limit = limitConst;
        else
            this.StartCoroutine(startLimit());
    }
}
