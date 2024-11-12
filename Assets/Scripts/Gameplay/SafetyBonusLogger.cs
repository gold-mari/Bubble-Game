using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyBonusLogger : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [SerializeField, Tooltip("The danger manager in the scene.")]
    private DangerManager dangerManager;
    [SerializeField, Tooltip("The bool var tracking whether or not we're in danger.")]
    private boolVar inDanger;
    [SerializeField, Tooltip("The uintVar that we write our safety bonus to.")]
    private uintVar safetyBonus;
    [SerializeField, Tooltip("The default number of points for the safety bonus, if no time is spent in danger." +
                             "\n\nDefault: 9999")]
    private uint initialSafetyBonus = 9999;
    [SerializeField, Tooltip("The amount of time, in seconds, we can be in danger before losing all our safety bonus." +
                             "\n\nDefault: 10")]
    private float timeAllowedInDanger = 10;

    // ================================================================
    // Internal variables
    // ================================================================

    // The amount of time we have spent in danger.
    private float timeInDanger = 0;
    // Whether or not the safety is locked. Don't track time in danger if it is.
    private bool safetyLocked = false;

    // Start is called before the first frame update
    void Start()
    {
        safetyBonus.value = initialSafetyBonus;

        dangerManager.SafetyLockChanged += UpdateSafetyLock;
    }

    void Update()
    {
        // Update is called once per frame. We use it to update our safetyBonus value.
        // ================

        // If we have time left to count AND we're unlocked AND we're in danger...
        if (timeInDanger < timeAllowedInDanger && !safetyLocked && inDanger.value) {
            timeInDanger += Time.deltaTime;

            float lerpAmount = Mathf.Clamp(timeInDanger/timeAllowedInDanger, 0, 1);
            safetyBonus.value = (uint)Mathf.Lerp(initialSafetyBonus, 0, lerpAmount);
        }
    }

    private void UpdateSafetyLock(bool value)
    {
        safetyLocked = value;
    }
}
