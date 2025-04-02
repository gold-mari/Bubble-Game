using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodaLoader : MonoBehaviour
{
    [SerializeField]
    private SaveHandler saveHandler;
    bool loaded = false;

    void Update()
    {
        if (!loaded && !LevelLoader.Instance.GetAnimator().gameObject.activeInHierarchy && InputHandler.GetAffirmDown()) {
            loaded = true;

            saveHandler.FinishedGame();
            LevelLoader.Instance.LoadLevel("MainMenu");
        }
    }
}
