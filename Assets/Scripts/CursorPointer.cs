using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorPointer : MonoBehaviour
{

    float radius = 4.5f;
    Vector2 center = new Vector2(0,0);

    // Start is called before the first frame update
    void Start()
    {
        // Check if the BubbleSpawner exists. If it does, take radius and center from there.
        BubbleSpawner spawner = transform.parent.GetComponent<BubbleSpawner>();
        if (spawner) {
            radius = spawner.radius;
            center = spawner.center;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Get the mouse position on the screen.
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // The spawn point is the vector from the center to the mouse position, normalized and then multiplied by the radius.
        transform.position = (Vector3)((mousePosition - center).normalized * radius);

        transform.rotation = Quaternion.LookRotation(Vector3.forward, transform.position);
    }
}