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
    private Vector2 center;

    // Start is called before the first frame update
    void Start()
    {
        center = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 scaledVector = (pointVector.value * radiusScale);
        if ( scaledVector.magnitude > maxRadius ) {
            scaledVector = scaledVector.normalized * maxRadius;
        }

        transform.position = (Vector3)(center + scaledVector);
    }
}
