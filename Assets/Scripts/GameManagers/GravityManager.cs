using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GravityManager : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [Tooltip("The MAGNITUDE strength of gravity, applied as a scalar to the strengthVar.\n\n"
           + "Default: 1\n\nIMPORTANT: gravityStrength should have a minimum value of 0.")]
    [MinValue(0.0f)]
    public float gravityStrength = 1;
    [Tooltip("The floatVar representing the current, vector strength of gravity. Negative is "
           + "inwards, positive is outwards.")]
    public floatVar strengthVar;
    [Tooltip("The boolVar which signals if gravity is flipped to point outwards instead of inwards.")]
    public boolVar gravityFlipped;

    // ================================================================
    // Default methods
    // ================================================================

    void Start()
    {
        // Start is called before the first frame update. We use it to initialize 
        // strengthVar to negative gravityStrength, and to set gravityFlipped to false.
        // ================

        strengthVar.value = -gravityStrength;
        gravityFlipped.value = false;
    }

    // ================================================================
    // Data-manipulation methods
    // ================================================================

    public void FlipGravity()
    {
        // A small helper function called elsewhere via events. Flips the direction of
        // gravity, and notes that we flipped it. Also plays a sound effect, for now.
        // ================

        // DEBUG???
        // DEBUG!!!
        // i got a glock in my rari
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/GravityFlip");
        // DEBUG???
        // DEBUG!!!
        // i got a glock in my rari

        strengthVar.value *= -1;
        gravityFlipped.value = !gravityFlipped.value;
    }
}
