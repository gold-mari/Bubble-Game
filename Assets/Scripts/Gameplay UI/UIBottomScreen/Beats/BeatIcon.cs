using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class BeatIcon : MonoBehaviour
{
    [SerializeField, Tooltip("The interval between 'flash' and 'unflash' states in the flicker animation.\n\nDefault: 0.1f")]
    float flashDelay = 0.1f;
    [SerializeField, ReadOnly, Tooltip("The beat number in the loop this icon corresponds to. Exposed for reference.")]
    private uint loopBeat;
    // The LoopTracker running the beat UI.
    private LoopTracker tracker;
    // Whether or not this icon represents a single spawn.
    bool single;
    // The sprite renderer on this object.
    private SpriteRenderer sprite;
    // The base scale of this icon's sprite.
    private Vector3 baseScale;
    // The currently running flicker routine. Used to check if one is running.
    Coroutine flickerRoutine = null;

    public void Initialize(uint beat, LoopTracker loopTracker, bool isSingle)
    {
        // Supplies our loopBeat ID and the loopTracker reference.
        // ================

        loopBeat = beat;

        tracker = loopTracker;
        tracker.update += OnUpdate;

        single = isSingle;

        sprite = GetComponentInChildren<SpriteRenderer>();

        baseScale = sprite.transform.localScale;

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

    private void OnUpdate()
    {
        // Called every time the loop tracker updates. ...
        // DEBUG DEBUG DEBUG DEBUG
        // DEBUG DEBUG DEBUG DEBUG
        // DEBUG DEBUG DEBUG DEBUG
        // ================

        // If we're up next, flash white.
        // For single spawns, we are considered 'up next' 1 beat before our turn.
        // For other events, we are considered 'up next' 1-2 beats before our turn.
        if ((loopBeat == tracker.nextLoopBeat) ||
            (!single && loopBeat == tracker.secondNextLoopBeat))
        {
            if (flickerRoutine == null)
            {
                flickerRoutine = StartCoroutine(Flicker());
            }
            
            sprite.transform.localScale = 1.5f * baseScale;
        }
        // If it's our turn, go back to normal scale and turn grey, to signal we're spent.
        else if (loopBeat == tracker.currentLoopBeat)
        {
            if (flickerRoutine != null)
            {
                StopCoroutine(flickerRoutine);
                flickerRoutine = null;
            }

            sprite.transform.localScale = baseScale;

            Color grey = Color.Lerp(Color.white, Color.black, 0.5f);
            //grey = Color.Lerp(grey, Color.clear, 0.8f);
            sprite.color = grey;
        }
    }

    private IEnumerator Flicker()
    {
        // Programmatic animation which flickers the color from the original to white over
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
