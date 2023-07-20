using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorPointer : MonoBehaviour
{

    // radius.x: inner radius --- radius.y: outer radius
    public Vector2 radius = new Vector2(1f, 4.4f);
    public boolVar gravityFlipped;
    SpriteRenderer sprite;
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

        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get the mouse position on the screen.
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // The spawn point is the vector from the center to the mouse position, normalized and then multiplied by the radius.
        transform.position = (Vector3)((mousePosition - center).normalized);

        // If gravity isn't flipped, multiply by outer radius.
        if (!gravityFlipped.value) {
            transform.position *= radius.y;
            sprite.flipY = true;
        }
        // Otherwise, multiply by inner radius.
        else {
            transform.position *= radius.x;
            sprite.flipY = false;
        }

        transform.rotation = Quaternion.LookRotation(Vector3.forward, transform.position);
    }
}