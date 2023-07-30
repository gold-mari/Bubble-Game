using UnityEngine;
using NaughtyAttributes;

public class Note : MonoBehaviour
{
    // Pseudo-comment for GameObjects.
    
    [SerializeField] [ResizableTextArea]
    private string note;
}
