using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionOnStart : MonoBehaviour
{
    [SerializeField, Tooltip("If true, our action is called in Start.\n\nDefault: true")]
    private bool onStart = true;
    [SerializeField, Tooltip("If true, our action is called in OnEnable.\n\nDefault: false")]
    private bool onEnable = false;
    [SerializeField]
    UnityEvent ourEvent;

    // Start is called before the first frame update
    private void Start()
    {
        if (onStart) ourEvent?.Invoke();
    }

    private void OnEnable()
    {
        if (onEnable) {
            ourEvent?.Invoke();
        }
    }
}
