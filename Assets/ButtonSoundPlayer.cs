using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSoundPlayer : MonoBehaviour
{
    [SerializeField, Tooltip("The SFX played on hovering over a button.")]
    private FMODUnity.EventReference menuHoverSFX;

    [SerializeField, Tooltip("The SFX played on clicking a button.")]
    private FMODUnity.EventReference menuClickSFX;
    

    private void Start()
    {
        Button[] buttons = GetComponentsInChildren<Button>(includeInactive:true);

        // Define our pointerEnterEvent to play the menu hover SFX.
        EventTrigger.Entry pointerEnterEvent = new(){ eventID = EventTriggerType.PointerEnter };
        pointerEnterEvent.callback.AddListener((eventData) => { 
            playMenuHoverSFX();
        });

        // Look through each button.
        foreach (Button button in buttons) {
            button.onClick.AddListener(playMenuClickSFX);

            // Find the EventTrigger on this object, or make one if one doesn't exist.
            if (!button.TryGetComponent<EventTrigger>(out var trigger)) {
                trigger = button.gameObject.AddComponent(typeof(EventTrigger)) as EventTrigger;
            }

            // Add our pointerEnterEvent from earlier.
            trigger.triggers.Add(pointerEnterEvent);
        }
    }

    public void playMenuHoverSFX()
    {
        FMOD.Studio.EventInstance playedInstance = FMODUnity.RuntimeManager.CreateInstance(menuHoverSFX);
        //playedInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        playedInstance.start();
        playedInstance.release();  
    }

    public void playMenuClickSFX()
    {
        FMOD.Studio.EventInstance playedInstance = FMODUnity.RuntimeManager.CreateInstance(menuClickSFX);
        //playedInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        playedInstance.start();
        playedInstance.release();  
    }
}
