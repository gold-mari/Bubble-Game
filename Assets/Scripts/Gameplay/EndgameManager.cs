using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EndgameManager : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [SerializeField, Tooltip("The level loader present in the scene.")]
    private LevelLoader loader;
    [SerializeField, Tooltip("The name of the scene we transition to on a loss- reaching the end of the song.")]
    private string sceneOnWin;
    [Tooltip("A UnityEvent which communicates with CharacterAnimator that we have just won.")]
    [SerializeField]
    UnityEvent winTriggered;
    [Tooltip("A UnityEvent which communicates with CharacterAnimator that we have just lost.")]
    [SerializeField]
    UnityEvent lossTriggered;

    // ================================================================
    // Internal variables
    // ================================================================

    // Bools if we've already lost or won.
    bool alreadyWon, alreadyLost;

    // ================================================================
    // State-changing methods
    // ================================================================

    public void TriggerWin()
    {
        // IE don't run anything if we're inactive.
        if (!alreadyWon && gameObject.activeInHierarchy) {
            alreadyWon = true;
            StartCoroutine(WinRoutine());
        }
    }
    IEnumerator WinRoutine()
    {
        print("You win!");
        winTriggered.Invoke();
        yield return new WaitForSeconds(2.5f);
        loader.LoadLevel(sceneOnWin);
    }

    public void TriggerLoss()
    {
        // IE don't run anything if we're inactive.
        if (!alreadyLost && gameObject.activeInHierarchy) {
            alreadyLost = true;
            StartCoroutine(LossRoutine());
        }
    }
    IEnumerator LossRoutine()
    {
        print("You lost!");
        lossTriggered.Invoke();
        yield return new WaitForSeconds(5f);
        loader.ReloadLevel();
    }

    // ================================================================
    // Debug methods
    // ================================================================

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 50, 50), "Win!"))
        {
            TriggerWin();
        }
        if (GUI.Button(new Rect(10, 70, 50, 50), "Lose!"))
        {
            TriggerLoss();
        }
    }
}
