using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimekeeperManager : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [Tooltip("The Endgame Manager present in the scene.")]
    [SerializeField]
    private EndgameManager endgameManager;
    [Tooltip("Not gonna write a meaningful tooltip for this one since it's next on the list to be deprecated.")]
    [SerializeField]
    private float waitLength = 159f;
    [Tooltip("The music event that plays in this scene.")]
    [SerializeField]
    private FMODUnity.EventReference musicEvent;

    // ================================================================
    // Internal variables
    // ================================================================

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
        // When this script is destroyed, stop the music.
        // ================

        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    // ================================================================
    // Soon-to-be-deprecated methods
    // ================================================================

    IEnumerator BAD_BAD_BAD_DEBUG_REPLACE_THIS_waitWin()
    {
        yield return new WaitForSeconds(waitLength);
        endgameManager.TriggerWin();
    }
}
