using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSoundPlayer : MonoBehaviour
{
    public FMODUnity.EventReference menuHoverSFX;
    [SerializeField, Tooltip("The SFX played on hovering over a button.")]

    public FMODUnity.EventReference menuClickSFX;
    [SerializeField, Tooltip("The SFX played on clicking a button.")]

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
