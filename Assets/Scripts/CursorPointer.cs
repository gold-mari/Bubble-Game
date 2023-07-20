using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class CursorPointer : MonoBehaviour
{

    [Tooltip("Whether this script should inherit its parent's BubbleSpawner script. Turn "
           + "this off if you need to fine-tune things.\n\nDefault: true")]
    public bool takeCenterAndRadius = true;
    [Tooltip("The center of the playable space.\n\nDefault: (0,0)")]
    [HideIf("takeCenterAndRadius")]
    public Vector2 center = new Vector2(0,0);
    [Tooltip("The radii (inner, outer) at which the cursor should orbit. When gravity isn't "
           + "inverted, the outer radius is used. When gravity IS inverted, the inner radius is "
           + "used.\n\nDefault: (1,4.4)")]
    [HideIf("takeCenterAndRadius")]
    public Vector2 radius = new Vector2(1f, 4.4f);
    [Tooltip("The boolVar which signals if gravity is flipped to point outwards instead of inwards.")]
    public boolVar gravityFlipped;
    // This objects SpriteRenderer. We need a reference to flip the sprite when gravity changes.
    SpriteRenderer sprite;

    void Start()
    {
        // Start is called before the first frame update. We use it to define sprite AND
        // to inherit radius and center if necessary.
        // ================
        
        // If we want to take our center and radius, do so.
        if (takeCenterAndRadius) {
            BubbleSpawner spawner = transform.parent.GetComponent<BubbleSpawner>();
            Debug.Assert( spawner != null, "CursorPointer Error: Start() failed: takeCenterAndRadius is true, "
                                         + "yet this script's parent object has no BubbleSpawner.", this );

            radius = spawner.radius;
            center = spawner.center;
        }

        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Update is called once per frame. Used to update cursor position and rotation.
        // ================

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