using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public class EndgameManager : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [Tooltip("The scene we transition to on a loss- reaching max danger.")]
    [SerializeField] [Scene]
    private string sceneOnLose;
    [Tooltip("The scene we transition to on a win- reaching the end of the song.")]
    [SerializeField] [Scene]
    private string sceneOnWin;
    [Tooltip("A UnityEvent which communicates with CharacterAnimator that we have just won.")]
    [SerializeField]
    UnityEvent winTriggered;
    [Tooltip("A UnityEvent which communicates with CharacterAnimator that we have just lost.")]
    [SerializeField]
    UnityEvent lossTriggered;

    // Bools if we've already lost or won.
    bool alreadyWon, alreadyLost;

    // ================================================================
    // State-changing methods
    // ================================================================

    public void TriggerWin()
    {
        // IE don't run anything if we're inactive.
        if (!alreadyWon && gameObject.activeInHierarchy) {
            StartCoroutine(WinRoutine());
        }
    }
    IEnumerator WinRoutine()
    {
        print("You win!");
        winTriggered.Invoke();
        yield return new WaitForSeconds(6f);
        SceneManager.LoadScene(sceneOnWin);
    }

    public void TriggerLoss()
    {
        // IE don't run anything if we're inactive.
        if (!alreadyLost && gameObject.activeInHierarchy) {
            StartCoroutine(LossRoutine());
        }
    }
    IEnumerator LossRoutine()
    {
        print("You lost!");
        lossTriggered.Invoke();
        yield return new WaitForSeconds(6f);
        SceneManager.LoadScene(sceneOnLose);
    }
}
