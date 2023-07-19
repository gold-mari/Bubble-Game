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
    public float spawnDelay = 1f;

    public float radius = 4.5f;
    public Vector2 center = new Vector2(0,0);

    uint age = 1;

    // Start is called before the first frame update
    void Start()
    {
        upcomingColor.value = Bubble_Color_Methods.random();
        currentColor.value = Bubble_Color_Methods.random();

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        var wait = new WaitForSeconds(spawnDelay);

        while (true) {
            SpawnBubble(currentColor.value);
            currentColor.value = upcomingColor.value;
            upcomingColor.value = Bubble_Color_Methods.random();

            yield return wait;
        }
    }

    private void SpawnBubble(Bubble_Color color)
    {
        // Get the mouse position on the screen.
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // The spawn point is the vector from the center to the mouse position, normalized and then multiplied by the radius.
        Vector2 spawnPoint = (mousePosition - center).normalized * radius;
        GameObject obj = Instantiate(bubble, spawnPoint, Quaternion.identity, transform);
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
