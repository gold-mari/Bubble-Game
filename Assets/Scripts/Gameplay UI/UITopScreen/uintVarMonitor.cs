using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class uintVarMonitor : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [SerializeField, Tooltip("The floatVar we're monitoring.")]
    protected uintVar UIntVar;
    [SerializeField, Tooltip("The amount to scale our uintVar by.\n\nDefault: 1.")]
    private uint scale = 1;
    [SerializeField, Tooltip("Used to format our scaled UIntVar for printing.")]
    private string formatString;
    [SerializeField, Tooltip("A prefix added to the start of our printed string.")]
    private string prefix;
    [SerializeField, Tooltip("A suffix added to the end of our printed string.")]
    private string suffix;

    // ================================================================
    // Internal variables
    // ================================================================

    protected TMP_Text textObject;

    // ================================================================
    // Default methods
    // ================================================================

    protected void Awake()
    {
        // Start is called before the first frame update. We use it to define our text object.
        // ================

        textObject = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        // Update is called once per frame. We use it to update our text.
        // ================

        textObject.text = prefix + string.Format(formatString, GetUInt() * scale) + suffix;
    }

    // ================================================================
    // Text-getting methods
    // ================================================================

    protected virtual uint GetUInt()
    {
        // Returns the value of our UIntVar, used in Update().
        // Overridden in child classes.
        // ================

        return UIntVar.value;
    }
}
