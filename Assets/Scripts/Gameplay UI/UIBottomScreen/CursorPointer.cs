using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class CursorPointer : MonoBehaviour
{

    // ================================================================
    // Parameters
    // ================================================================

    [Tooltip("The BeatReader in this scene.")]
    public BeatReader reader;
    [Tooltip("Whether this script should inherit its center and radius from its parent's "
           + "BubbleSpawner script. Turn this off if you need to fine-tune things.\n\nDefault: true")]
    public bool takeCenterAndRadius = true;
    [Tooltip("The radii (inner, outer) at which the cursor should orbit. When gravity isn't "
           + "inverted, the outer radius is used. When gravity IS inverted, the inner radius is "
           + "used.\n\nDefault: (1,4.4)")]
    [HideIf("takeCenterAndRadius")]
    public Vector2 radius = new Vector2(1f, 4.4f);
    [HideIf("takeCenterAndRadius")]
    public Vector2 center = new Vector2(0f, 0f);
    [Tooltip("The boolVar which signals if gravity is flipped to point outwards instead of inwards.")]
    [SerializeField]
    private boolVar gravityFlipped;
    [Tooltip("The vector2Var equalling the vector from the cursor position to the center.")]
    [SerializeField]
    private vector2Var cursorPointVector;

    // ================================================================
    // Internal variables
    // ================================================================

    // A float from 0 to 1, using the radius Vector2 to indicate what radius we should orbit at.
    // A radiusLerper of 1 means we're fully at the outer radius. A radiusLerper of 0 means we're
    // fully at the inner radius. In between is in between.
    [HideInInspector]
    public float radiusLerper = 1;
    // Store a variable which represents the true radius we orbit at.
    private float trueRadius;
    // Updated whenever gravityFlipped changes to store the previous value of brightness. We
    // compare this to the current value of gravityFlipped to detect if it has changed.
    private bool lastGravityFlipped;
    // The animator on this object.
    Animator animator;
    // This objects SpriteRenderer. We need a reference to flip the sprite when gravity changes.
    SpriteRenderer sprite;
    // The number of hyperbubbles that have been queued up, used to grow/shrink our cursor.
    private uint queuedHYPERBUBBLES = 0;

    // ================================================================
    // Default methods
    // ================================================================

    private void Start()
    {
        // Start is called before the first frame update. We use it to define sprite and
        // animator, to initialize lastGravityFlipped, and to inherit radius and center
        // if necessary.
        // ================
        
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        reader.beforeHyperSpawn += OnBeforeHyperSpawn;
        reader.hyperSpawn += OnHyperSpawn;

        lastGravityFlipped = gravityFlipped.value;

        // If we want to take our center and radius, do so.
        if (takeCenterAndRadius) {
            BubbleSpawner spawner = transform.parent.GetComponent<BubbleSpawner>();
            Debug.Assert(spawner != null, "CursorPointer Error: Start() failed: takeCenterAndRadius is true, "
                                        + "yet this script's parent object has no BubbleSpawner.", this);

            radius = spawner.radius;
            center = spawner.center;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribes from events.
        // ================

        reader.beforeHyperSpawn -= OnBeforeHyperSpawn;
        reader.hyperSpawn -= OnHyperSpawn;
    }

    private void Update()
    {
        // Update is called once per frame. Used to update cursor position and rotation.
        // ================

        // The spawn point is the vector from the center to the mouse position, normalized and then multiplied by the radius.
        transform.position = (Vector3)cursorPointVector.value.normalized;

        // Calculate our lerped radius.
        trueRadius = Mathf.Lerp(radius.y, radius.x, radiusLerper);

        // Apply this radius to our position.
        transform.position *= trueRadius;

        // Make sure we're centered right.
        transform.position += (Vector3)center;

        if (lastGravityFlipped != gravityFlipped.value) {
            lastGravityFlipped = gravityFlipped.value;
            animator.SetBool("gravityFlipped", gravityFlipped.value);
        }

        // Point towards the center.
        transform.rotation = Quaternion.LookRotation(Vector3.forward, transform.position-(Vector3)center);
        // Multiply this by a custom rotation which flips the cursor during animations.
        // When radiusLerper is 0, we want to flip 180 degrees. When 1, flip 0 degrees.
        float xAmount = (radiusLerper * 180f);
        transform.rotation *= Quaternion.Euler(xAmount, 0, 0);
    }

    // ================================================================
    // Event-handling methods
    // ================================================================

    private void OnBeforeHyperSpawn()
    {
        // Tells the animator to let it grow, let it grow, let the love inside you show!
        // ================

        queuedHYPERBUBBLES++;
        if (queuedHYPERBUBBLES > 0)
        {
            animator.SetBool("grow", true);
        }
    }

    private void OnHyperSpawn()
    {
        // Tells the animator to go back to normal size.
        // ================

        queuedHYPERBUBBLES--;
        if (queuedHYPERBUBBLES == 0)
        {
            animator.SetBool("grow", false);
        }
    }
}