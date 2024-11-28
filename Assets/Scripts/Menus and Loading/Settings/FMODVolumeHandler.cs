using UnityEngine;

public class FMODVolumeHandler : MonoBehaviour
{
    public enum TargetBus { Master, Music, SFX };

    // ==============================================================
    // Parameters
    // ==============================================================

    [SerializeField, Tooltip("The path of the FMOD master bus. Find it in the mixer by right clicking the " +
                             "Master group and selecting Copy Path.")]
    public string masterBusPath = "bus:/";
    [SerializeField, Tooltip("The path of the FMOD music bus. Find it in the mixer by right clicking the " +
                             "Music group and selecting Copy Path.")]
    public string musicBusPath = "bus:/Music";
    [SerializeField, Tooltip("The path of the FMOD SFX bus. Find it in the mixer by right clicking the " +
                             "SFX group and selecting Copy Path.")]
    public string sfxBusPath = "bus:/SFX";
    
    // ==============================================================
    // Misc. internal variables
    // ==============================================================

    private FMOD.Studio.Bus masterBus, musicBus, sfxBus;
    private bool initialized = false;

    // ==============================================================
    // Initializers
    // ==============================================================

    public void Initialize()
    {
        masterBus = FMODUnity.RuntimeManager.GetBus(masterBusPath);
        musicBus = FMODUnity.RuntimeManager.GetBus(musicBusPath);
        sfxBus = FMODUnity.RuntimeManager.GetBus(sfxBusPath);

        initialized = true;
    }

    // ==============================================================
    // Public accessors
    // ==============================================================

    public void SetMasterVolume(float volume) { SetVolume(volume, TargetBus.Master); }
    public void SetMusicVolume(float volume) { SetVolume(volume, TargetBus.Music); }
    public void SetSFXVolume(float volume) { SetVolume(volume, TargetBus.SFX); }



    private void SetVolume(float volume, TargetBus target)
    {
        // Sets the volume of a particular bus.
        // ================

        if (!initialized) {
            Debug.LogError($"FMODVolumeHandler Warning: SetVolume failed. Busses weren't initialized.");
            return;
        }

        if (volume < 0 || volume > 1) {
            Debug.LogError($"FMODVolumeHandler Error: SetVolume failed. Volume was not between 0-1 ({volume}).");
            return;
        }

        FMOD.Studio.Bus bus = target switch {
            TargetBus.Music => musicBus,
            TargetBus.SFX => sfxBus,
            _ => masterBus
        };

        bus.setVolume(volume);
    }
}