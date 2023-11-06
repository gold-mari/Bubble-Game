using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

[System.Serializable]
public class SceneData
{
    [Tooltip("The name of this scene data.")]
    public string name;
    [Scene, Tooltip("The scene associate with this name.")]
    public string scene;
}

public class LevelLoader : MonoBehaviour
{
    [SerializeField, Tooltip("The scenes we can transition to from this level.")]
    private SceneData[] scenes;

    private Dictionary<string, string> sceneDict = new();

    void Start()
    {
        // Start is called before the first frame update. We use it to populate our scene dict.
        // ================

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

        SceneManager.LoadScene(levelName);
    }

    public void ReloadLevel()
    {
        // Loads once more the level we are currently in.
        // ================

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
