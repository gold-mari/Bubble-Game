using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class RadialGravity : MonoBehaviour
{
    Rigidbody2D body;
    [Expandable]
    public floatVar strength;
    public Vector2 center = new Vector2(0,0);

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 vector = (Vector2)transform.position - center;
        vector = vector.normalized;
        vector *= strength.value;
        body.AddForce(vector);
    }
}
