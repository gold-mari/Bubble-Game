using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class SetBoolVar : MonoBehaviour
{
    [Expandable, SerializeField, Tooltip("The boolVar we set.")]
    boolVar boolVar;
    [SerializeField, Tooltip("Whether or not we set the boolVar's value on startup.\n\nDefault: false")]
    bool setOnStart = false;
    [ShowIf("setOnStart"), SerializeField, Tooltip("The value we set the boolVar's value to on startup.")]
    bool boolValue;

    private void Start()
    {
        if (setOnStart) boolVar.value = boolValue;
    }

    public void Set(bool value)
    {
        boolVar.value = value;
    }
}
