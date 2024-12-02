using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutroLoader : MonoBehaviour
{
    [SerializeField]
    private SaveHandler saveHandler;

    // Update is called once per frame
    public void PickScene()
    {
        if (!saveHandler.GetFinishedGame()) {
            LevelLoader.Instance.LoadLevel("Coda");
        } else { // Not our first rodeo
            LevelLoader.Instance.LoadLevel("MainMenu");
        }
    }
}
