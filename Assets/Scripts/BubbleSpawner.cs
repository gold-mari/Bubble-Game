using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    public GameObject bubble;
    uint age = 1;

    // Update is called once per frame
    void Update()
    {
        if ( Input.GetButtonDown("Fire1") ) {
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject obj = Instantiate(bubble, worldPosition, Quaternion.identity, transform);
            
            obj.GetComponent<Bubble>().age = age;
            age++;
        }
    }
}
