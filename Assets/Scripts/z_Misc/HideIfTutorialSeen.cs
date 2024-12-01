using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideIfTutorialSeen : MonoBehaviour
{
    [SerializeField, Tooltip("The SaveHandler in this scene.")]
    private SaveHandler saveHandler;

    private void OnEnable()
    {
        if (saveHandler.GetSeenTutorial()) {
            gameObject.SetActive(false);
        }
    }
}
