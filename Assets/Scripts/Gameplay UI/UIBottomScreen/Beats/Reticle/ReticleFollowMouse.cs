using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleFollowMouse : MonoBehaviour
{
    void Update()
    {
        // Update is called once per frame.
        // ================
        
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new(pos.x, pos.y, 0);
    }
}
