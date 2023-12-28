using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LosePinholeShrink : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [SerializeField, Tooltip("The amount of time, in seconds, it takes to go from maxSize to minSize.\n\nDefault: 1")]
    private float duration = 1;
    [SerializeField, Tooltip("The maximum size of this object.")]
    private Vector3 maxSize;
    [SerializeField, Tooltip("The position of this object at it's maximum size.")]
    private Vector3 maxPosition;
    [SerializeField, Tooltip("The minimum size of this object.")]
    private Vector3 minSize;
    [SerializeField, Tooltip("The position of this object at it's minimum size.")]
    private Vector3 minPosition;

    // ================================================================
    // Internal variables
    // ================================================================

    // Callback action for when we're done shrinking.
    public System.Action DoneShrinking;
    private Image image;
    private bool doShrinking;
    private float elapsed = 0;

    // ================================================================
    // Intializer methods
    // ================================================================

    private void Start()
    {
        // Start is called before the first frame update. Initializes!
        // ================

        image = GetComponent<Image>();
        if (image)
        {
            image.enabled = false;
        }
    }

    // ================================================================
    // Animation methods
    // ================================================================

    private void Update()
    {
        // Update is called every frame. Animates our shrinking.
        // ================

        if (doShrinking)
        {
            if (elapsed < duration)
            {
                transform.localPosition = Vector3.Lerp(maxPosition, minPosition, LerpKit.EaseOut(elapsed/duration, 3));
                transform.localScale = Vector3.Lerp(maxSize, minSize, LerpKit.EaseOut(elapsed/duration, 3));
                elapsed += Time.deltaTime;
            }
            else
            {
                DoneShrinking?.Invoke();
                doShrinking = false;
            }
        }
    }
    
    public void BeginShrinking()
    {
        // Called from elsewhere, primarily the endgame manager. Starts animating our shrinking.
        // ================

        if (image)
        {
            image.enabled = true;
        }
        
        transform.localPosition = maxPosition;
        transform.localScale = maxSize;

        doShrinking = true;
    }
}
