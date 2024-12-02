using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitLoader : MonoBehaviour
{
    [SerializeField]
    private SaveHandler saveHandler;
    bool loaded = false;

    // Update is called once per frame
    void Update()
    {
        if (!loaded && !LevelLoader.Instance.GetAnimator().gameObject.activeInHierarchy && Input.GetButtonDown("Fire1")) {
            loaded = true;

            if (!saveHandler.GetPlayedBefore()) {
                LevelLoader.Instance.LoadLevel("Cutscene Intro");
            } else { // Not our first rodeo
                LevelLoader.Instance.LoadLevel("MainMenu");
            }
        }
    }
}
