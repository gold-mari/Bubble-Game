using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatIndicator : MonoBehaviour
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

        string printstring = "";
        for ( float f=1; f <= timekeeper.song.loopLength; f++)
        {
            printstring += $"{f/timekeeper.song.loopLength} ";
        }
        print(printstring);

        timekeeper.markerUpdated += PassedMarker;
        timekeeper.beatUpdated += HitBeat;
    }

    void OnDestroy()
    {
        timekeeper.markerUpdated -= PassedMarker;
        timekeeper.beatUpdated -= HitBeat;
    }

    IEnumerator LerpBetweenPoints(int currentBeat)
    {
        double elapsed = 0;
        double duration  = timekeeper.length4th;
        float start = LerpPointFromBeat(currentBeat-1);
        float end = LerpPointFromBeat(currentBeat);
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

    float LerpPointFromBeat(int beat)
    {
        return ((float)beat)/timekeeper.song.loopLength;
    }

    void UpdateUI(float amount)
    {
        float zRot = Mathf.Lerp(angleRange.x, angleRange.y, amount);
        maskTransform.rotation = Quaternion.Euler(0,0,zRot);
    }

    private void HitBeat()
    {
        // ...
        // ================

        if ( shouldUpdate ) {
            StopAllCoroutines();
            StartCoroutine(LerpBetweenPoints((int)beatCount));

            // If we just hit the first beat, ping the ghost animator to fade down.
            if ( canTriggerGhost && beatCount == 1 ) {
                ghost.SetTrigger("FadeDown");
            }

            // Update beatCount.
            if ( beatCount >= timekeeper.song.loopLength ) {
                canTriggerGhost = true;
                beatCount = 1;
            }
            else {
                beatCount++;
            }
        }
    }

    private void PassedMarker()
    {
        // Updates shouldUpdate based on the lastMarker.
        // ================

        if ( timekeeper.timelineInfo.lastMarker == "dontSpawn" ) {
            UpdateUI(0f);
            shouldUpdate = false;
        }
        if ( timekeeper.timelineInfo.lastMarker == "doSpawn" ) {
            // Update beatCount to be the current beat in the measure.
            beatCount = (uint)timekeeper.timelineInfo.currentBeat;  
            shouldUpdate = true;
        }
    }
}
