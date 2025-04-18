using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaiveScreenshake : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================

    [SerializeField, Tooltip("The magnitude of the base screenshake, defined in maximum worldspace units from the base position."
                            +"\n\nDefault: 0.05")]
    private float baseShakeMagnitude = 0.05f;
    [Tooltip("The duration of the base screenshake, in seconds.n\nDefault: 0.5")]
    public float BaseShakeDuration { get; private set; } = 0.5f;

    [SerializeField, Range(0,1), Tooltip("The magnitude of the gamepad's low-frequency rumble.+\n\nDefault: 0.5")]
    private float lowGamepadRumble = 0.5f;
    [SerializeField, Range(0,1), Tooltip("The magnitude of the gamepad's high-frequency rumble.+\n\nDefault: 0.25")]
    private float highGamepadRumble = 0.25f;
    [SerializeField, Min(1), Tooltip("How quickly rumble magnitude falls off on screenshake. "
                                   + "Higher numbers are faster.\n\nDefault: 6")]
    private float rumbleFalloff = 6f;

    // ==============================================================
    // Internal variables
    // ==============================================================

    private floatVar shakeScaling;
    private Vector3 basePosition;
    private Coroutine shakeRoutine;

    // ==============================================================
    // Initializer methods
    // ==============================================================

    void Awake()
    {
        // Awake is called before Start.
        // ================

        basePosition = transform.position;
    }

    public void SetShakeScalingVar(floatVar _shakeScaling)
    {
        shakeScaling = _shakeScaling;
    }

    // ==============================================================
    // Shake methods
    // ==============================================================

    public void BaseShake()
    {
        // Calls a basic version of our shakeRoutine.
        // ================

        if (shakeRoutine != null) 
        {
            StopCoroutine(shakeRoutine);
        }
        shakeRoutine = StartCoroutine(Shake(baseShakeMagnitude, BaseShakeDuration));
    }

    public void ScaledShake(float scale)
    {
        // Calls a version of our shakeRoutine with a magnitude scaled by our float parameter.
        // ================

        if (shakeRoutine != null) 
        {
            StopCoroutine(shakeRoutine);
        }
        shakeRoutine = StartCoroutine(Shake(scale * baseShakeMagnitude, BaseShakeDuration));
    }

    private IEnumerator Shake(float magnitude, float duration)
    {
        // Applies the shake transformation to our camera.
        // ================

        magnitude *= shakeScaling.value; // Accessibility setting.

        float elapsed = 0;
        while (elapsed < duration)
        {
            if (Time.timeScale != 0) {
                float currentMagnitude = Mathf.Lerp(magnitude, 0, elapsed/duration);
                // Give us a random unit vector multiplied by our currentMagnitude.
                Vector3 offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized * currentMagnitude;

                float rumbleAmount = LerpKit.EaseIn(1-elapsed/duration, rumbleFalloff);
                InputHandler.SetRumble(lowGamepadRumble*rumbleAmount, highGamepadRumble*rumbleAmount);
                
                transform.position = basePosition + offset;
                elapsed += Time.deltaTime;
            }

            yield return null;
        }

        InputHandler.SetRumble(0f, 0f);

        transform.position = basePosition;
    }
}
