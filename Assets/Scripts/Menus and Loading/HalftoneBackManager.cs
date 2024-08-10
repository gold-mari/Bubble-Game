using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalftoneBackManager : MonoBehaviour
{
    public Color baseColor = Color.black;
    public Color orbColor = Color.white;

    private Material material;
    private Color lastBase, lastOrb;

    void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
        material.SetColor("_BaseColor", baseColor);
        material.SetColor("_OrbColor", orbColor);
        lastBase = baseColor;
        lastOrb = orbColor;
    }

    private void Update()
    {
        if (lastBase != baseColor) {
            material.SetColor("_BaseColor", baseColor);
            lastBase = baseColor;
        }
        if (lastOrb != orbColor) {
            material.SetColor("_OrbColor", orbColor);
            lastOrb = orbColor;
        }
    }
}
