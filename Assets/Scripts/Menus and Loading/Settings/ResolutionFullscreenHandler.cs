using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// ================================================================================================================================
// Resolution and fullscreen code from Lance Talbert, on red-gate.com.
// Accessed from: https://www.red-gate.com/simple-talk/development/dotnet-development/how-to-create-a-settings-menu-in-unity/
// ================================================================================================================================
public class ResolutionFullscreenHandler : MonoBehaviour
{
    [SerializeField, Tooltip("The dropdown which controls our resolution settings.")]
    TMP_Dropdown resolutionDropdown;

    private Vector2Int[] resolutions;

    // ==============================================================
    // Initializers
    // ==============================================================

    public void Initialize()
    {
        // Parse the base resolutions.
        HashSet<Vector2Int> resolutionSet = new();
        foreach (Resolution r in Screen.resolutions) {
            // We want to add two things to this set, per default resolution.
            // 1. A square aspect ratio, with side length equal to the shortest side
            //    of the default resolution.
            // 2. The default resolution.
            // If the default resolution is shorter than 16:9, skip BOTH these steps!
            // ================

            // If the aspect ratio is equal to 16:9 or is taller...
            if (r.width/r.height <= 16f/9f) { 
                // Find the shortest dimension, and add a square version of it.
                int shortest = (r.width < r.height) ? r.width : r.height;
                resolutionSet.Add(new(shortest, shortest));

                resolutionSet.Add(new(r.width, r.height));
            }
        }
        resolutions = resolutionSet.ToArray();

        resolutionDropdown.ClearOptions();
        List<string> options = new();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++) {
            Vector2Int res = resolutions[i];
            string option = $"{res.x} x {res.y}";
            // The recommended resolution is the highest one.
            if (i == resolutions.Length-1) option += " (Recommended)";
            options.Add(option);

            // If this resolution matches our current resolution, note that!
            if (res.x == Screen.currentResolution.width && res.y == Screen.currentResolution.height) {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    // ==============================================================
    // Public accessors
    // ==============================================================

    public void SetResolution(int index)
    {
        Vector2Int dim = resolutions[index];
        Screen.SetResolution(dim.x, dim.y, Screen.fullScreen);
    }

    public void SetFullscreen(bool isFullscreen) 
    {
        Screen.fullScreen = isFullscreen;
    }
}
