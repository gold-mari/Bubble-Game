using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatIndicator : MonoBehaviour
{
    [SerializeField]
    private TimekeeperManager timekeeperManager;
    [SerializeField]
    private SpriteMask mask;
    private Transform maskTransform;
    [SerializeField]
    private Vector2 angleRange;
    float timer = 0;
    bool shouldReset = false;
    double loopDuration = 0;

    // Start is called before the first frame update
    void Start()
    {
        maskTransform = mask.transform;
        timekeeperManager.beatUpdated += ResetTimer;
    }

    void OnDestroy()
    {
        timekeeperManager.beatUpdated -= ResetTimer;
    }

    // Update is called once per frame
    void Update()
    {
        loopDuration = timekeeperManager.song.loopLength * timekeeperManager.length4th;
        if ( loopDuration != 0 ) {
            float zRot = Mathf.Lerp(angleRange.x, angleRange.y, (float)(timer/loopDuration));
            maskTransform.rotation = Quaternion.Euler(0,0,zRot);
        }

        timer += Time.deltaTime;

        if ( timer > loopDuration ) {
            shouldReset = true;
        }
    }

    void ResetTimer()
    {
        if ( shouldReset ) {
            timer = 0;
            shouldReset = false;
        }
    }
}
