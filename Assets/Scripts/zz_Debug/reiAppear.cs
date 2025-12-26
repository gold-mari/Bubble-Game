using UnityEngine;
using System.Collections;

public class reiAppear : MonoBehaviour
{
    [SerializeField, Tooltip("The music player present in this scene.")]
    private MusicPlayer musicPlayer;
    public float duration = 0.25f;

    Vector3 baseScale;
    // The TimelineHandler on the MusicManager.
    private TimelineHandler handler;
    private CanvasGroup group;

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

        group = GetComponent<CanvasGroup>();
        if (group) group.alpha = 0;
    }

    void OnMarkerUpdated(string marker) {
        if (marker.Contains("rei")) {
            Animate();
        }
    }

    void Animate() 
    {
        StopAllCoroutines();
        StartCoroutine(AnimateRoutine());

        IEnumerator AnimateRoutine() {
            group.alpha = 1;

            float elapsed = 0;
            while (elapsed <= duration) {
                float t = LerpKit.EaseIn(elapsed/duration);
                group.alpha = Mathf.Lerp(1,0,t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            group.alpha = 0;
        }
    }
}
