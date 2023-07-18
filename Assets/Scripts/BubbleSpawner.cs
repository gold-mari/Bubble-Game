using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    public GameObject redbubble;
    public GameObject bluebubble;
    uint age = 1;

    // Update is called once per frame
    void Update()
    {
        if ( Input.GetButtonDown("Fire1") ) {
            SpawnBubble(Bubble.Color.Red);
        }

        if ( Input.GetButtonDown("Fire2") ) {
            SpawnBubble(Bubble.Color.Blue);
        }
    }

    private void SpawnBubble(Bubble.Color color) {
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject objectToSpawn = null;

        switch (color)
        {
            case Bubble.Color.Red:
                objectToSpawn = redbubble;
                break;
            case Bubble.Color.Blue:
                objectToSpawn = bluebubble;
                break;
        }

        GameObject obj = Instantiate(objectToSpawn, worldPosition, Quaternion.identity, transform);
        obj.GetComponent<Bubble>().age = age;
        age++;
    }
}
