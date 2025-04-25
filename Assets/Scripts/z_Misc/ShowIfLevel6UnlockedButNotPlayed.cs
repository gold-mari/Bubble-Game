using UnityEngine;

public class ShowIfLevel6UnlockedButNotPlayed : MonoBehaviour
{
    [SerializeField, Tooltip("The SaveHandler in this scene.")]
    private SaveHandler saveHandler;

    private void OnEnable()
    {
        bool shouldShow = saveHandler.GetBeatEndless() && !saveHandler.GetPlayedLevel6();
        
        if (!shouldShow) {
            gameObject.SetActive(false);
        }
    }
}