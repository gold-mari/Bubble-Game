using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuContentFade : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================

    [SerializeField, Tooltip("Whether we should use unscaled time for our animations.\n\nDefault: false")]
    bool unscaledTime = false;
    [SerializeField, Tooltip("The amount of time it takes our icon to fade in when initialized.\n\nDefault: 0.1")]
    private float fadeDuration = 0.1f;

    // ==============================================================
    // Internal variables
    // ==============================================================

    // The canvas group on this object.
    private CanvasGroup canvasGroup;

    // ==============================================================
    // Initializers
    // ==============================================================

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnEnable()
    {
        if (canvasGroup) {
            StopAllCoroutines();
            StartCoroutine(FadeRoutine());
        }
    }

    private IEnumerator FadeRoutine()
    {
        // Fades in our icon!
        // ================

        float elapsed = 0;
        canvasGroup.alpha = 0;

        while (elapsed < fadeDuration) {
            canvasGroup.alpha = LerpKit.EaseIn(elapsed/fadeDuration, 3);

            yield return null;
            float deltaTime = unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            elapsed += deltaTime;
        }

        canvasGroup.alpha = 1;
    }
}
