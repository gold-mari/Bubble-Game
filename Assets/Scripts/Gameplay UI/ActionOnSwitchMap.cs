using UnityEngine;

public class ActionOnSwitchMap : MonoBehaviour
{
    [SerializeField, Tooltip("The music manager present in this scene.")]
    protected MusicManager musicManager;

    protected virtual void Start()
    {
        musicManager.handler.markerUpdated += OnMarkerUpdated;
    }

    private void OnMarkerUpdated(string lastMarker)
    {
        string[] medleyStrings = lastMarker.Split('-');
        if (medleyStrings[0] == "switchMap")
        {
            Debug.Assert(medleyStrings.Length == 2, $"ActionOnSwitchMap error: OnMarkerUpdated failed. "
                                                    + $"Unable to parse switchMap marker: {lastMarker}");
            OnSwitchMap(medleyStrings[1]);
        }
    }

    protected virtual void OnSwitchMap(string mapName)
    {
        return;
    }
}
