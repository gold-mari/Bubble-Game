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
    private int sceneOnLose;
    [Tooltip("The scene we transition to on a win- reaching the end of the song.")]
    [SerializeField] [Scene]
    private int sceneOnWin;
    [Tooltip("A UnityEvent which communicates with CharacterAnimator that we have just won.")]
    [SerializeField]
    UnityEvent winTriggered;
    [Tooltip("A UnityEvent which communicates with CharacterAnimator that we have just lost.")]
    [SerializeField]
    UnityEvent lossTriggered;

    // ================================================================
    // State-changing methods
    // ================================================================

    public void TriggerWin()
    {
        StartCoroutine(WinRoutine());
    }
    IEnumerator WinRoutine()
    {
        print("You win!");
        winTriggered.Invoke();
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene(sceneOnWin);
    }

    public void TriggerLoss()
    {
        StartCoroutine(LossRoutine());
    }
    IEnumerator LossRoutine()
    {
        print("You lose!");
        lossTriggered.Invoke();
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene(sceneOnLose);
    }
}
