using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

public class PauseManager : MonoBehaviour
{
    [SerializeField, Tooltip("The key which, when pressed, pauses the game.")]
    private KeyCode[] pauseKeys;
    [SerializeField, Tooltip("Called when the pause state changes to true.")]
    public UnityEvent onPause;
    [SerializeField, Tooltip("Called when the pause state changes to false.")]
    public UnityEvent onUnpause;
    [SerializeField, Tooltip("Whether or not we can pause when entering this scene.\n\nDefault: true")]
    public bool canPauseOnEnter = true;
    [SerializeField, Tooltip("Whether or not the game should unpause when regaining application focus.\n\nDefault: false")]
    public bool unpauseOnRefocus = false;
    
    [SerializeField, ReadOnly, Tooltip("Whether or not the game is currently able to be paused.")]
    private bool pauseLocked;
    [SerializeField, ReadOnly, Tooltip("Whether or not the game is actively paused.")]
    private bool paused;

    private void Awake()
    {
        // Awake is called before start.
        // ================

        // Default state is always unpaused.
        // If we can't pause on enter, lock to unpaused.
        pauseLocked = !canPauseOnEnter;
    }

    private void Update()
    {
        // Update is called once per frame. We use it to detect a pause key input.
        // ================

        if (!pauseLocked)
        {
            foreach (KeyCode key in pauseKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    Pause(!paused);
                    return;
                }
            }
        }
    }

    private void OnDestroy()
    {
        // If we get destroyed (by changing scenes, for example), unpause.
        // ================

        Pause(false);
    }

    // protected void OnApplicationPause(bool shouldPause)
    // {
    //     // Detects when the application pauses or plays. If the app pauses, pause the game.
    //     // ================

    //     if (shouldPause) {
    //         Pause(true);
    //     }
    // }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) {
            Pause(true);
        }

        if (hasFocus && unpauseOnRefocus) {
            Pause(false);
        }
    }

    public void Pause(bool pauseStatus)
    {
        // Sets timeScale to 0 or 1, depending on the pause status.
        // Also invokes our actions.
        // ================

        paused = pauseStatus;

        // Pauses / unpauses the game.
        if (pauseStatus)
        {
            Time.timeScale = 0;
            onPause.Invoke();
        }
        else
        {
            Time.timeScale = 1;
            onUnpause.Invoke();
        }
    }

    public void LockPause(bool status)
    {
        // Sets whether or not we can change the pause state by user input.
        // ================

        pauseLocked = status;
    }
}
