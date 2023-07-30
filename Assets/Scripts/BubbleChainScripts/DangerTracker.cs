using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DangerTracker : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [Tooltip("The boolVar which signals if gravity is flipped to point outwards instead of inwards.")]
    public boolVar gravityFlipped;
    [Tooltip("The DangerManager that we update when we enter and exit danger. Normally assigned in "
           + " BubbleSpawner, but exposed here for debug purposes.")]
    public DangerManager dangerManager;
    [Tooltip("The radii (inner, outer) past which a bullet is considered in danger. When gravity "
           + "isn't inverted, the outer radius is used. When gravity IS inverted, the inner radius "
           + "is used.")]
    [SerializeField]
    private Vector2 dangerRadii;
    [Tooltip("The center of the playspace.\n\nDefault: (0,0)")]
    [SerializeField] 
    Vector2 center = new Vector2(0,0);
    [Tooltip("How long to wait, in seconds, before we start checking for danger.\n\nDefault: 0.5")]
    [SerializeField]
    private float initialDelay = 0.5f;

    // ==============================================================
    // Internal variables
    // ==============================================================

    // The radius of this bubble, used in distance calculation. Because bubbles are presumed to be
    // circular, taking either the width or the height of the bounding box will approximate
    // diameter well enough.
    private float bubbleRadius;
    // Used to determine if this bullet is in the danger range. We compare this in Update to the
    // boolean valuation of if we're in the danger radius- if at any point they differ, update
    // DangerManager and then update inDanger.
    private bool inDanger = false;
    // Used to halt Update from running calculations until after initialDelay seconds.
    private bool checkForDanger = false;
    // A cached reference to WaitForSeconds(initialDelay).
    private WaitForSeconds wait;
    // The BubbleSpawner with System.Action that is shouted when gravity flips.
    // IMPORTANT: THIS WILL BE OVERHAULED AND CONNECTED TO A TIMEKEEPER MANAGER.
    private BubbleSpawner spawner;
    // The Animator on this bubble. Used to start the flashing animation when we're in danger.
    private Animator animator;
    

    // ================================================================
    // Default methods
    // ================================================================

    IEnumerator Start()
    {
        // Start is called before the first frame update. We use it to define flipGravity
        // and toggle checkForDanger after initialDelay seconds.
        // ================

        // First, check to make sure this object has a Collider2D.
        Collider2D collider = GetComponent<Collider2D>();
        Debug.Assert( collider != null, "DangerTracker Error: Start() failed: gameObject must have a Collider2D.", this );
        // Define the radius of our bubble using a bounding box. Again, because bubbles
        // are presumed to be circular, width is an acceptable substitute for diameter.
        // Divide by 2 to get radius instead of diameter.
        bubbleRadius = collider.bounds.size.x/2f;

        wait = new WaitForSeconds(initialDelay);
        yield return wait;

        // Define flipGravity and connect OnFlipGravity to it if possible.
        spawner = transform.parent.GetComponent<BubbleSpawner>();
        if (spawner) {
            spawner.flipGravityAction += OnFlipGravity;
        }

        // Define the animator.
        animator = GetComponent<Animator>();

        // Finally, note we can checkForDanger.
        checkForDanger = true;
    }

    void OnDestroy()
    {
        // OnDestroy is used to remove OnFlipGravity from flipGravity, if possible. Also,
        // if this bubble is inDanger, we decrement the number of bubbles in danger.
        // ================

        if (spawner) {
            spawner.flipGravityAction -= OnFlipGravity;
        }
        if (dangerManager && inDanger) {
            dangerManager.Decrement();
        }
    }

    void Update()
    {
        // Update is called once per frame. We use it to check if we're past our danger
        // range, and update inDanger accordingly. We use edge detection to increment and
        // decrement dangerManager.
        // ================

        // ================================
        // Part 0: Short circuit
        // ================================

        if ( !checkForDanger ) {
            return;
        }

        // ================================
        // Part 1: pastRange?
        // ================================
    
        // Calculate distance from center to use later. Casting a Vector3 to a Vector2
        // discards the z value. 
        float distance = ((Vector2)transform.position - center).magnitude;

        // Also, track if we're past the range. We compare this to inDanger, later.
        bool pastRange = false;

        // If gravity isn't flipped, check if we're further than the outer radius. If so,
        // then note that we're past range. ALSO, add bubbleRadius to find the distance
        // to the furthest point on the bubble from the center.
        if ( !gravityFlipped.value && (distance + bubbleRadius) > dangerRadii.y ) {    
            pastRange = true;
        }
        // Otherwise, check if we're further than the inner radius. If so, then note that
        // we're past range. ALSO, subtract bubbleRadius to find the distance to the
        // closest point on the bubble from the center.
        else if ( gravityFlipped.value && (distance - bubbleRadius) < dangerRadii.x ) {
            pastRange = true;
        }
        // Recall pastRange is initialized to false. If it isn't set to true by one of
        // the above conditions

        // ================================
        // Part 2: Edge detection
        // ================================

        // If pastRange != inDanger, then pastRange just changed. Note the change, and
        // then update inDanger so it once again matches pastRange.

        if ( dangerManager ) {
        // If we're pastRange but not inDanger, we just entered danger.
            if ( pastRange && !inDanger ) {
                dangerManager.Increment();
                inDanger = pastRange;
            }
            // If we're not pastRange but we're inDanger, we just exited danger.
            if ( !pastRange && inDanger ) {
                dangerManager.Decrement();
                inDanger = pastRange;
            }
        }

        // Finally, if the animator exists, update the animator.
        if ( animator ) {
            animator.SetBool("inDanger", inDanger);
        }
    }

    // ================================================================
    // Data-manipulation methods
    // ================================================================

    void OnFlipGravity()
    {
        // Called via an event listener when gravity flips. Runs OnFlipGravityRoutine.
        // ================

        StartCoroutine(OnFlipGravityRoutine());
    }
    IEnumerator OnFlipGravityRoutine()
    {
        // Called from OnFlipGravity. Marks that we're not inDanger, marks that we
        // shouldn't checkForDanger, and counts to initialDelay seconds before we mark
        // checkForDanger as true again.
        // ================

        inDanger = false;
        animator.SetBool("inDanger", false);
        checkForDanger = false;
        // Reset DangerManager.
        yield return wait;
        checkForDanger = true;
    }

}
