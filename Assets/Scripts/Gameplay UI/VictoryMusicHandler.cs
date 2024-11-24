using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VictoryMusicHandler : MonoBehaviour 
{
    public SimpleMusicPlayer musicPlayer;
    public Transform headerParent;
    public Transform bodyParent;



    public System.Action ShowBody;
    private NaiveScreenshake naiveScreenshake;



    public void StartMusic()
    {
        headerParent.gameObject.SetActive(false);
        bodyParent.gameObject.SetActive(false);

        naiveScreenshake = Camera.main.GetComponent<NaiveScreenshake>();

        musicPlayer.handler.markerUpdated += OnMarkerUpdated;

        musicPlayer.BeginMusic();
    }

    private void OnMarkerUpdated(string marker)
    {
        if (marker == "showHeader") {
            if (naiveScreenshake != null) naiveScreenshake.BaseShake();
            headerParent.gameObject.SetActive(true);
        } else if (marker == "showBody") {
            if (naiveScreenshake != null) naiveScreenshake.BaseShake();
            bodyParent.gameObject.SetActive(true);
            ShowBody?.Invoke();
        }
    }
}
