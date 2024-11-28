using UnityEngine;

public class SettingsHandler : MonoBehaviour
{
    private ResolutionFullscreenHandler resFullscreen;

    public void Initialize()
    {
        resFullscreen = GetComponentInChildren<ResolutionFullscreenHandler>(includeInactive:true);
        resFullscreen.Initialize();
    }    
}