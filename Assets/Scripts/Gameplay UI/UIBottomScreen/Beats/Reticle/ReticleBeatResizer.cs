using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleBeatResizer : BeatWarning
{
    // ================================================================
    // Parameters
    // ================================================================

    [SerializeField, Tooltip("The size when at rest.")]
    private float restSize;
    [SerializeField, Tooltip("The size when the warning is active.")]
    private float warningSize;

    // ================================================================
    // Parameters
    // ================================================================

    private RectTransform rectTransform;

    // ================================================================
    // Initialization / finalization methods
    // ================================================================

    protected override IEnumerator Start()
    {
        // Start is called before the first frame update. We use it to get references and
        // initialize our UI.
        // ================

        rectTransform = (RectTransform)transform;
        
        yield return StartCoroutine(base.Start());
    }

    // ================================================================
    // Update methods
    // ================================================================

    protected override void OnMinorFlash()
    {
        // From base class. Called if the next beat is a single spawn.
        // ================

        rectTransform.sizeDelta = new(warningSize, warningSize);
    }

    protected override void OnMajorFlash()
    {
        // From base class. Called if the second next beat OR the next beat is a higher-order beat 
        // (not single, not null).
        // ================

        rectTransform.sizeDelta = new(warningSize, warningSize);
    }

    protected override void OnNoFlash()
    {
        // From base class. Called if the beat is neither a minor or major flash.
        // ================

        rectTransform.sizeDelta = new(restSize, restSize);
    }
}
