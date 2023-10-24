using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(SquashStretchHandler))]
public class BeatBouncer : MonoBehaviour
{
    [SerializeField, Tooltip("The music manager present in this scene.")]
    private MusicManager musicManager;
    [SerializeField, Tooltip("The curve we evaluate when animating the bounce on the beat. "
                           + "The duration of the curve is determined by beatBounceLength.\n\n"
                           + "Recall negative is squash and positive is stretch.")]
    private AnimationCurve beatBounceCurve;
    [SerializeField, Tooltip("Animation durations are driven by the length of a quarter note, as "
                           + "obtained from musicManager. This field defines what proportion, 0-1, "
                           + "of a quarter note the beatBounceCurve animation lasts.\n\nIf "
                           + "beatBounceLength + leadInLength > 1, this value is normalized.")]
    private double beatBounceLength;
    [SerializeField, Tooltip("The curve we evaluate when animating the lead up to a bounce. "
                           + "The duration of the curve is determined by leadInLength.\n\n"
                           + "Recall negative is squash and positive is stretch.")]
    private AnimationCurve leadInCurve;
    [SerializeField, Tooltip("Animation durations are driven by the length of a quarter note, as "
                           + "obtained from musicManager. This field defines what proportion, 0-1, "
                           + "of a quarter note the leadInCurve animation lasts.\n\nIf beatBounceLength "
                           + "+ leadInLength > 1, this value is normalized.")]
    private double leadInLength;

    // The TimelineHandler on the MusicManager.
    private TimelineHandler handler;
    // The SquashStretchHandler on this object.
    private SquashStretchHandler squashStretch;
    // The currently evaluating coroutine.
    

    private void Awake()
    {
        // Awake is called before start, and normalizes our animation lengths if we need.
        // It also finds the squashStretch component on this object.
        // ================

        // Normalize our lengths if needed.
        if (beatBounceLength + leadInLength > 1)
        {
            double sum = beatBounceLength + leadInLength;
            beatBounceLength = beatBounceLength/sum;
            leadInLength = beatBounceLength/sum;
        }

        squashStretch = GetComponent<SquashStretchHandler>();
    }

    private void Start()
    {
        // Start is called before the first frame update. We use it to subscribe to beat updates.
        // ================

        handler = musicManager.handler;
        handler.beatUpdated += OnBeatUpdated;
    }

    private void OnDestroy()
    {
        // Unsubscribe to beat updates.
        // ================

        handler.beatUpdated -= OnBeatUpdated;
    }

    private void OnBeatUpdated()
    {
        float bounceTime = (float)(beatBounceLength * handler.length4th);
        float leadTime = (float)(leadInLength * handler.length4th);
        float timeBetweenCurves = (float)handler.length4th - bounceTime - leadTime;
        
        StopAllCoroutines();
        StartCoroutine(Bounce(bounceTime, timeBetweenCurves, leadTime));
    }

    private IEnumerator Bounce(float bounceTime, float timeBetweenCurves, float leadTime)
    {
        // Calls EvaluateCurve on the beatBounceCurve, waits, and then calls it on the
        // leadInCurve.
        // ================

        yield return StartCoroutine(EvaluateCurve(beatBounceCurve, bounceTime));
        yield return new WaitForSeconds(timeBetweenCurves);
        yield return StartCoroutine(EvaluateCurve(leadInCurve, leadTime));
    }

    private IEnumerator EvaluateCurve(AnimationCurve curve, float duration)
    {
        // Sets the value of squashStretch.squetch to the value evaluated from curve,
        // over duration seconds.
        // ================

        float elapsed = 0;
        // Store the length of the animation as the time of the final keyframe.
        float animationLength = curve.keys[curve.length-1].time;

        float evalAmount = 0;

        while (elapsed < duration)
        {
            evalAmount = elapsed/duration;
            squashStretch.squetch = curve.Evaluate(evalAmount * animationLength);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
