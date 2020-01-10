using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnChildUpdata : MonoBehaviour
{

    public float aniTime = 0.03f;
    public SpriteRenderer sprite;
    public List<Sprite> sprites = new List<Sprite>();

    private void Start()
    {
        StartCoroutine(Up());
    }

    IEnumerator Up()
    {
        while (true)
        {
            foreach (var item in sprites)
            {
                sprite.sprite = item;
                yield return new WaitForSecondsRealtime(aniTime);
            }
        }

    }
}
