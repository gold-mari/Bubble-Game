using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class SquashStretchHelper : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================

    [Tooltip("This object's base scale. By default we get this from our transform, but it can "
           + "be updated at runtime if you wish.")]
    [SerializeField]
    private Vector3 baseScale;
    [Tooltip("A float (-1 to 1) which is used to lerp the base scale towards squash, if squetch is "
           + "negative, or towards stretch, if squetch is positive.")]
    [SerializeField] [Range(-1, 1)]
    private float squetch = 0;
    [Tooltip("The magnitude of squash / stretch. When magnitude is 2, max stretch is 2x as tall "
           + "and 0.5x as wide as the rest state. When magnitude is 5, max stretch is 5x as tall "
           + "as the rest state. Etc.\n\nDefault: 2")]
    [SerializeField]
    private float magnitude = 2;
    // Updated whenever squetch changes to store the previous value of squetch. We compare
    // this to the current value of squetch to detect if it has changed.
    private float lastSquetch;

    // ==============================================================
    // Default methods
    // ==============================================================

    void Start()
    {
        // Start is called before the first frame update. Used to initialize all our
        // variables and define all our references.
        // ================

        squetch = lastSquetch = 0;
        
        // Get the color from our parent's Bubble component.
        baseScale = transform.localScale;
    }

    void Update()
    {
        // If squetch has changed, update squetch.
        if ( squetch != lastSquetch ) {
            UpdateSquetch();
            lastSquetch = squetch;
        }
    }

    void UpdateSquetch()
    {
        // Called when squetch changes. If squetch is 0, resets scale to baseScale. If it
        // is above 0, lerps towards full stretch. If it is below 0, lerps towards full
        // squash.
        // ================


        float width = baseScale.x * Mathf.Pow(magnitude, squetch);
        float height = baseScale.y * Mathf.Pow(magnitude, -squetch);
        transform.localScale = new Vector3(width, height, 0f);
    }
}