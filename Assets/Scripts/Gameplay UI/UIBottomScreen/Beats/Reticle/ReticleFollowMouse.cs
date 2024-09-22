using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleFollowMouse : MonoBehaviour
{
    void Update()
    {
        // Update is called once per frame.
        // ================

        // If time scale is 0
        if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new(pos.x, pos.y, 0);
    }
}
