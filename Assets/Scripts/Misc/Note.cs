using UnityEngine;
using NaughtyAttributes;

public class Note : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================

    // Pseudo-comment for GameObjects.
    [Tooltip("Leave a note here!")]
    [SerializeField] [ResizableTextArea]
    private string note;
}
