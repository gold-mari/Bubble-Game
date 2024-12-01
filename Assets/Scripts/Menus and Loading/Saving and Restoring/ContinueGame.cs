using UnityEngine;
using UnityEngine.UI;

public class ContinueGame : MonoBehaviour
{
    [SerializeField, Tooltip("The SaveHandler in the scene.")]
    private SaveHandler saveHandler;
    [SerializeField, Tooltip("The LeveLoader in the scene.")]
    private LevelLoader levelLoader;

    // private void Start()
    // {
    //     if (saveHandler.GetLastPlayedScene() == null) {
    //         // If we have no saved game, don't let us press the continue button.
    //         continueButton.interactable = false;
    //     } else {
    //         // Elsewise, hook up the button to our loader.
    //         continueButton.onClick.AddListener(LoadLastPlayedScene);
    //     }
    // }

    public void LoadLastPlayedScene()
    {
        string lastPlayedScene = saveHandler.GetLastPlayedScene();
        if (lastPlayedScene == null) {
            Debug.LogError("ContinueGame Error: LoadLastPlayedScene failed. lastPlayedScene was null.");
            return;
        }

        levelLoader.LoadLevel(lastPlayedScene);
    }
}