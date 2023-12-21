using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class ColorIndicator : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [Tooltip("The Bubble_FlavorVar that this script should monitor.")]
    public Bubble_FlavorVar flavor;
    [SerializeField, Tooltip("If we should change our color according to the monitored flavor.")]
    private bool changeColor = true;
    [SerializeField, Tooltip("If we should change our sprite according to the monitored flavor.")]
    private bool changeSprite = true;
    [ShowIf("changeSprite"), SerializeField, Tooltip("A binder of bubble sprites, corresponding to the color of the bubble this indicator monitors.")]
    private BubbleSpriteBinder binder;

    // ================================================================
    // Internal variables
    // ================================================================

    // The sprite renderer on this gameObject.
    private SpriteRenderer sprite;
    // The image renderer on this gameObject.
    private Image image;

    // ================================================================
    // Default methods
    // ================================================================

    void Start()
    {
        // Start is called before the first frame update. Used to define sprite. If a
        // SpriteRenderer doesn't exist on this object, raise an error.
        // ================

        sprite = GetComponent<SpriteRenderer>();
        image = GetComponent<Image>();
        Debug.Assert(sprite != null || image != null, "ColorIndicator Error: Start() failed: gameObject must have a SpriteRenderer or Image.", this);
    }

    void Update()
    {
        // Update is called once per frame. Used to update the sprite Color (and potentailly sprite) 
        // to the Color which corresponds to our Bubble_Flavor.
        // ================

        Color c = Bubble_Flavor_Methods.getColor(flavor.value);
        if (sprite)
        {
            if (changeColor)
            {
                sprite.color = c;
            }
            if (changeSprite)
            {
                sprite.sprite = Bubble_Flavor_Methods.getSprite(flavor.value, binder);
            }
        }
        if (image)
        {
            if (changeColor)
            {
                image.color = c;
            }
            if (changeSprite)
            {
                image.sprite = Bubble_Flavor_Methods.getSprite(flavor.value, binder);
            }
        }
    }
}
