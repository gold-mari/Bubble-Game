using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChainBreakHandler : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================

    [Tooltip("A UnityEvent which communicates with ..., noting that a chain has just broken.")]
    [SerializeField]
    UnityEvent chainBreak;

    // ==============================================================
    // Event-handling methods
    // ==============================================================

    public void ShoutChainBreak()
    {
        // ShoutChainBreak is called from Chain.cs whenever a chain breaks. In the
        // inspector, we use it to update the character animator.
        // ================

        chainBreak?.Invoke();
    }
}
