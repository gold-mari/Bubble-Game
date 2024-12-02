using UnityEngine;

public class DisableFromFinishedGame : MonoBehaviour
{
    public SaveHandler saveHandler;
    public bool disableIfFinished;

    private void OnEnable()
    {
        if (disableIfFinished) {
            if (saveHandler.GetFinishedGame()) {
                gameObject.SetActive(false);
            }        
        } else { // disable if not finished
            if (!saveHandler.GetFinishedGame()) {
                gameObject.SetActive(false);
            }
        }
    }
}