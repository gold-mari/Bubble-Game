using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoRingManager : BeatWarning
{
    [SerializeField, Tooltip("The ring object to spawn.")]
    GameObject echoRing;

    protected override void OnMinorFlash()
    {
        // From base class. Called if the next beat is a single spawn.
        // ================

        Instantiate(echoRing, transform);
    }

    protected override void OnMajorFlash()
    {
        // From base class. Called if the second next beat OR the next beat is a higher-order beat 
        // (not single, not null).
        // ================

        Instantiate(echoRing, transform);
    }
}
