using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitVectorFollower : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [SerializeField]
    private vector2Var unitVector;
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
        transform.position = (Vector3)(center + (unitVector.value * maxRadius));
    }
}
