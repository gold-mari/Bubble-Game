using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[System.Serializable]
public class MarkerEventPair
{
    public string Marker;
    public UnityEvent Event;
}

public class actionOnMarker : MonoBehaviour
{
    [SerializeField, Tooltip("The music player present in this scene.")]
    private MusicPlayer musicPlayer;


    public MarkerEventPair[] MarkerEvents;


    // The TimelineHandler on the MusicManager.
    private TimelineHandler handler;


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
    }

    void OnMarkerUpdated(string marker)
    {
        UnityEvent ourEvent = MarkerEvents.Where(e => marker.Contains(e.Marker)).FirstOrDefault()?.Event;
        ourEvent?.Invoke();
    }
}
