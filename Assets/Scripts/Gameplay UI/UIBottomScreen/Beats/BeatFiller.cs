using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatFiller : MonoBehaviour
{
    [SerializeField, Tooltip("The transform of our sprite mask.")]
    private Transform maskTransform;
    [SerializeField, Tooltip("The (min, max) range of angles of the sprite mask.")]
    private Vector2 angleRange;

    [SerializeField, Tooltip("The BeatIndicator component above this object.")]
    private BeatIndicator beatIndicator;
    // The MusicManager referenced in beatIndicator.
    private MusicManager musicManager;
    // A reference to our loop tracker.
    private LoopTracker tracker = null;
    // The currently lerping coroutine.
    private Coroutine lerpRoutine = null;
    
    private IEnumerator Start()
    {
        // Start is called before the first frame update. We use it to get references and
        // initialize our UI.
        // ================

        musicManager = beatIndicator.musicManager;

        // Wait a frame, then access the loop tracker.
        yield return null;

        tracker = beatIndicator.tracker;
        tracker.update += OnUpdate;
        UpdateUI(1f/(tracker.currentBatchSize+1));
    }

    private void OnDestroy()
    {
        tracker.update -= OnUpdate;
    }

    private void OnUpdate()
    {
        // Called every time we get an update from our tracker. Starts the coroutine based off of
        // the current beat in our batch.
        // ================
        
        if (lerpRoutine != null) 
        {
            StopCoroutine(lerpRoutine);
        }

        lerpRoutine = StartCoroutine(LerpBetweenPoints(tracker.currentBatchBeat));
    }

    private IEnumerator LerpBetweenPoints(uint currentBeat)
    {
        double elapsed = 0;
        double duration  = musicManager.handler.length4th;
        float start = GetLerpPointFromBeat(currentBeat);
        float end = GetLerpPointFromBeat(currentBeat+1);
        float currentAmount = start;

        UpdateUI(start);

        while (elapsed <= duration) {
            currentAmount = Mathf.Lerp(start, end, LerpKit.EaseIn((float)(elapsed/duration), 3f));
            UpdateUI(currentAmount);

            elapsed += Time.deltaTime;
            duration = musicManager.handler.length4th;
            yield return null;
        }

        UpdateUI(end);
    }

    private float GetLerpPointFromBeat(uint beat)
    {
        // Returns how far into the song batch we are, from 0 to 1.
        // ================

        float lerpIndex = (float)beat/(tracker.currentBatchSize+1);
        // If we have been taken to our max, return 0.
        if (lerpIndex == 1)
        {
            return 1f/(tracker.nextBatchSize+1);
        }
        // Otherwise, return the index.
        return lerpIndex;
    }

    private void UpdateUI(float amount)
    {   
        // Helper function which lerps the rotation of the maskTransform, depending on
        // the value of amount (min: 0, max: 1).
        // ================

        float zRot = Mathf.Lerp(angleRange.x, angleRange.y, amount);
        maskTransform.rotation = Quaternion.Euler(0,0,zRot);
    }
}
