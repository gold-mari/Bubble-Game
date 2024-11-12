using UnityEngine;
using UnityEngine.Events;

public class RankDisplay : MonoBehaviour
{
    [SerializeField, Tooltip("The SFX played on revealing the rank.")]
    private FMODUnity.EventReference rankRevealSFX;
    [SerializeField]
    private UnityEvent onDismiss;



    private bool dismissed = false;



    void OnEnable()
    {
        FMODUnity.RuntimeManager.PlayOneShot(rankRevealSFX);
    }

    void Update()
    {
        // Update is called once per frame. We use it to detect input.
        // ================

        if (!gameObject.activeInHierarchy) return;
        if (dismissed) return;

        if (Input.GetButtonDown("Fire1")) {
            onDismiss?.Invoke();
            dismissed = true;
        }
    }
}