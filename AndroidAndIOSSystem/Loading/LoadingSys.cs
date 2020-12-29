using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSys : MonoBehaviour
{
    [SerializeField]
    Slider slider;

    private const float timeLimit = 10f;
    private float timer = 5f;

    void Start()
    {
#if UNITY_EDITOR
        Destroy(this.gameObject);
#endif
#if !UNITY_IOS
        Destroy(this.gameObject);
        return;
#endif
        StartCoroutine(LoadScreen());
    }

    IEnumerator LoadScreen()
    {
        var i = 0f;

        while (i <= timeLimit)
        {
            i += Time.deltaTime;
            slider.value = i / timeLimit;
            yield return null;
        }

        slider.value = 1f;

        AdController.ShowGameStartInterstitial(GoldData.giftNum.Value>=0.01f);

        Destroy(this.gameObject);

    }
}
