using System;
using System.Collections;
using System.Timers;
using UnityEngine;

public class VictoryStatDisplay : uintVarMonitor 
{
    [Header("VictoryStatDisplay Parameters")]
    [SerializeField, Tooltip("The amount of time, in seconds, it takes this object to tick up.")]
    private float tickTime = 1;
    [SerializeField, Tooltip("The exponential power applied to our lerp functions.\n\nDefault: 2")]
    private float lerpPower = 2;

    // The lerp amount used with GetUIInt.
    private float lerpAmount;

    // ================================================================
    // Startup methods
    // ================================================================

    private new void Start()
    {
        base.Start();
    }

    // ================================================================
    // Text-getting methods
    // ================================================================

    protected override uint GetUInt()
    {
        // Returns the value of our UIntVar, used in Update().
        // Overridden in child classes.
        // ================

        return (uint)(UIntVar.value * lerpAmount);
    }

    // ================================================================
    // Accessor methods
    // ================================================================

    public bool IsDone()
    {
        return lerpAmount == 1;
    }

    public void StartTicking()
    {
        StartCoroutine(DisplayTickUpRoutine());
    }

    private IEnumerator DisplayTickUpRoutine() 
    {
        float elapsed = 0;
        lerpAmount = 0;

        while (elapsed < tickTime) {
            lerpAmount = LerpKit.EaseInOut(elapsed/tickTime, lerpPower);
            elapsed += Time.deltaTime;
            yield return null;
        }

        lerpAmount = 1;
        Debug.Log("VictoryStatDisplay: Done lerping.", this);
    }
}