using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueAlignSpriteImage : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Image image;

    private Sprite lastSprite;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        image = GetComponent<Image>();

        lastSprite = spriteRenderer.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        // If it changed, align the image sprite to the spriteRenderer sprite.
        // ================
        if (spriteRenderer.sprite != lastSprite) {
            image.sprite = spriteRenderer.sprite;
            lastSprite = spriteRenderer.sprite;
        }
    }
}
