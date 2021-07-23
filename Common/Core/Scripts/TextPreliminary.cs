using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPreliminary
{
    private float leftF;
    private float rightF;
    private string geshi;
    private TextMeshProUGUI T;

    public void FloatToFloat(float left, float right, MonoBehaviour thisSelf, float timer,float lerp,TextMeshProUGUI text,string geshi)
    {
        leftF = left;
        rightF = right;
        T = text;
        this.geshi = geshi;
        thisSelf.StartCoroutine(Timer(timer, lerp));
    }

    private IEnumerator Timer(float timer,float lerp)
    {
        yield return new WaitForSecondsRealtime(lerp);
        var s = (rightF - leftF) / timer;

        while (timer > 0)
        {
            leftF += s * Time.deltaTime;
            T.text = leftF.ToString(geshi);
            timer -= Time.deltaTime;
            yield return null;
        }

        T.text = rightF.ToString(geshi);

        yield break;

    }
}
