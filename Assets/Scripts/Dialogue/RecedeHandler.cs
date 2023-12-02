using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class RecedeHandler : MonoBehaviour
{
    [Range(0.0f,1.0f), Tooltip("How far into the background we are. 0 is fully front, 1 is fully back.")]
    public float backgroundness = 0f;

    [Header("Front")]
    [SerializeField, Tooltip("The position of the object when it is fully front.")]
    private Vector3 frontPosition;
    [SerializeField, Tooltip("The color of the object when it is fully front.")]
    private Color frontColor;

    [Header("Back")]
    [SerializeField, Tooltip("The position of the object when it is fully back.")]
    private Vector3 backPosition;
    [SerializeField, Tooltip("The color of the object when it is fully back.")]
    private Color backColor;

    

    private SpriteRenderer sprite;

    void Start()
    {
        // Start is called before the first frame update. We use it to define our sprite renderer.
        // ================

        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Update is called once per frame. We use it to apply the changes to our position and color.
        // ================

        transform.localPosition = Vector3.Lerp(frontPosition, backPosition, backgroundness);
        sprite.color = Color.Lerp(frontColor, backColor, backgroundness);
    }
}
