using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class BubbleSpawner : MonoBehaviour
{
    public Chain nilChain;
    public GameObject bubble;
    [Expandable]
    public bubble_ColorVar currentColor;
    [Expandable]
    public bubble_ColorVar upcomingColor;
    uint age = 1;

    // Start is called before the first frame update
    void Start()
    {
        upcomingColor.value = Bubble_Color_Methods.random();
        currentColor.value = Bubble_Color_Methods.random();
    }

    // Update is called once per frame
    void Update()
    {
        if ( Input.GetButtonDown("Fire1") ) {
            SpawnBubble(currentColor.value);
            currentColor.value = upcomingColor.value;
            upcomingColor.value = Bubble_Color_Methods.random();
        }
    }

    private void SpawnBubble(Bubble_Color color)
    {
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject obj = Instantiate(bubble, worldPosition, Quaternion.identity, transform);
        Bubble objBubble = obj.GetComponent<Bubble>();

        // Initialize color, age, and chain.
        objBubble.bubbleColor = color;
        objBubble.age = age;
        age++;
        objBubble.chain = nilChain;

        // Initialize sprite color.
        SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
        switch (color)
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
