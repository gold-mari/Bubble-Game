using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuArcZoom : MonoBehaviour
{
    [System.Serializable]
    private class RectTransformValues
    {
        public Vector3 position;
        public Vector3 scale;
        public float zRotation;
    }

    [Range(0,1), SerializeField]
    private float zoomAmount = 0;
    [SerializeField]
    private RectTransformValues regularState;
    [SerializeField]
    private RectTransformValues zoomState;

    private RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = (RectTransform)transform;

        rectTransform.anchoredPosition = regularState.position;
        rectTransform.localScale = regularState.scale;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = Vector3.Lerp(regularState.position, zoomState.position, zoomAmount);
        Vector3 scale = Vector3.Lerp(regularState.scale, zoomState.scale, zoomAmount);
        float zRotation = Mathf.Lerp(regularState.zRotation, zoomState.zRotation, zoomAmount);

        rectTransform.anchoredPosition = pos;
        rectTransform.localScale = scale;
        rectTransform.localRotation = Quaternion.Euler(0, 0, zRotation);
    }
}