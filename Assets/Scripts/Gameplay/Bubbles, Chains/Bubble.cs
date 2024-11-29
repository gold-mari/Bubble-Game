using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Bubble : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================

    // Bubbles of like flavors are eligible to join in chains together.
    [Tooltip("This bubble's flavor, represented as a Bubble_Flavor variable.")]
    public BubbleFlavor bubbleFlavor = BubbleFlavor.Sweet;
    // When a collision occurs, to avoid doublecounting, the Bubble with the oldest/smallest age
    // runs the calculation.
    [Tooltip("This bubble's age, assigned normally by BubbleSpawner, and used to prevent double "
           + "calculations occuring during collisions.\n\nExposed here for debug use, if needed.")]
    public uint age = 0;
    // The chain this bubble is a part of. When new bubbles spawn, they are by default of the NIL
    // chain. This is assigned elsewhere.
    [Tooltip("This bubble's Chain, assigned in collisions.\n\nDefault: the NIL chain.")]
    [Expandable]
    public Chain chain;
    // The list of adjacent bubbles to this one. Updated OnCollisionEnter and OnCollisionExit,
    // among other places.
    [Tooltip("The bubbles which are adjacent to this one. Exposed for debug use.")]
    [ReadOnly]
    public List<Bubble> adjacencies = new List<Bubble>();
    [HideInInspector]
    // Whether or not this bubble is a flavor bomb. Kinda hacky, but it's cheaper than searching every time.
    public bool isBomb = false;
    // Whether or not this bubble is a HYPERBUBBLE. Assigned on spawn.
    [HideInInspector]
    public bool isHyperbubble = false;
    // Assigned on spawn.
    [HideInInspector]
    public ChainBreakHandler handler;
    [HideInInspector]
    public bool isDestroying = false;

    // ==============================================================
    // Finalization methods
    // ==============================================================

    public void DestroyBubble(bool isBombChain)
    {
        isDestroying = true;

        // We only want to do our fancy animations if this bubble is not a bomb.
        // If it is, the bomb animations + the fancy break animations is too much
        // visual noise.
        FlavorBomb bomb = GetComponentInChildren<FlavorBomb>();
        if (bomb)
        {
            bomb.StoreSpawnPosition(transform.position);
            RemoveAllAdjacencies();
            Destroy(gameObject);
        }
        // Likewise, don't animate if ANY bubble in this chain is a bomb.
        else if (isBombChain)
        {
            RemoveAllAdjacencies();
            Destroy(gameObject);
        }
        else
        {
            Destroy(GetComponent<CircleCollider2D>());
            Destroy(GetComponent<DangerTracker>());
            Destroy(GetComponent<RadialGravity>());

            transform.localScale *= 0.85f;

            Rigidbody2D body = GetComponent<Rigidbody2D>();
            if (body)
            {
                // Cancel out most of our existing velocity, apply gravity, and give us a random horizontal force.
                body.velocity *= 0.1f;
                body.gravityScale = 2f;
                body.AddForce(new Vector2(Random.Range(-1,1)*100, 0f));
            }

            // Set transparency to 25% through the BubbleColorHelper.
            BubbleColorHelper helper = GetComponentInChildren<BubbleColorHelper>();
            if (helper) {
                helper.alpha = 0.25f;
            }

            // Bubbles are normally at sortingOrder 10: put destroyed bubbles in the back.
            SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();
            if (sprite) {
                sprite.sortingOrder = 5;
            }
            
            GetComponent<Animator>().SetTrigger("destroyed");
            RemoveAllAdjacencies();
            Destroy(gameObject, 2);
        }
    }

    // ==============================================================
    // Default methods
    // ==============================================================
    
    void OnDrawGizmos()
    {
        // Use the ID of this bullet's chain to determine the color of this bullet's
        // gizmo. Use modulo and prime numbers to hash the ID into RGB values.
        // ================

        float r,g,b; 
        r = (Mathf.Abs(chain.ID)/2)%100;     
        g = (Mathf.Abs(chain.ID)/3)%100;   
        b = (Mathf.Abs(chain.ID)/5)%100;
        
        Gizmos.color = new Color(r/100f,g/100f,b/100f);
        Gizmos.DrawSphere(transform.position, 0.25f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Detects collision starts between our collider and other triggers. Used to
        // update adjacencies and to consolidate bubbles into chains, if needed.
        // ================

        if (isDestroying) return;

        Bubble otherBubble = other.transform.parent.GetComponent<Bubble>();
        // If the otherBubble is not null...
        if (otherBubble) {
            if (otherBubble.isDestroying) return;
            // Add them to our adjacency list.
            AddAdjacency(otherBubble);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Detects collision endings between our collider and other triggers. Used to
        // update adjacencies and to distribute chains, if needed.
        // ================

        if (isDestroying) return;

        Bubble otherBubble = other.transform.parent.GetComponent<Bubble>();
        // If the otherBubble is not null...
        if (otherBubble) {
            if (otherBubble.isDestroying) return;
            // Remove them from our adjacency list.
            RemoveAdjacency(otherBubble);
        }
    }

    // ==============================================================
    // Data-manipulation methods
    // ==============================================================

    private void AddAdjacency(Bubble other)
    {
        // Adds Bubble other to this's adjacency list. If the bubbles are of the same
        // flavor, also consolidates their chains.
        // ================

        if (isDestroying) return;

        adjacencies.Add(other);
        // If they are of the same flavor as us, AND we're the younger bubble...
        if (other.bubbleFlavor == bubbleFlavor && age > other.age) {
            // Run the Consolidation algorithm on us, looking at them.
            Consolidate(other);
        }
        // If we're of different flavors or we're the older bubble, do nothing.
    }

    public void RemoveAdjacency(Bubble other)
    {
        // Removes Bubble other to this's adjacency list. If the bubbles are of the same
        // flavor and chain, also distributes their chain.
        // ================

        if (isDestroying) return;

        adjacencies.Remove(other);
        other.adjacencies.Remove(this);
        // If they are of the same flavor as us, AND we're the younger bubble...
        if (other.bubbleFlavor == bubbleFlavor && age > other.age) {
            // AND we're in the same chain...
            if (chain == other.chain) {
                // Run the Distribution algorithm on our chain.
                chain.Distribute(handler);
            }
        }
    }

    public void RemoveAllAdjacencies()
    {
        // Removes all bubbles in this's adjacency list. If ANY bubble is of the same
        // flavor, also distributes their chain.
        // ================

        // Make a copy of our adjacency list.
        List<Bubble> adjCopy = new List<Bubble>(adjacencies);
        // Keep track of if we'll have to distribute.
        bool distribute = false;

        foreach (Bubble bubble in adjCopy) {
            adjacencies.Remove(bubble);
            bubble.adjacencies.Remove(this);

            // If they are of the same flavor as us, AND we're the younger bubble...
            if (bubble.bubbleFlavor == bubbleFlavor && age > bubble.age) {
                // AND we're in the same chain...
                if (chain == bubble.chain) {
                    // Note that we'll need to distribute.
                    distribute = true;
                }
            }
        }

        // If by the end we need to distribute,
        if (!isDestroying && distribute) {
            // run the Distribution algorithm on our chain.
            chain.Distribute(handler);
        }
    }

    private void Consolidate(Bubble other)
    {
        // Consolidates two bubbles with like flavors in a collision, this and other,
        // into a single chain. Due to how this function is called in AddAdjacency,
        // this will always be younger than other.
        // Prerequisites: this.chain != null, other.chain != null
        // ====================

        if (isDestroying) return;

        Debug.Assert(this.chain != null, "Bubble Error: Consolidate() failed: this's chain must not be null.", this);
        Debug.Assert(other.chain != null, "Bubble Error: Consolidate() failed: other's chain must not be null.", other);

        // We must check if either bubble has a nil chain. If so, we must make it a chain
        // first. The nil chain, and ONLY the nil chain, has a length of 0. We can thus
        // check for length == 0 as a subsitute.
        if (chain.length == 0) {
            // Cache the maxLength uintVar, supplant the chain, and apply it back.
            uintVar maxLengthCache = chain.maxLength;
            chain = ScriptableObject.CreateInstance<Chain>();
            chain.maxLength = maxLengthCache;

            chain.AddBubble(this, handler);
        }
        if (other.chain.length == 0) {
            // Cache the maxLength uintVar, supplant the chain, and apply it back.
            uintVar maxLengthCache = other.chain.maxLength;
            other.chain = ScriptableObject.CreateInstance<Chain>();
            other.chain.maxLength = maxLengthCache;

            other.chain.AddBubble(other, handler);
        }

        // After this, check to make sure we are not consolidating a chain to itself.
        if (chain == other.chain) {
            return;
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
        longer.Concatenate(shorter, handler);

        // Finally, delete the old chain if it wasn't nil.
        if (shorter.length != 0) {
            Destroy(shorter);
        }
    }
}
