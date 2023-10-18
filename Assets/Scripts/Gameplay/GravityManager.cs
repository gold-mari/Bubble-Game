using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GravityManager : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [Tooltip("The beat reader object in the scene.")]
    public BeatReader beatReader;
    [Tooltip("The MAGNITUDE strength of gravity, applied as a scalar to the strengthVar.\n\n"
           + "Default: 1\n\nIMPORTANT: gravityStrength should have a minimum value of 0.")]
    [MinValue(0.0f)]
    public float gravityStrength = 1;
    [Tooltip("The floatVar representing the current, vector strength of gravity. Negative is "
           + "inwards, positive is outwards.")]
    public floatVar strengthVar;
    [Tooltip("The boolVar which signals if gravity is flipped to point outwards instead of inwards.")]
    public boolVar gravityFlipped;

    [SerializeField, Tooltip("The SFX played on gravity flip.")]
    FMODUnity.EventReference gravityFlipSFX;
    [SerializeField, Tooltip("The SFX played the beat before gravity flip.")]
    FMODUnity.EventReference beforeGravityFlipSFX;

    // ================================================================
    // Internal variables
    // ================================================================

    private FMOD.Studio.EventInstance gravityFlipSFX_i, beforeGravityFlipSFX_i;

    // ================================================================
    // Default methods
    // ================================================================

    private void Awake()
    {
        // Awake is called before Start. We use it to subscribe to beatReader.
        // ================

        beatReader.flipGravity += OnFlipGravity;
        beatReader.beforeFlipGravity += OnBeforeFlipGravity;
    }

    private void Start()
    {
        // Start is called before the first frame update. We use it to initialize 
        // strengthVar to negative gravityStrength, and to set gravityFlipped to false.
        // ================

        strengthVar.value = -gravityStrength;
        gravityFlipped.value = false;

        // Create our event isntances.
        gravityFlipSFX_i = FMODUnity.RuntimeManager.CreateInstance(gravityFlipSFX);
        beforeGravityFlipSFX_i = FMODUnity.RuntimeManager.CreateInstance(beforeGravityFlipSFX);
    }

    private void OnDestroy()
    {
        // Called when this object is destroyed. We use it to unsubscribe from beatReader.
        // ================

        beatReader.flipGravity -= OnFlipGravity;
        beatReader.beforeFlipGravity -= OnBeforeFlipGravity;

        gravityFlipSFX_i.release();
        beforeGravityFlipSFX_i.release();
    }

    // ================================================================
    // Data-manipulation methods
    // ================================================================

    private void OnFlipGravity()
    {
        // A small helper function called elsewhere via events. Flips the direction of
        // gravity, and notes that we flipped it. Also plays a sound effect, for now.
        // ================

        gravityFlipSFX_i.start();

        strengthVar.value *= -1;
        gravityFlipped.value = !gravityFlipped.value;
    }
    private void OnBeforeFlipGravity()
    {
        // Plays a sound effect when we recieve the pre_FlipGravity event.
        // ================

        beforeGravityFlipSFX_i.start();
    }

}
