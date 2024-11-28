using UnityEngine;

public class SettingsHandler : MonoBehaviour
{
    private ResolutionFullscreenHandler resFullscreen;
    private AccessibilitySettingsHandler accessibility;

    public void Initialize()
    {
        resFullscreen = GetComponentInChildren<ResolutionFullscreenHandler>(includeInactive:true);
        resFullscreen.Initialize();

        accessibility = GetComponentInChildren<AccessibilitySettingsHandler>(includeInactive:true);
        accessibility.Initialize();
    }    
}