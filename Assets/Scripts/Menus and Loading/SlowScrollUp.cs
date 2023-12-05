using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SlowScrollUp : MonoBehaviour
{
    [SerializeField, Tooltip("How fast this object scrolls up.")]
    private float speed = 1;
    [SerializeField]
    private bool invokeAction = false;
    [SerializeField]
    private int[] heightKeyframes;
    private int nextFrameIndex = 1;
    public UnityEvent actionOnReachKeyframe;

    private Vector3 basePosition;

    private void Start()
    {
        // Initializes our basePosition.
        // ================
        
        basePosition = transform.localPosition;
    }

    private void Update()
    {
        // Update is called once per frame. We use it to go up!
        // ================

        transform.localPosition += speed * Time.deltaTime * Vector3.up;

        if (invokeAction && nextFrameIndex < heightKeyframes.Length && transform.localPosition.y > heightKeyframes[nextFrameIndex])
        {
            actionOnReachKeyframe?.Invoke();
        }
    }

    public void Reset()
    {
        // Resets our position to the initial state.
        // ================
        
        transform.localPosition = basePosition;
    }

    public void Hide()
    {
        // ...
        // ================
        
        nextFrameIndex++;
        gameObject.SetActive(false);
    }

    public void Show()
    {
        // ...
        // ================
        
        gameObject.SetActive(true);
    }

    public void ScanToKeyframe(int keyframe)
    {
        // ...
        // ================
        
        print($"Scanned to index {keyframe}, height {heightKeyframes[keyframe]}!");
        transform.localPosition = heightKeyframes[keyframe] * Vector3.up;
    }
}
