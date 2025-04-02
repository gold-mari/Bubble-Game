using UnityEngine;
using UnityEngine.Events;

public class RankDisplay : MonoBehaviour
{
    [SerializeField, Tooltip("The SFX played on revealing the rank.")]
    private FMODUnity.EventReference rankRevealSFX;
    [SerializeField]
    private UnityEvent onDismiss;



    private bool dismissed = false;



    public void PlaySFX()
    {
        FMODUnity.RuntimeManager.PlayOneShot(rankRevealSFX);
    }

    void Update()
    {
        // Update is called once per frame. We use it to detect input.
        // ================

        if (!gameObject.activeInHierarchy) return;
        if (dismissed) return;
        if (Time.timeScale == 0) return;

        if (InputHandler.GetAffirmDown()) {
            onDismiss?.Invoke();
            dismissed = true;
        }
    }
}