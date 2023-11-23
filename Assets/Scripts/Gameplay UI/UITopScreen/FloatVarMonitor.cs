using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatVarMonitor : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [SerializeField, Tooltip("The floatVar we're monitoring.")]
    private floatVar floatVar;
    [SerializeField, Tooltip("The amount to scale our floatVar by.\n\nDefault: 100.")]
    private float scale = 100;
    [SerializeField, Tooltip("The number of decimal places to display of the floatVar\n\nDefault: 1.")]
    private uint decimalPlaces = 1;
    [SerializeField, Tooltip("A suffix added to the end of our printed string.")]
    private string suffix;

    // ================================================================
    // Internal variables
    // ================================================================

    private TMP_Text textObject;
    private string formatString = "0.";

    // ================================================================
    // Update methods
    // ================================================================

    private void Start()
    {
        // Start is called before the first frame update. We use it to define our text object.
        // ================

        textObject = GetComponent<TMP_Text>();
        formatString += new string('0', (int)decimalPlaces);
    }

    private void Update()
    {
        // Update is called once per frame. We use it to update our text.
        // ================

        textObject.text = (floatVar.value * scale).ToString(formatString) + suffix;
    }
}
