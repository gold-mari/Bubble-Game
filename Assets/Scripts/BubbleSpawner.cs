using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class BubbleSpawner : MonoBehaviour
{
    public Chain nilChain;
    public GameObject bubble;
    [Expandable]
    public List<bubble_ColorVar> colors;
    public float spawnDelay = 1f;

    public float radius = 4.5f;
    public Vector2 center = new Vector2(0,0);

    uint age = 1;

    void Start()
    {
        // Start is called before the first frame update
        // ================

        RandomizeColors();
        StartCoroutine(RegularSpawnRoutine());
        //StartCoroutine(ImpatientSpawnRoutine());
    }

    IEnumerator RegularSpawnRoutine()
    {
        // Spawns a new bubble at the cursor every spawnDelay seconds. Updates colors
        // after spawning a bubble.
        // ================

        var wait = new WaitForSeconds(spawnDelay);

        while (true) {
            yield return wait;
            SpawnBubble(colors[0].value);
            UpdateColors();
        }
    }

    IEnumerator ImpatientSpawnRoutine()
    {
        // Spawns a new bubble at the cursor on cursor click. Updates colors after
        // spawning a bubble.

        while (true) {
            if ( Input.GetButtonDown("Fire1") ) {
                SpawnBubble(colors[0].value);
                UpdateColors();
            }
            yield return null;
        }
    }

    private void RandomizeColors()
    {
        // Updates the Bubble_Colors in the array colors. Each color is randomized.
        // ================
        
        for (int i = 0; i < colors.Count; i++)
        {
            // Generate colors.
            colors[i].value = Bubble_Color_Methods.random();
        }
    }

    private void UpdateColors()
    {
        // Updates the Bubble_Colors in the array colors. For all i > 0, the Bubble_Color
        // at colors[i] is passed to colors[i-1]. The color at colors[Count-1] is then
        // randomly regenerated.
        // ================
        
        for (int i = 1; i < colors.Count; i++)
        {
            // Pass the colors backwards.
            colors[i-1].value = colors[i].value;
        }

        // Regenerate the final color.
        colors[colors.Count-1].value = Bubble_Color_Methods.random();  
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
        sprite.color = Bubble_Color_Methods.getSpriteColor(color);
    }
}
