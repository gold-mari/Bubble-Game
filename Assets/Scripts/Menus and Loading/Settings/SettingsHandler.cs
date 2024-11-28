using UnityEngine;

public class SettingsHandler : MonoBehaviour
{
    private FMODVolumeHandler volumeHandler;
    private ResolutionFullscreenHandler resFullscreen;
    private AccessibilitySettingsHandler accessibility;

    public void Initialize()
    {
        volumeHandler = GetComponentInChildren<FMODVolumeHandler>(includeInactive:true);
        volumeHandler.Initialize();

        resFullscreen = GetComponentInChildren<ResolutionFullscreenHandler>(includeInactive:true);
        resFullscreen.Initialize();

        accessibility = GetComponentInChildren<AccessibilitySettingsHandler>(includeInactive:true);
        accessibility.Initialize();
    }    
}