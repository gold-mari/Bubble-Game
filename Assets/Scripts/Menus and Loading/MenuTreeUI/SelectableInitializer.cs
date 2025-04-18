using UnityEngine;
using UnityEngine.UI;

public class SelectableInitializer : MonoBehaviour
{
    [Tooltip("The selectable 'down' from the back button.")]
    public Selectable EntryPoint;
    [Tooltip("The selectable(s) from which we can go 'up' to return to the back button.")]
    public Selectable[] ExitPoints;
}
