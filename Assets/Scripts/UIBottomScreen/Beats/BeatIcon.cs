using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatIcon : MonoBehaviour
{
    // The beat number in the batch this icon corresponds to.
    private uint batchBeat;
    // The LoopTracker running the beat UI.
    private LoopTracker tracker;
    // The sprite renderer on this object.
    private SpriteRenderer sprite;

    public void Initialize(uint beat, LoopTracker loopTracker)
    {
        // Supplies our batchBeat ID and the loopTracker reference.
        // ================

        batchBeat = beat;

        tracker = loopTracker;
        tracker.update += OnUpdate;

        sprite = GetComponentInChildren<SpriteRenderer>();
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
        if (batchBeat == tracker.nextBatchBeat)
        {
            StartCoroutine(Flicker());
        }
        // If it's our turn, ...
        else if (batchBeat == tracker.currentBatchBeat)
        {
            StopAllCoroutines();
            Color grey = Color.Lerp(Color.white, Color.black, 0.5f);
            grey = Color.Lerp(grey, Color.clear, 0.8f);
            sprite.color = grey;
        }
    }

    private IEnumerator Flicker()
    {
        // DEBUG DEBUG DEBUG DEBUG DEBUG DEBUG DEBUG DEBUG 
        // DEBUG DEBUG DEBUG DEBUG DEBUG DEBUG DEBUG DEBUG 
        // DEBUG DEBUG DEBUG DEBUG DEBUG DEBUG DEBUG DEBUG 
        // DEBUG DEBUG DEBUG DEBUG DEBUG DEBUG DEBUG DEBUG 

        // Todo: do a squash stretch animation instead?
        // ================

        Color original = sprite.color;

        while (true)
        {
            sprite.color = Color.white;
            yield return null;
            sprite.color = original;
            yield return null;
        }
    }
}
