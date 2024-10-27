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
    [Scene]
    public string cut1, cut2, cut3, cut4, cut5;

    public void LoadLevel(int level)
    {
        switch (level)
        {
            case -1: SceneManager.LoadScene(cut1); break;
            case -2: SceneManager.LoadScene(cut2); break;
            case -3: SceneManager.LoadScene(cut3); break;
            case -4: SceneManager.LoadScene(cut4); break;
            case -5: SceneManager.LoadScene(cut5); break;
            case 1: SceneManager.LoadScene(level1); break;
            case 2: SceneManager.LoadScene(level2); break;
            case 3: SceneManager.LoadScene(level3); break;
            case 4: SceneManager.LoadScene(level4); break;
            case 5: SceneManager.LoadScene(level5); break;
            default: Debug.LogError($"INVALID LEVEL ID ENTERED: {level}."); break;
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
