using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Chain : ScriptableObject
{
    [ReadOnly]
    public List<Bubble> members;
    [ReadOnly]
    public uint length;

    // ================================================================
    // Constructors and Destructors
    // ================================================================

    public void Initialize(Bubble bubble)
    {
        // Single member initializer.
        // ================
        members = new List<Bubble>() { bubble };
        length = 1;
    }

    void OnDestroy()
    {
        members.Clear();
    }

    // ================================================================
    // Member functions
    // ================================================================

    // Update is called once per frame
    void Update()
    {
        
    }
}
