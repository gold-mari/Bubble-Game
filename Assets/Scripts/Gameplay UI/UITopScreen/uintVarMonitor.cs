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
    private uintVar UIntVar;
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

    private TMP_Text textObject;

    // ================================================================
    // Update methods
    // ================================================================

    private void Start()
    {
        // Start is called before the first frame update. We use it to define our text object.
        // ================

        textObject = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        // Update is called once per frame. We use it to update our text.
        // ================

        textObject.text = prefix + String.Format(formatString, UIntVar.value * scale) + suffix;
    }
}
