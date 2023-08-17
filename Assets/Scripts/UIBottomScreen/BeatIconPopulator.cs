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

    // Start is called before the first frame update
    void Start()
    {
        float lerpAmount = 0;
        float spawnAngle = 0;
        for ( float f=0; f < timekeeper.song.loopLength; f++)
        {
            lerpAmount = f/timekeeper.song.loopLength;
            spawnAngle = Mathf.Lerp(angleRange.x, angleRange.y, lerpAmount);

            Instantiate(icon, Vector3.zero, Quaternion.Euler(0,0,spawnAngle), transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
