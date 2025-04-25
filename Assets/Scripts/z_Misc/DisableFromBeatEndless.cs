using UnityEngine;

public class DisableFromBeatEndless : MonoBehaviour
{
    public SaveHandler saveHandler;
    public bool disableIfBeat;

    private void OnEnable()
    {
        if (disableIfBeat) {
            if (saveHandler.GetBeatEndless()) {
                gameObject.SetActive(false);
            }        
        } else { // disable if not beat
            if (!saveHandler.GetBeatEndless()) {
                gameObject.SetActive(false);
            }
        }
    }
}