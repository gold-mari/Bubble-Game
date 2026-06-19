using UnityEngine;
using NaughtyAttributes;

public class HardModeMonitor : MonoBehaviour
{
    private SettingsHandler settingsHandler;

    [SerializeField, ReadOnly]
    private bool hardModeValid = false;

    private void Start()
    {
        settingsHandler = FindAnyObjectByType<SettingsHandler>(FindObjectsInactive.Include);
        hardModeValid = CheckHardMode();
    }    

    private void Update()
    {
        if (!settingsHandler) return;

        // At any point during a scene (a level), if hard mode becomes invalid, it stays invalid.
        if (hardModeValid) {
            hardModeValid = CheckHardMode();
        }
    }

    private bool CheckHardMode()
    {
        if (!settingsHandler) return false;
        return settingsHandler.CheckHardMode();
    }

    public bool HardModeValid() => hardModeValid;
}