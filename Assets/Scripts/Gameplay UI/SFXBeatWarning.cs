using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXBeatWarning : BeatWarning
{
    // ================================================================
    // Parameters
    // ================================================================

    [SerializeField, Tooltip("The SFX played before a minor flash.")]
    private FMODUnity.EventReference minorSFX;

    // ================================================================
    // Update methods
    // ================================================================

    protected override void OnMinorFlash()
    {
        // From base class. Called if the next beat is a single spawn.
        // ================

        FMODUnity.RuntimeManager.PlayOneShot(minorSFX);
    }
}
