using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoRing : MonoBehaviour
{
    [SerializeField, Tooltip("The amount of time, in seconds, it takes this ring to disappear.\n\nDefault: 2")]
    float fadeDuration = 2;
    [SerializeField, Tooltip("The final scale, as a multiple of the base scale, of this object's transform.\n\nDefault: 2")]
    float finalScale = 2;

    private Vector3 baseScale, finalScaleVector;
    private SpriteRenderer sprite;
    private Color baseColor, endColor;
    float elapsed = 0;


    // Start is called before the first frame update
    void Awake()
    {
        baseScale = transform.localScale;
        finalScaleVector = baseScale * finalScale;

        sprite = GetComponent<SpriteRenderer>();
        baseColor = sprite.color;
        endColor = new(baseColor.r, baseColor.g, baseColor.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (elapsed < fadeDuration)
        {
            transform.localScale = Vector3.Lerp(baseScale, finalScaleVector, LerpKit.EaseOut(elapsed/fadeDuration, 3));
            sprite.color = Color.Lerp(baseColor, endColor, LerpKit.EaseOut(elapsed/fadeDuration, 3));
            elapsed += Time.deltaTime;
        }   
        else
        {
            Destroy(gameObject);
        }
    }
}
