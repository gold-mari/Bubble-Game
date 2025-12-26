using UnityEngine;
using System.Collections;

public class tetoMaiFlipper : MonoBehaviour
{
    [SerializeField, Tooltip("The music player present in this scene.")]
    private MusicPlayer musicPlayer;
    public float duration = 0.25f;

    Vector3 baseScale;
    // The TimelineHandler on the MusicManager.
    private TimelineHandler handler;
    bool flipped = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (musicPlayer)
        {
            handler = musicPlayer.handler;
        }
        if (handler != null) 
        {
            handler.markerUpdated += OnMarkerUpdated;
        }

        baseScale = transform.localScale;
    }

    void OnMarkerUpdated(string marker) {
        if (marker.Contains("flip")) {
            flipped = !flipped;
            AnimateFlip(flipped);
        }
    }

    void AnimateFlip(bool target) 
    {
        StopAllCoroutines();
        StartCoroutine(FlipRoutine(target));

        IEnumerator FlipRoutine(bool target) {
            int start = target ? 1 : -1;
            int end = -start;

            transform.localScale = new(start,start,1);

            float elapsed = 0;
            float halfDur = duration/2f;
            while (elapsed <= halfDur) {
                float t = LerpKit.EaseIn(elapsed/halfDur);
                transform.localScale = new(Mathf.Lerp(start,0,t),start,1);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localScale = new(0,end,1);

            elapsed = 0;
            while (elapsed <= duration/2f) {
                float t = LerpKit.EaseOut(elapsed/halfDur);
                transform.localScale = new(Mathf.Lerp(0,end,t),end,1);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localScale = new(end,end,1);
        }
    }
}
