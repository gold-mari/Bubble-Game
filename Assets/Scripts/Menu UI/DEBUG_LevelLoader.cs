using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public class DEBUG_LevelLoader : MonoBehaviour
{
    [Scene]
    public string level1, level2, level3, level4, level5;

    public void LoadLevel(int level)
    {
        switch (level)
        {
            case 1: SceneManager.LoadScene(level1); break;
            case 2: SceneManager.LoadScene(level2); break;
            case 3: SceneManager.LoadScene(level3); break;
            case 4: SceneManager.LoadScene(level4); break;
            case 5: SceneManager.LoadScene(level5); break;
            default: Debug.LogError($"INVALID LEVEL ID ENTERED: {level}. Must be 1-5."); break;
        }
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
