using UnityEngine;

// Generate a screenshot and save it to disk with the name SomeLevel.png.

public class ScreenshotOnClick : MonoBehaviour
{
    [SerializeField] string title;
    [SerializeField] uint index;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            string filename = $"Screenshots/{title}{index}.png";
            ScreenCapture.CaptureScreenshot(filename);
            print($"Saved {filename} to the Screenshots folder.");
            index++;
        }
    }
}