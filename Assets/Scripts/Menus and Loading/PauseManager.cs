using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseManager : MonoBehaviour
{
    [SerializeField, Tooltip("The key which, when pressed, pauses the game.")]
    private KeyCode pauseKey = KeyCode.Escape;
    [SerializeField, Tooltip("Called when the pause state changes to true.")]
    public UnityEvent onPause;
    [SerializeField, Tooltip("Called when the pause state changes to false.")]
    public UnityEvent onUnpause;

    // Whether or not the game is actively paused.
    private bool paused;

    private void Update()
    {
        // Update is called once per frame. We use it to detect a pause key input.
        // ================

        if (!paused && Input.GetKeyDown(pauseKey))
        {
            Pause(true);
        }
        else if (paused && Input.GetKeyDown(pauseKey))
        {
            Pause(false);
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
}
