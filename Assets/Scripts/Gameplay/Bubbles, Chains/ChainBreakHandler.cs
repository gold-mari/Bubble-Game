using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ChainEvent : UnityEvent<Chain>
{
}

public class ChainBreakHandler : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================

    [SerializeField, Tooltip("A UnityEvent which communicates with other scripts, noting that a chain has just broken.")]
    private ChainEvent chainBreak;

    // ==============================================================
    // Event-handling methods
    // ==============================================================

    public void ShoutChainBreak(Chain chain)
    {
        // ShoutChainBreak is called from Chain.cs whenever a chain breaks. In the
        // inspector, we use it to update the character animator.
        // ================

        chainBreak?.Invoke(chain);
    }
}
