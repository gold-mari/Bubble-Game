using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBeatIndicator : MonoBehaviour
{
    [SerializeField]
    private Color baseColor;
    [SerializeField]
    private float opacity;
    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        opacity = 0;
    }

    // Update is called once per frame
    void Update()
    {
        sprite.color = new Color(baseColor.r, baseColor.g, baseColor.b, opacity);
    }
}
