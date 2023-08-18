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
    private Sprite spawnSprite;
    [SerializeField]
    private Sprite massSpawnSprite;
    [SerializeField]
    private Sprite flipSprite;

    private songObject song;

    // Start is called before the first frame update
    void Start()
    {
        song = timekeeper.song;
    }

    public void PlaceIcons(uint min, uint max)
    {
        float lerpAmount = 0;
        float spawnAngle = 0;
        uint batchLength = max-min+1;

        for ( uint i = min; i <= max; i++ )
        {
            uint i_normalized = i-min;
            lerpAmount = (float)i_normalized/batchLength;
            spawnAngle = Mathf.Lerp(angleRange.x, angleRange.y, lerpAmount);

            BeatType type = timekeeper.song.GetBeatType(i);
            // If the type of this beat is not NONE,
            if ( type != BeatType.NONE ) {
                // Spawn an object.
                GameObject iconObj = Instantiate(icon, Vector3.zero, Quaternion.Euler(0,0,spawnAngle), transform);
                // Set its sprite according to its type.
                SpriteRenderer renderer = iconObj.GetComponentInChildren<SpriteRenderer>();
                switch (type)
                {
                    case BeatType.SingleSpawn:
                    {
                        renderer.sprite = spawnSprite;
                        break;
                    }  
                    case BeatType.MassSpawn:
                    {
                        renderer.sprite = massSpawnSprite;
                        break;
                    }
                    case BeatType.GravityFlip:
                    {
                        renderer.sprite = flipSprite;
                        break;
                    }
                }
            }
        }
    }

    public void ClearIcons()
    {
        // Destroys all extant icons by destroying all children.
        // ================

        foreach( Transform child in transform )
        {
            Destroy(child.gameObject);
        }
    }
}
