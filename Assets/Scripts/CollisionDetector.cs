using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        print("Started CollisionEnter with " + other.transform.name);
        longString();
        print("Finished CollisionEnter with " + other.transform.name);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        print("Started CollisionExit with " + other.transform.name);
        longString();
        print("Finished CollisionExit with " + other.transform.name);
    }

    private void longString()
    {
        for (int i=0; i<1000; i++){ print(i); }
    }
}
