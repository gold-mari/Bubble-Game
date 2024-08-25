using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSoundPlayer : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================

    [SerializeField, Tooltip("The SFX played on hovering over a button.")]
    private FMODUnity.EventReference menuHoverSFX;

    [SerializeField, Tooltip("The SFX played on clicking a button.")]
    private FMODUnity.EventReference menuClickSFX;
    
    // ==============================================================
    // Initializer / finalizer methods
    // ==============================================================

    private void Start()
    {
        SupplySFX();
    }

    public void SupplySFX()
    {
        // Looks through all buttons underneath us, and applies the button 
        // sounds to each of them.
        // ================

        Button[] buttons = GetComponentsInChildren<Button>(includeInactive: true);

        // Define our pointerEnterEvent to play the menu hover SFX.
        EventTrigger.Entry pointerEnterEvent = new() { eventID = EventTriggerType.PointerEnter };
        pointerEnterEvent.callback.AddListener((eventData) =>
        {
            PlayMenuHoverSFX();
        });

        // Look through each button.
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(PlayMenuClickSFX);

            // Find the EventTrigger on this object, or make one if one doesn't exist.
            if (!button.TryGetComponent<EventTrigger>(out var trigger))
            {
                trigger = button.gameObject.AddComponent(typeof(EventTrigger)) as EventTrigger;
            }

            // Add our pointerEnterEvent from earlier.
            trigger.triggers.Add(pointerEnterEvent);
        }
    }

    private void OnDestroy()
    {
        // Cleans up listeners and event triggers.
        // ================

        Button[] buttons = GetComponentsInChildren<Button>(includeInactive:true);

        // Look through each button.
        foreach (Button button in buttons) {
            button.onClick.RemoveAllListeners();

            // Find the EventTrigger on this object, or make one if one doesn't exist.
            if (button.TryGetComponent<EventTrigger>(out var trigger)) {
                Destroy(trigger);
            }
        }
    }

    // ==============================================================
    // SFX methods
    // ==============================================================

    public void PlayMenuClickSFX()
    {
        FMOD.Studio.EventInstance playedInstance = FMODUnity.RuntimeManager.CreateInstance(menuClickSFX);
        //playedInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        playedInstance.start();
        playedInstance.release();  
    }

    public void PlayMenuHoverSFX()
    {
        FMOD.Studio.EventInstance playedInstance = FMODUnity.RuntimeManager.CreateInstance(menuHoverSFX);
        //playedInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        playedInstance.start();
        playedInstance.release();  
    }
}
