using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class DangerIndicator : MonoBehaviour
{

    // ================================================================
    // Parameters
    // ================================================================

    [Tooltip("The boolVar which signals if gravity is flipped to point outwards instead of inwards.")]
    [SerializeField]
    private boolVar gravityFlipped;
    [Tooltip("The floatVar, ranging from 0 to 1, which signals how close we are to a game over.")]
    [SerializeField]
    private floatVar dangerAmount;
    [Tooltip("The size of the mask, ranging from 0 to 1, corresponding to min to max danger when "
           + "gravity is flipped OUTWARDS.\n\nIMPORTANT: Used with VisibleInsideMask.")]
    [SerializeField]
    private Vector2 innerRange;
    [Tooltip("The size of the mask, ranging from 1 to 0, corresponding to min to max danger when "
           + "gravity points INWARDS.\n\nIMPORTANT: Used with VisibleOutsideMask.")]
    [SerializeField]
    private Vector2 outerRange;

    // ================================================================
    // Internal variables
    // ================================================================

    // The sprite used as the danger indicator.
    private SpriteRenderer indicatorSprite;
    // The transform of the sprite used to mask the indicatorSprite.
    private Transform maskTransform;

    // ================================================================
    // Default methods
    // ================================================================

    void Start()
    {
        // Start is called before the first frame update. Used to define indicatorSprite
        // and maskTransform.
        // ================

        // Get indicatorSprite from us.
        indicatorSprite = GetComponent<SpriteRenderer>();
        // Get the transform of our masked child.
        maskTransform = GetComponentInChildren<SpriteMask>().transform;
    }

    void Update()
    {
        // Update is called once per frame. Used to update the size of the mask and flip
        // the mask behavior of the parent sprite.
        // ================

        // A temp variable that is overwritten depending on if we're using the inner or
        // outer range vector.
        Vector2 range = Vector2.zero;

        if (!gravityFlipped.value) {
            // If gravity points inwards, then the danger zone is on the outside.
            indicatorSprite.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            range = outerRange;
        }
        else { // if gravityFlipped is true...
            // If gravity points outwards, then the danger zone is on the inside.
            indicatorSprite.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            range = innerRange;
        }

        // Lerp scale based on our chosen range.
        float scale = Mathf.Lerp(range.x, range.y, dangerAmount.value);
        maskTransform.localScale = new Vector3(scale,scale,scale);

        // Lerp color from transparent to opaque. Use an ease out so that initial gains
        // in opacity are greater than later gains.
        indicatorSprite.color = Color.Lerp(Color.clear, Color.white, LerpKit.EaseOut(dangerAmount.value, 2.5f));
    }
}
