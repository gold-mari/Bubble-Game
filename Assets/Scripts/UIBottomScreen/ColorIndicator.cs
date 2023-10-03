using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorIndicator : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [Tooltip("The Bubble_FlavorVar that this script should monitor.")]
    public Bubble_FlavorVar color;
    [SerializeField, Tooltip("A binder of bubble sprites, corresponding to the color of the bubble this indicator monitors.")]
    private BubbleSpriteBinder binder;

    // ================================================================
    // Internal variables
    // ================================================================

    // The sprite renderer on this gameObject.
    private SpriteRenderer sprite;

    // ================================================================
    // Default methods
    // ================================================================

    void Start()
    {
        // Start is called before the first frame update. Used to define sprite. If a
        // SpriteRenderer doesn't exist on this object, raise an error.
        // ================

        sprite = GetComponent<SpriteRenderer>();
        Debug.Assert(sprite != null, "ColorIndicator Error: Start() failed: gameObject must have a SpriteRenderer.", this);
    }

    void Update()
    {
        // Update is called once per frame. Used to update the sprite Color to the Color
        // which corresponds to our Bubble_Flavor.
        // ================

        sprite.color = Bubble_Flavor_Methods.getColor(color.value);
        //sprite.sprite = Bubble_Flavor_Methods.getSprite(color.value, binder);
    }
}
