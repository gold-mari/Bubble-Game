using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFromSpriteVar : MonoBehaviour
{
    [SerializeField, Tooltip("The spriteVar we read our sprite from.")]
    private spriteVar spriteVar;


    private Image image;
    private Sprite lastSprite = null;

    
    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (lastSprite != spriteVar.value) {
            image.sprite = spriteVar.value;
            lastSprite = spriteVar.value;
        }
    }
}
