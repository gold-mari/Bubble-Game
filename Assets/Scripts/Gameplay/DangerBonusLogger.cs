using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerBonusLogger : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [SerializeField, Tooltip("The danger manager in the scene.")]
    private DangerManager dangerManager;
    [SerializeField, Tooltip("The bool var tracking whether or not we're in danger.")]
    private boolVar inDanger;
    [SerializeField, Tooltip("The uintVar that we write our safety bonus to.")]
    private uintVar dangerBonus;
    [SerializeField, Tooltip("The amount of points we gain per second when in danger.\n\nDefault: 100")]
    private float pointsPerSecond = 100;

    // ================================================================
    // Internal variables
    // ================================================================

    // The amount of time we have spent in danger.
    private float rawPoints = 0;
    // Whether or not the safety is locked. Don't track time in danger if it is.
    private bool safetyLocked = false;

    // Start is called before the first frame update
    void Start()
    {
        dangerBonus.value = 0;

        dangerManager.SafetyLockChanged += UpdateSafetyLock;
    }

    void Update()
    {
        // Update is called once per frame. We use it to update our dangerBonus value.
        // ================

        // If we have time left to count AND we're unlocked AND we're in danger...
        if (!safetyLocked && inDanger.value) {
            rawPoints += pointsPerSecond*Time.deltaTime;
            dangerBonus.value = (uint)rawPoints;
        }
    }

    private void UpdateSafetyLock(bool value)
    {
        safetyLocked = value;
    }
}
