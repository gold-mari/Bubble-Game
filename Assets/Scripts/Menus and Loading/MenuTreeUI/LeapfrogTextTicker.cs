using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.PackageManager.Requests;

// Inspired by (with some code from) Board to Bits Games, on YouTube.
// https://www.youtube.com/watch?v=mptVj9-I0gQ

public class LeapfrogTextTicker : MonoBehaviour
{
    [SerializeField, Tooltip("The text applied to our text objects.")]
    private string textContent;
    [SerializeField, Tooltip("The text prefab that we spawn.")]
    private GameObject prefab;
    [SerializeField, Tooltip("The speed, in units per second, at which our text scrolls.\n\nDefault: 15")]
    private float scrollSpeed;
    [SerializeField, Tooltip("The width of text objects, relative to its preferredWidth.\n\nDefault: 1.1")]
    private float paddingWidth = 1.1f;
    [SerializeField, Tooltip("The RectTransform that bounds this text.")]
    private RectTransform boundingRect;
    private ObjectPool pool;
    private GameObject trailingObject = null;
    private Dictionary<GameObject, RectTransform> rectDict = new();
    private Dictionary<GameObject, TMP_Text> textDict = new();

    private void Start()
    {
        // Start is called before the first frame update. We use it to:
        //  * Define our bounding rect.
        //  * Initialize our prefab, and add it to our dictionaries.
        //  * Build our pool from our initialized prefab.
        //  * Fill in the bounding rect with objects from our pool.
        // ================

        // boundingRect = GetComponent<RectTransform>();

        // Init our prefab in the rect and text dictionaries.
        rectDict[prefab] = prefab.GetComponent<RectTransform>();
        textDict[prefab] = prefab.GetComponent<TMP_Text>();

        // Set prefab text,
        textDict[prefab].text = textContent;
        // And prefab size.
        Vector2 objSize = textDict[prefab].GetPreferredValues();
        objSize.x *= paddingWidth;
        rectDict[prefab].sizeDelta = objSize;

        // Prepping prefab done. Begin spawning texts.
        pool = new(prefab, transform);
        prefab.SetActive(false);
        trailingObject = null;

        // Initialize our texts by spawning enough to fill the bounding rect.
        for (float leftPosition=GetXPosition(boundingRect)-0.5f*GetWidth(boundingRect); // Start at left edge
                   leftPosition<GetXPosition(boundingRect)+0.5f*GetWidth(boundingRect); // Go until right edge
                   leftPosition+=objSize.x) {

            // Get a new object.
            GameObject obj = RequestAndInitialize();
            // Calculate and apply the spawn position.
            Vector3 pos = new(
                leftPosition + 0.5f*GetWidth(rectDict[obj]), // Align left of obj with leftPos.
                prefab.transform.localPosition.y,            // Same height as prefab.
                0
            );
            obj.transform.localPosition = pos;

            // Find the trailing object.

            // We spawn objects left to right. If we're scrolling right,
            // the trailing object should be the leftmost, the first spawned.
            // If we're scrolling left, the trailing object should be the rightmost,
            // the last spawned.

            if (scrollSpeed > 0) { // Trailing object should be the first encountered.
                if (trailingObject == null) {
                    trailingObject = obj;
                }
            } else { // Trailing object should be the last encountered.
                trailingObject = obj;
            }
        }
    }

    private void Update()
    {
        // Update is called once per frame. We use it to:
        //  * Move all our objects to the left.
        //  * Check if all our objects are far enough to the left that we need
        //    to deactivate one.
        //  * Check if the trailing object is far enough from the start that we
        //    need to request a new object.
        // ================

        foreach (GameObject currentObject in pool.pool.ToArray()) {    
            if (!currentObject.activeSelf) continue;

            // Move all our objects to the left.
            rectDict[currentObject].localPosition += scrollSpeed * Time.deltaTime * Vector3.right;

            // If the object is out of bounds...
            if (FullyOut(rectDict[currentObject], boundingRect)) {
                // Deactivate it!
                pool.Deactivate(currentObject);
            }

            // If this object is the trailing object...
            if (currentObject == trailingObject) {
                // And it's fully in bounds...
                if (FullyIn(rectDict[trailingObject], boundingRect)) {
                    // Request a new one!
                    trailingObject = RequestAndInitialize();
                    // Initialize its position.
                    SetSpawnPosition(trailingObject);
                }
            }
        }

        bool FullyOut(RectTransform rect, RectTransform bounds)
        {
            // Local function for determining if rect lies fully out of bounds.
            // ================

            return (
                GetXPosition(rect) < -0.5f*GetWidth(bounds) - 0.5f*GetWidth(rect)
             || GetXPosition(rect) > 0.5f*GetWidth(bounds) + 0.5f*GetWidth(rect)
            );
        }

        bool FullyIn(RectTransform rect, RectTransform bounds)
        {
            // Local function for determining if rect lies fully within bounds.
            // ================

            return (
                GetXPosition(rect) < 0.5f*GetWidth(bounds) - 0.5f*GetWidth(rect)
             && GetXPosition(rect) > -0.5f*GetWidth(bounds) + 0.5f*GetWidth(rect)
            );            
        }
    }

    private GameObject RequestAndInitialize()
    {
        // An extension method for our pool.Request() method, which also
        // ensures the requested object is in our rect and text dictionaries,
        // and has had its size and text content properly set.
        //
        // Updates the size and text content of the prefab as well.
        // ================

        GameObject obj = pool.Request();

        // Check if the object is in our rectDict already, and add it if not.
        if (!rectDict.ContainsKey(obj)) {
            if (!obj.TryGetComponent<RectTransform>(out RectTransform rect)) {
                Debug.LogError("LeapfrogTextTicker Error: RequestAndInitialize failed. "
                             + "trailingObject does not have a RectTransform.", obj);
                return null;
            } else {
                rectDict[obj] = rect;
            }
        }

        // Check if the object is in our textDict already, and add it if not.
        if (!textDict.ContainsKey(obj)) {
            if (!obj.TryGetComponent<TMP_Text>(out TMP_Text text)) {
                Debug.LogError("LeapfrogTextTicker Error: RequestAndInitialize failed. "
                             + "trailingObject does not have a TMP_Text.", obj);
                return null;
            } else {
                textDict[obj] = text;
            }
        }

        // Update the prefab values.
        Vector2 prefabSize = prefab.GetComponent<TMP_Text>().GetPreferredValues();
        rectDict[prefab].sizeDelta = new(paddingWidth*prefabSize.x, prefabSize.y);
        textDict[prefab].text = textContent;

        // Assign the prefab values to our requested object.
        rectDict[obj].sizeDelta = rectDict[prefab].sizeDelta;
        textDict[obj].text = textDict[prefab].text;

        obj.SetActive(true);
        return obj;
    }

    private void SetSpawnPosition(GameObject obj)
    {
        Vector3 spawnPosition = new(
            0.5f*GetWidth(boundingRect) + 0.5f*GetWidth(rectDict[obj]), // Just offscreen
            prefab.transform.localPosition.y,                           // Our current height
            0
        );
        // If scrolling right, spawn left. Vice versa.
        spawnPosition.x *= -Mathf.Sign(scrollSpeed);

        obj.transform.localPosition = spawnPosition;
    }

    private float GetXPosition(RectTransform rect)
    {
        return rect.anchoredPosition.x;
    }

    private float GetWidth(RectTransform rect)
    {
        return rect.rect.width;
    }
}
