using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    public Chain nilChain;
    public GameObject bubble;
    uint age = 1;

    // Update is called once per frame
    void Update()
    {
        if ( Input.GetKeyDown(KeyCode.A) ) {
            SpawnBubble(Bubble_Color.Red);
        }

        if ( Input.GetKeyDown(KeyCode.S) ) {
            SpawnBubble(Bubble_Color.Blue);
        }

        if ( Input.GetKeyDown(KeyCode.D) ) {
            SpawnBubble(Bubble_Color.Yellow);
        }

        if ( Input.GetKeyDown(KeyCode.F) ) {
            SpawnBubble(Bubble_Color.Green);
        }
    }

    private void SpawnBubble(Bubble_Color color) {
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
