using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorIndicator : MonoBehaviour
{
    [Tooltip("The bubble_ColorVar that this script should monitor.")]
    public bubble_ColorVar color;
    // The sprite renderer on this gameObject.
    SpriteRenderer sprite;

    void Start()
    {
        // Start is called before the first frame update. Used to define sprite. If a
        // SpriteRenderer doesn't exist on this object, raise an error.
        // ================

        sprite = GetComponent<SpriteRenderer>();
        Debug.Assert( sprite != null, "ColorIndicator Error: Start() failed: gameObject must have a SpriteRenderer.", this );
    }

    void Update()
    {
        // Update is called once per frame. Used to update the sprite Color to the Color
        // which corresponds to our Bubble_Color.
        // ================

        sprite.color = Bubble_Color_Methods.getSpriteColor(color.value);
    }
}
