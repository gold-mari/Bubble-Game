using UnityEngine;

[RequireComponent(typeof(BasicUIFade))]
public class FadeToFromBlack : MonoBehaviour
{
    [SerializeField, Tooltip("The music manager present in this scene.")]
    private MusicManager musicManager;
    [SerializeField, Tooltip("The music manager present in this scene.")]
    private BasicUIFade uiFader;
    [SerializeField, Tooltip("Now long we fade for, in seconds.\n\nDefault: 5")]
    private float duration = 5;

    private TimelineHandler _handler;

    private void Start()
    {
        _handler = musicManager.handler;
        _handler.markerUpdated += OnMarkerUpdated;
    }

    private void OnMarkerUpdated(string lastMarker)
    {
        if (lastMarker == "fadeToBlack") {
            uiFader.SetAlpha(0);
            uiFader.FadeIn(duration);
        } else if (lastMarker == "fadeFromBlack") {
            uiFader.SetAlpha(1);
            uiFader.FadeOut(duration);
        } else if (lastMarker == "setToBlack") {
            uiFader.SetAlpha(1);
        }
    }
}
