using System;
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
    [SerializeField, Tooltip("A UnityEvent which communicates with CharacterAnimator that we have just won.")]
    UnityEvent winTriggered;
    [SerializeField, Tooltip("The event banks run on a loss. Each bank is played after the previous one, via callbacks.")]
    private EndgameEventBank[] lossEventBank;

    // ================================================================
    // Internal variables
    // ================================================================

    // Bools if we've already lost or won.
    bool alreadyWon, alreadyLost;
    // Our current index in the current endgame event bank.
    int bankIndex = 0;

    // ================================================================
    // Update methods
    // ================================================================

    private void Update()
    {
        // Update is called once per frame. We use it to update our eventBanks and by extension the
        // endgame events.
        // ================

        if (alreadyLost)
        {
            lossEventBank[bankIndex].Update();
        }
    }

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
            //StartCoroutine(LossRoutine());

            // DEBUG DEBUG DEBUG //
            // DEBUG DEBUG DEBUG //
            // DEBUG DEBUG DEBUG //
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Lose");
            // DEBUG DEBUG DEBUG //
            // DEBUG DEBUG DEBUG //
            // DEBUG DEBUG DEBUG //

            lossEventBank[bankIndex].Run();
        }
    }

    // ================================================================
    // Callback methods
    // ================================================================

    public void EndgameEventCallback()
    {
        //
        // ================

        if (alreadyLost || alreadyWon)
        {
            bankIndex++;
            lossEventBank[bankIndex].Run();
        }
        else
        {
            Debug.LogError("EndgameManager error: EndgameEventCallback failed. No endgame is currently active.");
        }
    }
}

// ================================================================
// Helper classes
// ================================================================

[Serializable]
public class EndgameEvent : UnityEvent<EndgameManager> {}

[Serializable]
public class DelayedEndgameEvent
{
    [Tooltip("The amount of time after Invoke() is called before the event is invoked.")]
    public float delay;
    [Tooltip("The event to invoke.")]
    public EndgameEvent ourEvent;

    // The endgame manager associated with this event.
    EndgameManager endgameManager = null;
    // A list of floats representing queued invocations of our event.
    private List<float> invokeQueue = new();

    public void Invoke(EndgameManager manager=null)
    {
        // Adds an invocation to the end of our queue. Takes an optional EndgameManager argument, 
        // acting as a lazy initializer.
        // ====================

        if(endgameManager = null) endgameManager = manager;
        invokeQueue.Add(delay);
    }

    public void Update()
    {
        // Updates the timers in our invoke queue.
        // IMPORTANT: MUST BE CALLED IN MONOBEHAVIOUR.UPDATE().
        // ================

        if (invokeQueue.Count > 0) {
            for (int i = 0; i < invokeQueue.Count; i++) 
            {
               invokeQueue[i] -= Time.deltaTime;
            }
            while (invokeQueue.Count > 0 && invokeQueue[0] <= 0)
            {
                ourEvent?.Invoke(endgameManager);
                invokeQueue.RemoveAt(0);
            }
        }
    }
}

[Serializable]
public class EndgameEventBank
{
    [SerializeField, Tooltip("The event which handles our callback within this bank.")]
    DelayedEndgameEvent callbackEvent;
    [SerializeField, Tooltip("Any other events called alongside our callbackEvent.")]
    DelayedEndgameEvent[] auxilliaryEvents;

    public void Run()
    {
        // Runs all the events in our bank.
        // ================

        callbackEvent.Invoke();
        foreach (DelayedEndgameEvent aux in auxilliaryEvents)
        {
            aux.Invoke();
        }   
    }

    public void Update()
    {
        callbackEvent.Update();
        foreach (DelayedEndgameEvent aux in auxilliaryEvents)
        {
            aux.Update();
        }
    }
}
