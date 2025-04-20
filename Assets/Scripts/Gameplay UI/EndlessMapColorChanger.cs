using UnityEngine;
using NaughtyAttributes;

public class EndlessMapColorChanger : MapColorChanger
{
    [SerializeField, ReadOnly]
    private bool listenToMarkers;

    protected override void Awake()
    {
        if (musicManager is EndlessMusicManager) {
            (musicManager as EndlessMusicManager).OnNextStage += OnNextStage;
            print("Initialized with EndlessMusicManager");
        }

        base.Awake();
    }

    private void OnNextStage(EndlessMusicManager.EndlessStage stage)
    {
        print("OnNextStage");
        listenToMarkers = stage.switchColorOnMap;

        halftoneBackManager.baseColor = stage.baseColor;
        halftoneBackManager.orbColor = stage.orbColor;
    }

    protected override void OnSwitchMap(string mapName)
    {
        if (listenToMarkers) {
            base.OnSwitchMap(mapName);
        }
    }
}
