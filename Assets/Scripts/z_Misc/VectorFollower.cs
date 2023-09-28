using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorFollower : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [SerializeField]
    private vector2Var pointVector;
    [SerializeField]
    private float radiusScale;
    [SerializeField]
    private float maxRadius;

    // ================================================================
    // Internal variables
    // ================================================================

    // The point in 2D space where this object is centered. The pointVector is applied to this
    // center. It is defined as the point this gameObject is at when this script starts.
    private Vector2 center;

    // ================================================================
    // Default methods
    // ================================================================

    void Start()
    {
        // Start is called before the first frame update. We use it to define center as
        // the origin point of this object.
        // ================

        center = transform.position;
    }

    void Update()
    {
        // Update is called once per frame. We use it to scale pointVector by
        // radiusScale, and clamp it's magnitude to maxRadius. We then apply this to
        // center.
        // ================
        
        Vector2 scaledVector = (pointVector.value * radiusScale);
        if (scaledVector.magnitude > maxRadius) {
            scaledVector = scaledVector.normalized * maxRadius;
        }

        transform.position = (Vector3)(center + scaledVector);
    }
}
