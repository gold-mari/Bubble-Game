using System.Linq;
using UnityEngine;
using NaughtyAttributes;

public class EndlessRecedeOnMarker : RecedeOnMarker
{
    [SerializeField]
    private Song targetSong;
    [SerializeField, ReadOnly]
    private bool listenToMarkers;

    protected override void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        if (musicManager is EndlessMusicManager) {
            (musicManager as EndlessMusicManager).OnNextStage += OnNextStage;
        }
    }

    private void OnNextStage(EndlessMusicManager.EndlessStage stage)
    {
        listenToMarkers = stage.switchColorOnMap;

        if (stage.song == targetSong || (listenToMarkers && visibleByDefault)) {
            animator.ResetTrigger("goBack");
            animator.SetTrigger("goFront");
        } else {
            animator.ResetTrigger("goFront");
            animator.SetTrigger("goBack");
        }
    }

    protected override void OnSwitchMap(string mapName)
    {
        if (listenToMarkers) {
            base.OnSwitchMap(mapName);
        }
    }
}
