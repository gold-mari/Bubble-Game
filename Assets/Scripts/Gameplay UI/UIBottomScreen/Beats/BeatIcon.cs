using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Rendering.Universal;

public class BeatIcon : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [SerializeField, ReadOnly, Tooltip("The beat number in the loop this icon corresponds to. Exposed for reference.")]
    private uint loopBeat;

    [Header("Animation Settings")]
    [SerializeField, Tooltip("The interval between 'flash' and 'unflash' states in the flicker animation.\n\nDefault: 0.1f")]
    private float flashDelay = 0.1f;
    [SerializeField, Tooltip("The base scale of the sprite on this object is, by default, read from it's initial size. "
                           + "Check this if you want to define it manually.")]
    private bool overrideBaseScale = false;
    [ShowIf("overrideBaseScale"), SerializeField, Tooltip("The base scale of the sprite on this object.")]
    private Vector3 baseScale;
    [SerializeField, Tooltip("The enlarged scale of the sprite on this object, as a multiple of baseScale.")]
    private Vector3 bigFactor;
    [SerializeField, Range(0,1), Tooltip("The lerp index, between 0 and 1, between our baseScale and bigScale. "
                                       + "Applied to our sprite.")]
    private float scaleIndex;


    [SerializeField, Tooltip("The base color of the sprite on this object is, by default, read from it's initial color. "
                           + "Check this if you want to define it manually.")]
    private bool overrideBaseColor = false;
    [ShowIf("overrideBaseColor"), SerializeField, Tooltip("The base color of the sprite on this object.")]
    private Color baseColor;
    [SerializeField, Tooltip("The color we lerp to on highlight.")]
    private Color highlightColor;
    [SerializeField, Range(0,1), Tooltip("The lerp index, between 0 and 1, between our baseColor and highlightColor. "
                                       + "Applied to our sprite.")]
    private float colorIndex;
    [SerializeField, Range(0,1), Tooltip("The lerp index, between 0 and 1, between our color and grey. "
                                       + "Applied to our sprite.")]
    private float greyness;

    // ================================================================
    // Internal variables
    // ================================================================

    // The LoopTracker running the beat UI.
    private LoopTracker tracker;
    // Whether or not this icon represents a single spawn.
    bool single;
    // The sprite renderer on this object.
    private SpriteRenderer sprite;
    // The animator on this object.
    private Animator anim;
    // The currently running flicker routine. Used to check if one is running.
    Coroutine flickerRoutine = null;
    // If our beat has passed already.
    private bool spent = false;

    // Grey.
    private static Color grey = Color.Lerp(Color.black, Color.white, 0.5f);
    // The true scale of 
    private static Vector3 bigScale;

    // ================================================================
    // Initialization / Finalization methods
    // ================================================================

    public void Initialize(uint beat, LoopTracker loopTracker, bool isSingle)
    {
        // Supplies our loopBeat ID and the loopTracker reference.
        // ================

        loopBeat = beat;

        tracker = loopTracker;
        tracker.update += OnUpdate;

        single = isSingle;

        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // Get our base scale and color, if we're not overriding them.
        if (!overrideBaseScale)
        {
            baseScale = sprite.transform.localScale;
        }
        if (!overrideBaseColor)
        {
            baseColor = sprite.color;
        }

        // Calculate bigScale from bigFactor.
        bigScale = Vector3.Scale(baseScale, bigFactor);

        // Force OnUpdate immediately after initialization. Without this, icons on the 1st or 2nd
        // batch beat will not appear correctly.
        OnUpdate();
    }

    private void OnDestroy()
    {
        // Unsubscribes us from the tracker.
        // ================

        tracker.update -= OnUpdate;
    }

    // ================================================================
    // Continuous methods
    // ================================================================

    private void Update()
    {
        // Animation logic which takes our index for scale and applies it to our sprite.
        // ================

        sprite.transform.localScale = Vector3.Lerp(baseScale, bigScale, scaleIndex);
    }

    private void OnUpdate()
    {
        // Called every time the loop tracker updates. Triggers animations depending on what beat
        // the loop tracker is in, relative to our beat ID.
        // ================

        if (spent)
        {
            return;
        }

        // If we're up next, flash white.
        // For single spawns, we are considered 'up next' 1 beat before our turn.
        // For other events, we are considered 'up next' 1-2 beats before our turn.
        if ((loopBeat == tracker.nextLoopBeat) ||
            (!single && loopBeat == tracker.secondNextLoopBeat))
        {
            anim.SetTrigger("UpNext");

            if (flickerRoutine == null)
            {
                flickerRoutine = StartCoroutine(Flicker());
            }
        }
        // If it's our turn, go back to normal scale and turn grey, to signal we're spent.
        else if (loopBeat == tracker.currentLoopBeat)
        {
            anim.SetTrigger("Spent");

            if (flickerRoutine != null)
            {
                StopCoroutine(flickerRoutine);
                flickerRoutine = null;
            }

            Color grey = Color.Lerp(Color.white, Color.black, 0.2f);
            grey = Color.Lerp(grey, Color.clear, 0.4f);
            sprite.color = grey;

            spent = true;
        }
    }

    private IEnumerator Flicker()
    {
        // Programmed animation which flickers the color from the original to white over
        // flashDelay seconds.
        // ================

        Color original = sprite.color;
        WaitForSeconds wait = new WaitForSeconds(flashDelay);

        while (true)
        {
            sprite.color = Color.white;
            yield return wait;
            sprite.color = original;
            yield return wait;
        }
    }
}