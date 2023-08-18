using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatFillIndicator : MonoBehaviour
{
    [SerializeField]
    private TimekeeperManager timekeeper;
    [SerializeField]
    private SpriteMask mask;
    private Transform maskTransform;
    [SerializeField]
    private Vector2 angleRange;
    [SerializeField]
    private Animator ghost;
    private bool canTriggerGhost = false;
    // The number of beats that have elapsed in this cycle.
    private uint beatCount = 1;
    bool shouldUpdate = false;

    // Start is called before the first frame update
    void Start()
    {
        maskTransform = mask.transform;
        UpdateUI(0f);

        timekeeper.markerUpdated += OnMarkerUpdated;
        timekeeper.beatUpdated += OnBeatUpdated;
    }

    void OnDestroy()
    {
        timekeeper.markerUpdated -= OnMarkerUpdated;
        timekeeper.beatUpdated -= OnBeatUpdated;
    }

    IEnumerator LerpBetweenPoints(int currentBeat)
    {
        double elapsed = 0;
        double duration  = timekeeper.length4th;
        float start = GetLerpPointFromBeat(currentBeat-1);
        float end = GetLerpPointFromBeat(currentBeat);
        float currentAmount = start;

        UpdateUI(start);

        while (elapsed <= duration) {
            currentAmount = Mathf.Lerp(start, end, LerpKit.EaseIn((float)(elapsed/duration), 3f));
            UpdateUI(currentAmount);

            elapsed += Time.deltaTime;
            duration = timekeeper.length4th;
            yield return null;
        }

        UpdateUI(end);
    }

    float GetLerpPointFromBeat(int beat)
    {
        // Returns how far into the song loop we are, from 0 to 1.
        // ================

        return ((float)beat)/timekeeper.song.loopLength;
    }

    void UpdateUI(float amount)
    {   
        // Helper function which lerps the rotation of the maskTransform, depending on
        // the value of amount (min: 0, max: 1).
        // ================

        float zRot = Mathf.Lerp(angleRange.x, angleRange.y, amount);
        maskTransform.rotation = Quaternion.Euler(0,0,zRot);
    }

    private void OnBeatUpdated()
    {
        // Runs when the timekeeper notes we hit a beat.
        // ================

        if ( shouldUpdate ) {
            StopAllCoroutines();
            StartCoroutine(LerpBetweenPoints((int)beatCount));

            // If we just hit the first beat, ping the ghost animator to fade down.
            if ( canTriggerGhost && beatCount == 1 ) {
                ghost.SetTrigger("FadeDown");
                // Note that we can't do this again until we organically reach the end.
                canTriggerGhost = false;
            }

            // Update beatCount. If we organically reach the end, note that we can show
            // the ghost!
            if ( beatCount >= timekeeper.song.loopLength ) {
                canTriggerGhost = true;
                beatCount = 1;
            }
            else {
                beatCount++;
            }
        }
    }

    private void OnMarkerUpdated()
    {
        // Updates shouldUpdate based on the lastMarker.
        // ================

        if ( timekeeper.timelineInfo.lastMarker == "dontSpawn" ) {
            shouldUpdate = false;
        }
        if ( timekeeper.timelineInfo.lastMarker == "doSpawn" ) {
            // Update beatCount to be the current beat in the measure.
            beatCount = (uint)timekeeper.timelineInfo.currentBeat;  
            shouldUpdate = true;
        }
    }
}
