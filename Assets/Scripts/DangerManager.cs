using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class DangerManager : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [Tooltip("The floatVar, ranging from 0 to 1, which signals how close we are to a game over.")]
    [SerializeField] [Expandable]
    private floatVar dangerAmount;
    // The number of bubbles in danger. If this is above 0, dangerAmount steadily increases.
    [SerializeField] [ReadOnly]
    private uint bubblesInDanger = 0;

    // ================================================================
    // Default methods
    // ================================================================

    void Start()
    {
        // Start is called before the first frame update. We use it to initialize dangerAmount.
        // ================

        dangerAmount.value = 0;
    }

    // ================================================================
    // Data-manipulation methods
    // ================================================================

    public void Increment()
    {
        // Adds 1 to bubblesInDanger.
        // ================

        bubblesInDanger++;
    }

    public void Decrement()
    {
        // Subtracts 1 from bubblesInDanger.
        // ================

        bubblesInDanger--;
    }

    public void ZeroOut()
    {
        // Sets bubblesInDanger to 0.
        // ================

        bubblesInDanger = 0;
    }
}
