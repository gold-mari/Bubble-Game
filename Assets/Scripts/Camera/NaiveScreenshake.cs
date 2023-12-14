using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Lumin;

public class NaiveScreenshake : MonoBehaviour
{
    [SerializeField]
    private float baseShakeMagnitude = 0.2f;
    [SerializeField]
    private float baseShakeDuration = 1f;

    private Vector3 basePosition;

    void Awake()
    {
        // Awake is called before Start.
        // ================

        basePosition = transform.position;
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 80, 80), "Shake!"))
        {
            StartCoroutine(Shake(baseShakeMagnitude, baseShakeDuration));
        }
    }

    IEnumerator Shake(float magnitude, float duration)
    {
        // Applies the shake transformation to our camera.
        // ================

        float elapsed = 0;
        while (elapsed < duration)
        {
            float currentMagnitude = Mathf.Lerp(magnitude, 0, elapsed/duration);
            // Give us a random unit vector multiplied by our currentMagnitude.
            Vector3 offset = new Vector3(Random.Range(-1f,1f), Random.Range(-1f,1f), 0).normalized * currentMagnitude;
            transform.position = basePosition + offset;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = basePosition;
    }
}
