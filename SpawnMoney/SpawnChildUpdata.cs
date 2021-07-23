using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnChildUpdata : MonoBehaviour
{
    public float aniTime = 0.07f;
    public SpriteRenderer sprite;
    public List<Sprite> sprites = new List<Sprite>();

    float spendTime = 0;
    int spriteIndex = 0;

    private void Start()
    {
        ChangeSprite();
    }

    private void Update()
    {
        spendTime += Time.deltaTime;
        if (spendTime > aniTime)
        {
            ChangeSprite();
        }
    }

    private void ChangeSprite()
    {
        sprite.sprite = sprites[spriteIndex];
        spriteIndex = (spriteIndex + 1) % sprites.Count;
        spendTime = 0;
    }
}
