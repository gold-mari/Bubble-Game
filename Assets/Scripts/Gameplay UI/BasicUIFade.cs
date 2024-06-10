using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicUIFade : MonoBehaviour
{
    [SerializeField, Tooltip("The amount of time it takes to fade out.\n\nDefault: 0.5")]
    private float fadeInTime = 0.5f;
    [SerializeField, Tooltip("The amount of time it takes to fade out.\n\nDefault: 0.5")]
    private float fadeOutTime = 0.5f;
    [SerializeField, Tooltip("Whether or not this object is visible at the start of the scene.\n\nDefault: false")]
    private bool defaultVisibility = false;

    private CanvasGroup group;
    private Coroutine activeRoutine = null;

    private void Awake()
    {
        // Awake is called before Start.
        // ================

        group = GetComponent<CanvasGroup>();
        group.alpha = defaultVisibility ? 1 : 0;
    }

    public void FadeIn()
    {
        if (activeRoutine != null) 
        {
            StopCoroutine(activeRoutine);
        }
        activeRoutine = StartCoroutine(LerpVisibility(1, fadeInTime));
    }

    public void FadeOut()
    {
        StopCoroutine(activeRoutine);
        print("Starting the fade out");
        activeRoutine = StartCoroutine(LerpVisibility(0, fadeOutTime));
    }

    IEnumerator LerpVisibility(float target, float duration)
    {
        // Lerps the alpha value of group. Starts from the current alpha, ends at target, and
        // finishes in duration seconds.
        // ================

        float start = group.alpha;
        float elapsed = 0;
        while (elapsed < duration)
        {
            group.alpha = Mathf.Lerp(start, target, elapsed/duration);
            elapsed += Time.deltaTime;
            yield return null;
            print("Fading");
        }

        // After lerping, set to exactly the correct amount to account for floating point errors.
        group.alpha = target;
    }
}
