using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VictoryMusicHandler : MonoBehaviour 
{
    public SimpleMusicPlayer musicPlayer;
    public Transform headerParent;
    public Transform bodyParent;



    public System.Action ShowBody;



    public void StartMusic()
    {
        headerParent.gameObject.SetActive(false);
        bodyParent.gameObject.SetActive(false);

        musicPlayer.handler.markerUpdated += OnMarkerUpdated;

        musicPlayer.BeginMusic();
    }

    private void OnMarkerUpdated(string marker)
    {
        if (marker == "showHeader") {
            headerParent.gameObject.SetActive(true);
        } else if (marker == "showBody") {
            bodyParent.gameObject.SetActive(true);
            ShowBody?.Invoke();
        }
    }
}
