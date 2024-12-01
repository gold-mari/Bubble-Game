using UnityEngine;
using UnityEngine.Events;

public class ActionOnBoolVar : MonoBehaviour
{
    [SerializeField, Tooltip("The boolVar we're monitoring.")]
    private boolVar target;
    [SerializeField, Tooltip("The target boolean value. We call our action once the boolVar changes to this value.")]
    private bool actionValue;
    [Tooltip("The action called when target is set to actionValue.")]
    public UnityEvent action;


    public void Query()
    {
        if (target.value == actionValue) {
            action?.Invoke();
        }
    }
}