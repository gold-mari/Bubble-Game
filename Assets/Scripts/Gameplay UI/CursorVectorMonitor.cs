using UnityEngine;

public class CursorVectorMonitor : MonoBehaviour
{
    [Tooltip("The vector2Var equalling the vector from the cursor position to the center.")]
    [SerializeField]
    private vector2Var cursorPointVector;
    [Tooltip("The center of the playable space.\n\nDefault: (0,0)")]
    public Vector2 center = new Vector2(0,0);

    private void Update()
    {
        // Update is called once per frame. Used to update cursor position and rotation.
        // ================

        // Get the mouse position on the screen.
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Define the cursorPointVector
        cursorPointVector.value = (mousePosition - center);
    }
}