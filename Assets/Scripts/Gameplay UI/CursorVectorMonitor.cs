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

        // If time scale is 0
        if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

        if (InputHandler.Instance.LastUsedScheme == InputHandler.Instance.KeyboardScheme) {
            // Get the mouse position on the screen.
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(InputHandler.GetPointer());
            // Define the cursorPointVector
            cursorPointVector.value = mousePosition - center;
        } else { // Gamepad
            Vector2 stick = InputHandler.GetStick();
            if (stick.magnitude != 0) {
                cursorPointVector.value = stick.normalized;
            }
        }
        
    }
}