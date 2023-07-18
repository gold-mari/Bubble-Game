using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Bubble : MonoBehaviour
{
    public enum Color {NONE, Red};

    // ================================================================

    // Bubbles of like colors are eligible to join in chains together.
    public Color bubbleColor = Color.Red;
    // When a collision occurs, to avoid doublecounting, the Bubble with the oldest/smallest age
    // runs the calculation.
    public uint age = 0;
    // The chain this bubble is a part of. When new bubbles spawn, they are by default of the NIL
    // chain. This is assigned elsewhere.
    [Expandable]
    public Chain chain;
    // The list of adjacent bubbles to this one. Updated OnCollisionEnter and OnCollisionExit,
    // among other places.
    public List<Bubble> adjacencies = new List<Bubble>();

    // ================================================================
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        // Detects collision starts between our collider and other colliders. Used to
        // update adjacencies and to consolidate bubbles into chains, if needed.
        // ================

        Bubble otherBubble = other.gameObject.GetComponent<Bubble>();
        // If the otherBubble is not null...
        if ( otherBubble ) {
            // Add them to our adjacency list.
            AddAdjacency(otherBubble);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        // Detects collision endings between our collider and other colliders. Used to
        // update adjacencies and to distribute chains, if needed.
        // ================

        Bubble otherBubble = other.gameObject.GetComponent<Bubble>();
        // If the otherBubble is not null...
        if ( otherBubble ) {
            // Remove them from our adjacency list.
            RemoveAdjacency(otherBubble);
        }
    }

    private void AddAdjacency(Bubble other)
    {
        // Adds Bubble other to this's adjacency list. If the bubbles are of the same
        // color, also consolidates their chains.
        // ================

        adjacencies.Add(other);
        // If they are of the same color as us, AND we're the younger bubble...
        if ( other.bubbleColor == bubbleColor && age > other.age ) {
            // Run the Consolidation algorithm on us, looking at them.
            Consolidate(other);
        }
        // If we're of different colors or we're the older bubble, do nothing.
    }

    public void RemoveAdjacency(Bubble other)
    {
        // Removes Bubble other to this's adjacency list. If the bubbles are of the same
        // color, also distributes their chain.
        // ================

        adjacencies.Remove(other);
        // If they are of the same color as us, AND we're the younger bubble...
        if ( other.bubbleColor == bubbleColor && age > other.age ) {
            // AND we're in the same chain...
            if ( chain == other.chain) {
                // Run the Distribution algorithm on our chain.
                chain.Distribute();
            }
        }
    }

    private void Consolidate(Bubble other)
    {
        // Consolidates two bubbles with like colors in a collision, this and other,
        // into a single chain. Due to how this function is called in AddAdjacency,
        // this will be younger than other.
        // Prerequisites: this.chain != null, other.chain != null
        // ====================

        Debug.Assert( this.chain != null, "Bubble Error: Consolidate() failed: this's chain must not be null.", this );
        Debug.Assert( other.chain != null, "Bubble Error: Consolidate() failed: other's chain must not be null.", other );

        // We must check if either bubble has a nil chain. If so, we must make it a chain
        // first. The nil chain, and ONLY the nil chain, has a length of 0. We can thus
        // check for length == 0 as a subsitute.
        if (chain.length == 0) {
            chain = ScriptableObject.CreateInstance<Chain>();
            chain.AddBubble(this);
        }
        if (other.chain.length == 0) {
            other.chain = ScriptableObject.CreateInstance<Chain>();
            other.chain.AddBubble(other);
        }

        // Begin by comparing chain length. The shorter one will be added to the longer one.
        // If there is a tie, add from this to other.

        Chain shorter, longer;
        if (chain.length > other.chain.length) {
            shorter = other.chain;
            longer = chain;
        }
        else { // if (chain.length <= other.chain.length)
            shorter = chain;
            longer = other.chain;
        }

        // Append the contents of shorter to the end of longer.
        longer.Concatenate(shorter);

        // Finally, unload the old chain if it is now unused.
        Resources.UnloadUnusedAssets();
    }
}
