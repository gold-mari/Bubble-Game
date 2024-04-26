using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
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
    
    // Whether or not the game is actively paused.
    private bool paused;
    [SerializeField, ReadOnly, Tooltip("Whether or not the game is actively paused.")]
    private bool canPause;

    // Stores whether the application has first focused. When we
    // recieve a focus call in OnApplicationFocused, if this is false,
    // we toggle it to true and do nothing else. This prevents the pause 
    // menu from displaying on startup.
    private static bool firstFocused = false;

    private void Awake()
    {
        // Awake is called before start.
        // ================

        canPause = canPauseOnEnter;
    }

    private void Update()
    {
        // Update is called once per frame. We use it to detect a pause key input.
        // ================

        if (canPause)
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

    /*public void CanPause(bool status)
    {
        // 
        // ================
    }*/

    protected void OnApplicationFocus()
    {
        // Detects when the application pauses or plays. If it changes at
        // any point, set to paused. No funny business.
        // ================

        if (!firstFocused) {
            firstFocused = true;
            return;
        }
        
        Pause(true);
    }
}
