using UnityEngine;
using System.Collections;

public class randomFlipper : MonoBehaviour
{
    public Vector2 period = new(0.5f, 2f);
    Vector3 baseScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        baseScale = transform.localScale;

        while (true) {
            yield return new WaitForSeconds(Random.Range(period.x,period.y));

            Vector3 newScale;
            do {
                newScale = new(
                    baseScale.x * (Random.Range(0,2)*2 - 1),
                    baseScale.y * (Random.Range(0,2)*2 - 1),
                    1
                );
            } while (newScale == transform.localScale);

            transform.localScale = newScale;
        }
        
    }
}
