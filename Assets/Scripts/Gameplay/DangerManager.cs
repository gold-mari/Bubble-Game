using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class DangerManager : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================
    
    [Tooltip("The Endgame Manager present in the scene.")]
    [SerializeField] 
    private EndgameManager endgameManager;
    [Tooltip("The floatVar, ranging from 0 to 1, which signals how close we are to a game over.")]
    [SerializeField] [Expandable]
    private floatVar dangerAmount;
    [Tooltip("The float threshold compared against dangerAmount to determine if inDanger is true."
           + "\n\nDefault: 0.2f")]
    [SerializeField]
    private float dangerThreshold = 0.2f;
    [Tooltip("The boolVar, corresponding to if dangerAmount is greater than or equal to "
           + "dangerThreshold. Communicated to animators and other sources.")]
    [SerializeField]
    private boolVar inDanger;
    [Tooltip("The number of bubbles in danger. If this is above 0, dangerAmount steadily increases.")]
    [SerializeField] [ReadOnly]
    private uint bubblesInDanger = 0;   // The field.
    private uint BubblesInDanger {      // The property.
        get 
        {
            return bubblesInDanger;
        }
        set 
        {
            bubblesInDanger = value;
            BubblesInDangerUpdated();
        }
    }
    [Tooltip("The time it takes to go from 0 to 1 dangerAmount when bubblesInDanger becomes nonzero."
           + "\n\nDefault: 5")]
    [SerializeField]
    private float inDangerLerpTime = 5f;
    [Tooltip("The time it takes to go from 1 to 0 dangerAmount when bubblesInDanger becomes zero."
           + "\n\nDefault: 0.667")]
    [SerializeField] 
    private float outOfDangerLerpTime = 0.667f;

    // Used to announce the the safety lock value has changed.
    public System.Action<bool> SafetyLockChanged;

    // ================================================================
    // Internal variables
    // ================================================================

    private bool safetyLock = false;

    // ================================================================
    // Default methods
    // ================================================================

    void Start()
    {
        // Start is called before the first frame update. We use it to initialize dangerAmount.
        // ================

        dangerAmount.value = 0;
        inDanger.value = false;
    }

    void Update()
    {
        // Update is called once per frame. We use it to update inDanger based on the
        // value of dangerAmount and dangerThreshold.
        // ================

        if (dangerAmount.value >= dangerThreshold) {
            inDanger.value = true;
        }
        else {  // dangerAmount.value < dangerThreshold
            inDanger.value = false;
        }
    }

    // ================================================================
    // Data-manipulation methods
    // ================================================================

    public void Increment()
    {
        // Adds 1 to bubblesInDanger.
        // ================

        BubblesInDanger++;
    }

    public void Decrement()
    {
        // Subtracts 1 from bubblesInDanger.
        // ================

        BubblesInDanger--;
    }

    public void ZeroOut()
    {
        // Sets bubblesInDanger to 0, dangerAmount to 0, and stops all coroutines.
        // ================

        BubblesInDanger = 0;
        StopAllCoroutines();
        dangerAmount.value = 0;
    }

    private void BubblesInDangerUpdated()
    {
        // Called when BubblesInDanger is updated. If BubblesInDanger is now over 0, run
        // the InDanger routine. If it is now equal to 0, run the OutOfDanger routine.
        // ================

        // If we're in danger, start the InDanger routine.
        if (BubblesInDanger > 0) {
            StopAllCoroutines();
            // This coroutine has a nasty habit of starting on scene transition and
            // raising errors. No clue why. Put a check in to fix it.
            if (gameObject.activeInHierarchy) {
                StartCoroutine(InDangerRoutine());
            }
        }
        // If we're out of danger, start the OutOfDanger routine.
        else if (BubblesInDanger == 0) {
            StopAllCoroutines();
            // This coroutine has a nasty habit of starting on scene transition and
            // raising errors. Put a check in to fix it.
            if (gameObject.activeInHierarchy) {
                StartCoroutine(OutOfDangerRoutine());
            }
        }
    }

    IEnumerator InDangerRoutine() 
    {
        // Lerps dangerAmount up from 0 to 1 in maximum inDangerLerpTime.
        // ================

        // We may be starting from a nonzero dangerAmount. Calculate how much time has
        // already elapsed from 0.
        // IMPORTANT: Enclose this calculation in the same LerpKit function used in the
        // Lerp function in the while loop.
        float elapsed = dangerAmount.value * inDangerLerpTime;
        
        while (elapsed < inDangerLerpTime) {
            // If the safety is locked, don't allow for danger to change.
            if (!safetyLock) {
                dangerAmount.value = Mathf.Lerp(0, 1, elapsed/inDangerLerpTime);
                elapsed += Time.deltaTime;
            }
            yield return null;
        }

        // Finally, set dangerAmount to 1 to smooth over floating point inconsistencies.
        dangerAmount.value = 1;
        FullDanger();
        StopAllCoroutines();
    }

    private void FullDanger()
    {
        // A method to be run at the end of InDangerRoutine, when dangerAmount is 1.
        // ================

        if (endgameManager) {
            endgameManager.TriggerLoss();
        }
    }

    IEnumerator OutOfDangerRoutine() 
    {
        // Lerps dangerAmount down from 1 to 0 in maximum outOfDangerLerpTime.
        // ================

        // We may be starting from a nonone dangerAmount. Calculate how much time has
        // already elapsed from 1.
        // IMPORTANT: Enclose this calculation in the same LerpKit function used in the
        // Lerp function in the while loop.
        float elapsed = LerpKit.Flip(dangerAmount.value) * outOfDangerLerpTime;
        
        while (elapsed < outOfDangerLerpTime) {
            // If the safety is locked, don't allow for danger to change.
            if (!safetyLock) {
                dangerAmount.value = Mathf.Lerp(1, 0, elapsed/outOfDangerLerpTime);
                elapsed += Time.deltaTime;
            }
            yield return null;
        }

        // Finally, set dangerAmount to 0 to smooth over floating point inconsistencies.
        dangerAmount.value = 0;
        StopAllCoroutines();
    }

    public void SetSafetyLock(bool value)
    {
        // Public accessor for the safetyLock variable.
        // ================
        
        safetyLock = value;
        SafetyLockChanged?.Invoke(value);
    }
}
