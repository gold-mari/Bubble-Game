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
    [SerializeField, Tooltip("The path of the FMOD nonmenu SFX bus. Find it in the mixer by right clicking the " +
                             "NonMenuSFX group and selecting Copy Path.")]
    private string nonmenuSFXBusPath = "bus:/SFX/NonMenuSFX";
    [SerializeField, Tooltip("The path of the FMOD music bus. Find it in the mixer by right clicking the " +
                             "NonMenuSFX group and selecting Copy Path.")]
    private string musicBusPath = "bus:/Music";
    [SerializeField, Tooltip("Called when the pause state changes to true.")]
    private UnityEvent onPause;
    [SerializeField, Tooltip("Called when the pause state changes to false.")]
    private UnityEvent onUnpause;
    [SerializeField, Tooltip("Whether or not we can pause when entering this scene.\n\nDefault: true")]
    private bool canPauseOnEnter = true;
    [SerializeField, Tooltip("Whether or not the game should unpause when regaining application focus.\n\nDefault: false")]
    private bool unpauseOnRefocus = false;
    
    [SerializeField, ReadOnly, Tooltip("Whether or not the game is currently able to be paused.")]
    private bool pauseLocked;
    [SerializeField, ReadOnly, Tooltip("Whether or not the game is actively paused.")]
    private bool paused;

    private FMOD.Studio.Bus sfxBus;
    private FMOD.Studio.Bus musicBus;

    private void Awake()
    {
        // Awake is called before start.
        // ================

        // Default state is always unpaused.
        // If we can't pause on enter, lock to unpaused.
        pauseLocked = !canPauseOnEnter;

        sfxBus = FMODUnity.RuntimeManager.GetBus(nonmenuSFXBusPath);
        musicBus = FMODUnity.RuntimeManager.GetBus(musicBusPath);
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
            sfxBus.setPaused(true);
            onPause.Invoke();
            musicBus.setPaused(true);
        }
        else
        {
            Time.timeScale = 1;
            sfxBus.setPaused(false);
            onUnpause.Invoke();
            musicBus.setPaused(false);
        }
    }

    public void LockPause(bool status)
    {
        // Sets whether or not we can change the pause state by user input.
        // ================

        pauseLocked = status;
    }
}
