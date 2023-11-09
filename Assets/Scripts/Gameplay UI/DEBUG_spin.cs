using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG_spin : MonoBehaviour
{
    public float speed = 2.5f;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0,0,(Time.time*speed));
    }
}
