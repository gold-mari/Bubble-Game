using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
using FMOD.Studio;

[System.Serializable]
public class SceneData
{
    [Tooltip("The name of this scene data.")]
    public string name;
    [Scene, Tooltip("The scene associated with this name.")]
    public string scene;
}

public class LevelLoader : MonoBehaviour
{
    [SerializeField, Tooltip("The scenes we can transition to from this level.")]
    private SceneData[] scenes;
    [SerializeField, Tooltip("The SFX used when transitioning in.")]
    private FMODUnity.EventReference inSFX;
    [SerializeField, Tooltip("The SFX used when transitioning out.")]
    private FMODUnity.EventReference outSFX;
    [SerializeField, Tooltip("The path of the FMOD music bus. Find it in the mixer by right clicking the " +
                             "Music group and selecting Copy Path.")]
    public string musicBusPath = "bus:/Music";
    [SerializeField, Tooltip("Whether or not we should play the transition animation when starting the scene.\n\nDefault: true")]
    private bool transitionStart = true;

    private Dictionary<string, string> sceneDict = new();
    private Animator transitionAnimator;
    private PauseManager pauseManager;
    private string queuedLevel = "NULL";
    private FMOD.Studio.Bus musicBus;

    void Awake()
    {
        transitionAnimator = GetComponentInChildren<Animator>(true);
        pauseManager = transform.parent.GetComponentInChildren<PauseManager>();

        if (transitionAnimator && transitionStart)
        {
            transitionAnimator.gameObject.SetActive(true);
            transitionAnimator.SetTrigger("out");
            FMODUnity.RuntimeManager.PlayOneShot(outSFX);
        }
    }

    void Start()
    {
        // Start is called before the first frame update. We use it to populate our scene dict.
        // ================

        musicBus = FMODUnity.RuntimeManager.GetBus(musicBusPath);

        foreach (SceneData sceneData in scenes)
        {
            sceneDict.Add(sceneData.name, sceneData.scene);
        }
    }

    public void LoadLevel(string levelName)
    {
        // Loads a level based on our levelName.
        // ================

        Debug.Assert(sceneDict.ContainsKey(levelName), $"LevelLoader error: LoadLevel failed. {levelName} was not found " +
                                                        "as a name in the scenes array.", this);

        if (!transitionAnimator)
        {
            SceneManager.LoadScene(sceneDict[levelName]);
        }
        else
        {
            queuedLevel = levelName;
            
            TransitionFlavorRandomizer flavorRandomizer = GetComponent<TransitionFlavorRandomizer>();
            if (flavorRandomizer)
            {
                flavorRandomizer.Randomize();
            }

            if (pauseManager) {
                print("Locked pause to true");
                pauseManager.LockPause(true);
            }

            transitionAnimator.gameObject.SetActive(true);
            transitionAnimator.SetTrigger("in");
            FMODUnity.RuntimeManager.PlayOneShot(inSFX);
            musicBus.stopAllEvents(STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void LoadQueuedLevel()
    {
        // Loads a level that was queued up elsewhere.
        // ================

        Debug.Assert(sceneDict.ContainsKey(queuedLevel), $"LevelLoader error: LoadLevel failed. Queued level {queuedLevel} was not " +
                                                        "found as a name in the scenes array.", this);

        SceneManager.LoadScene(sceneDict[queuedLevel]);
    }
}
