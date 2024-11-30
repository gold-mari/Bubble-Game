using UnityEngine;

public class SettingsHandler : MonoBehaviour
{
    private FMODVolumeHandler volumeHandler;
    private ResolutionFullscreenHandler resFullscreen;
    private CurtainSpriteHandler curtainHandler;
    private AccessibilitySettingsHandler accessibility;
    private BubbleColorController colorController;

    public void Initialize()
    {
        // Each individual settings script is responsible for loading from / saving to prefs.
        // THIS FUNCTION IS CALLED ONLY ONCE PER SCENE, in Awake().
        // ================
        
        volumeHandler = GetComponentInChildren<FMODVolumeHandler>(includeInactive:true);
        volumeHandler.Initialize();

        resFullscreen = GetComponentInChildren<ResolutionFullscreenHandler>(includeInactive:true);
        resFullscreen.Initialize();

        curtainHandler = GetComponentInChildren<CurtainSpriteHandler>(includeInactive:true);
        curtainHandler.Initialize();

        accessibility = GetComponentInChildren<AccessibilitySettingsHandler>(includeInactive:true);
        accessibility.Initialize();

        colorController = GetComponentInChildren<BubbleColorController>(includeInactive:true);
        colorController.Initialize();
    }
}