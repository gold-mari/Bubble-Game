using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionOnStart : MonoBehaviour
{
    [SerializeField]
    UnityEvent ourEvent;

    // Start is called before the first frame update
    void Start()
    {
        ourEvent?.Invoke();
    }
}
