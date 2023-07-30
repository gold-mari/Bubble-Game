using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class RadialGravity : MonoBehaviour
{

    // ================================================================
    // Parameters
    // ================================================================

    [Tooltip("The floatVar representing the current, vector strength of gravity. Negative is "
           + "inwards, positive is outwards.")]
    [Expandable]
    public floatVar strength;
    [Tooltip("The center of gravity.\n\nDefault: (0,0)")]
    public Vector2 center = new Vector2(0,0);

    // ==============================================================
    // Internal variables
    // ==============================================================

    // The Rigidbody2D on this object.
    Rigidbody2D body;

    // ================================================================
    // Default methods
    // ================================================================
    
    void Start()
    {
        // Start is called before the first frame update. Used to define body.
        // ================

        body = GetComponent<Rigidbody2D>();
        Debug.Assert( body != null, "RadialGravity Error: Start() failed: gameObject must have a Rigidbody2D.", this );
    }

    void FixedUpdate()
    {
        // Update is called once per frame. Used to apply the force of gravity onto body.
        // ================

        Vector2 vector = ((Vector2)transform.position - center).normalized;
        vector *= strength.value;
        body.AddForce(vector);
    }
}
