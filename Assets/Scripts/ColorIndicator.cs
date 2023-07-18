using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorIndicator : MonoBehaviour
{
    public bubble_ColorVar color;
    SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (color.value)
        {
            case Bubble_Color.Red:
                sprite.color = new Color(1f,0.2962264f,0.2962264f);
                break;
            case Bubble_Color.Blue:
                sprite.color = new Color(0.2980392f,0.6712846f,1f);
                break;
            case Bubble_Color.Yellow:
                sprite.color = new Color(0.9963511f,1f,0.2980392f);
                break;
            case Bubble_Color.Green:
                sprite.color = new Color(0.2980392f,1f,0.5218196f);
                break;
        }
    }
}
