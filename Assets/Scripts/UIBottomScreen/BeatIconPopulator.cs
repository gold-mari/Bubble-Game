using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatIconPopulator : MonoBehaviour
{
    [SerializeField]
    private TimekeeperManager timekeeper;
    [SerializeField]
    private Vector2 angleRange;
    [SerializeField]
    private GameObject icon;
    [SerializeField]
    private int displaySize;

    // Start is called before the first frame update
    void Start()
    {
        float lerpAmount = 0;
        float spawnAngle = 0;
        uint beatToDisplay = 1;
        int batch = 1;

        songObject song = timekeeper.song;

        while ( beatToDisplay <= song.loopLength )
        {
            for ( int i = 0; i < displaySize && beatToDisplay <= song.loopLength; i++ )
            {
                print( $"{batch}, {beatToDisplay}" );

                lerpAmount = (float)beatToDisplay/song.loopLength;
                spawnAngle = Mathf.Lerp(angleRange.x, angleRange.y, lerpAmount);
                if ( song.GetBeatType(beatToDisplay) != BeatType.NONE ) {
                    Instantiate(icon, Vector3.zero, Quaternion.Euler(0,0,spawnAngle), transform);
                }        

                beatToDisplay++;
            }
            batch++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
