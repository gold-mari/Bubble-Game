using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatFlash : BeatWarning
{
    // ================================================================
    // Parameters
    // ================================================================

    [SerializeField, Tooltip("Whether we should flip our sprite on a major flash.")]
    private bool doFlip;
    [SerializeField, Tooltip("The sprite to display on a minor flash.")]
    private Sprite minorSprite;
    [SerializeField, Tooltip("The sprite to display on a major flash.")]
    private Sprite majorSprite;

    // ================================================================
    // Internal variables
    // ================================================================

    // The sprite renderer on this object.
    private SpriteRenderer spriteRenderer;
    // The animator on this object.
    private Animator animator;

    // ================================================================
    // Initialization / finalization methods
    // ================================================================

    protected override IEnumerator Start()
    {
        // Start is called before the first frame update. We use it to get references and
        // initialize our UI.
        // ================

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        yield return StartCoroutine(base.Start());
    }

    // ================================================================
    // Update methods
    // ================================================================

    protected override void OnMinorFlash()
    {
        // From base class. Called if the next beat is a single spawn.
        // ================

        if (spriteRenderer)
        {
            if (doFlip) spriteRenderer.flipY = false;
            spriteRenderer.sprite = minorSprite;   
        }

        if (animator) animator.SetTrigger("MinorFlash");
    }

    protected override void OnMajorFlash()
    {
        // From base class. Called if the second next beat OR the next beat is a higher-order beat 
        // (not single, not null).
        // ================

        if (spriteRenderer)
        {
            if (doFlip) spriteRenderer.flipY = true;
            spriteRenderer.sprite = majorSprite;
        }

        if (animator) animator.SetTrigger("MajorFlash");
    }
}
