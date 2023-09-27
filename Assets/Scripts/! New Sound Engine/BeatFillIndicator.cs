using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatFillIndicator : MonoBehaviour
{
    [SerializeField, Tooltip("The music manager present in the scene.")]
    private MusicManager musicManager;
    [SerializeField, Tooltip("The current beatmap. Used to get loop length.")]
    private Beatmap currentBeatmap;
    [SerializeField, Tooltip("How many beats it takes to go from empty to full fill.\n\nDefault: 8")]
    private uint batchSize = 8;
    private LoopTracker tracker;

    [SerializeField]
    private Coroutine lerpRoutine = null;
    [SerializeField]
    private Transform maskTransform;
    [SerializeField]
    private Vector2 angleRange;
    
    private void Start()
    {
        // Start is called before the first frame update. We use it to initialize the UI
        // position and creating the tracker.
        // ================

        // Initialize the tracker.
        tracker = new LoopTracker(musicManager.handler, currentBeatmap.length, batchSize);
        tracker.update += OnUpdate;

        UpdateUI(1f/tracker.currentBatchSize);
    }

    private void OnDestroy()
    {
        tracker.update -= OnUpdate;
    }

    private void OnUpdate()
    {
        // Called every time we get an update from our tracker. Does X Y Z ??????????????????????????
        // ================

        print($"{tracker.currentBatchBeat}/{tracker.currentBatchSize}");
        
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

        float lerpIndex = (float)beat/tracker.currentBatchSize;
        // If we have been taken over our max, return 0.
        if (lerpIndex > 1)
        {
            return 1f/tracker.currentBatchSize;
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
