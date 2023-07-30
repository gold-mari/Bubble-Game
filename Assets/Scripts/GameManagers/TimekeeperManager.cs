using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimekeeperManager : MonoBehaviour
{
    [SerializeField]
    private EndgameManager endgameManager;
    [SerializeField]
    private float waitLength = 159f;
    [SerializeField]
    private FMODUnity.EventReference musicEvent;
    [SerializeField]
    private FMOD.Studio.EventInstance instance;

    // ================================================================
    // Default methods
    // ================================================================

    void Start()
    {
        // Start is called before the first frame update. For now, we just use it to
        // start the music.
        // ================

        instance = FMODUnity.RuntimeManager.CreateInstance(musicEvent);
        instance.start();
        instance.release();

        StartCoroutine(BAD_BAD_BAD_DEBUG_REPLACE_THIS_waitWin());
    }

    void OnDestroy()
    {
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    IEnumerator BAD_BAD_BAD_DEBUG_REPLACE_THIS_waitWin()
    {
        yield return new WaitForSeconds(waitLength);
        endgameManager.TriggerWin();
    }
}
