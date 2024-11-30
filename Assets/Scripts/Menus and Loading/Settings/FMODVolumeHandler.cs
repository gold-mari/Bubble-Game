using UnityEngine;
using UnityEngine.UI;

// Works in editor!

public class FMODVolumeHandler : MonoBehaviour
{
    public enum TargetBus { Master, Music, SFX };

    // ==============================================================
    // Parameters
    // ==============================================================

    [SerializeField, Tooltip("The path of the FMOD master bus. Find it in the mixer by right clicking the " +
                             "Master group and selecting Copy Path.")]
    private string masterBusPath = "bus:/";
    [SerializeField, Tooltip("The path of the FMOD music bus. Find it in the mixer by right clicking the " +
                             "Music group and selecting Copy Path.")]
    private string musicBusPath = "bus:/Music";
    [SerializeField, Tooltip("The path of the FMOD SFX bus. Find it in the mixer by right clicking the " +
                             "SFX group and selecting Copy Path.")]
    private string sfxBusPath = "bus:/SFX";
    [SerializeField, Tooltip("The slider used to update this bus' volume.")]
    private Slider masterSlider, musicSlider, sfxSlider;
    
    // ==============================================================
    // Misc. internal variables
    // ==============================================================

    private FMOD.Studio.Bus masterBus, musicBus, sfxBus;
    private float masterVolume, musicVolume, sfxVolume;
    private bool initialized = false;

    // ==============================================================
    // Initializers/finalizers
    // ==============================================================

    public void Initialize()
    {
        // THIS FUNCTION IS CALLED ONLY ONCE PER SCENE, on Awake().
        // ================

        // Load settings from prefs

        if (PlayerPrefs.HasKey("MasterVolumePref")) {
            masterVolume = PlayerPrefs.GetFloat("MasterVolumePref");
        } else { // Default value.
            masterVolume = 0.8f;
        }
        if (PlayerPrefs.HasKey("MusicVolumePref")) {
            musicVolume = PlayerPrefs.GetFloat("MusicVolumePref");
        } else { // Default value.
            musicVolume = 0.8f;
        }
        if (PlayerPrefs.HasKey("SFXVolumePref")) {
            sfxVolume = PlayerPrefs.GetFloat("SFXVolumePref");
        } else { // Default value.
            sfxVolume = 0.8f;
        }

        // Load FMOD values from settings

        masterBus = FMODUnity.RuntimeManager.GetBus(masterBusPath);
        musicBus = FMODUnity.RuntimeManager.GetBus(musicBusPath);
        sfxBus = FMODUnity.RuntimeManager.GetBus(sfxBusPath);

        initialized = true;

        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume); 
    }

    private void OnEnable()
    {
        InitializeSliders();
    }

    private void InitializeSliders()
    {
        masterSlider.value = masterVolume;
        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;
    }

    private void OnDisable()
    {
        SaveToPrefs();
    }

    public void SaveToPrefs()
    {
        PlayerPrefs.SetFloat("MasterVolumePref", masterVolume);
        PlayerPrefs.SetFloat("MusicVolumePref", musicVolume);
        PlayerPrefs.SetFloat("SFXVolumePref", sfxVolume);
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

        FMOD.Studio.Bus bus;
        switch (target) {
            case TargetBus.Music:
                bus = musicBus;
                musicVolume = volume;
                break;
            case TargetBus.SFX:
                bus = sfxBus;
                sfxVolume = volume;
                break;
            default:
                bus = masterBus;
                masterVolume = volume;
                break;
        };

        bus.setVolume(volume);
    }
}