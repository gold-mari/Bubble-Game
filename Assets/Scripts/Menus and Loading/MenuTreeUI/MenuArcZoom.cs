using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MenuArcZoom : MonoBehaviour
{
    [System.Serializable]
    private class RectTransformValues
    {
        [Tooltip("The local scale of the state.")]
        public Vector3 scale;
        [Tooltip("The local rotation of the state along the z-axis.")]
        public float zRotation;
    }

    [Range(0,1), SerializeField, Tooltip("The UI image displaying our arc.")]
    private Image arcImage;
    [Range(0,1), SerializeField, Tooltip("The amount (0-1) that we're zoomed in by.")]
    private float zoomAmount = 0;
    public float ZoomAmount { // Public accessor for zoom amount.
        get { return zoomAmount; }
        set { zoomAmount = value; }
    }

    [SerializeField, Tooltip("The rect transform values used for the non-zoomed-in state.")]
    private RectTransformValues regularState;
    [SerializeField, Tooltip("The rect transform values used for the zoomed-in state.")]
    private RectTransformValues zoomState;

    // Cached ref to the rect transform.
    private RectTransform rectTransform;
    // Cached ref to the image rect transform.
    private RectTransform imageRectTransform;
    // Cache the last value, for edge detection.
    private float lastZoomAmount = 0;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = (RectTransform)transform;

        rectTransform.localScale = regularState.scale;
    }

    // Update is called once per frame
    void Update()
    {
        if (lastZoomAmount != zoomAmount) {

            Vector3 scale = Vector3.Lerp(regularState.scale, zoomState.scale, zoomAmount);
            float zRotation = Mathf.Lerp(regularState.zRotation, zoomState.zRotation, zoomAmount);

            rectTransform.localScale = scale;
            rectTransform.localRotation = Quaternion.Euler(0, 0, zRotation);

            lastZoomAmount = zoomAmount;
        }
    }
}