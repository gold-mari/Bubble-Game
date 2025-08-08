using System.Collections;
using UnityEngine;

public class InitLoader : MonoBehaviour
{
    [SerializeField]
    private SaveHandler saveHandler;
    [SerializeField]
    private CanvasGroup splashes;
    [SerializeField]
    private CanvasGroup controls;
    public float splashTime = 1.5f;
    public float fadeTime = 0.75f;
    bool splashed = false;
    bool loaded = false;

    IEnumerator Start()
    {
        splashes.alpha = 1;
        controls.alpha = 0;
        yield return new WaitForSeconds(splashTime);

        splashed = true;

        float elapsed = 0;
        float duration = fadeTime;
        while (elapsed < duration)
        {
            splashes.alpha = LerpKit.EaseIn(LerpKit.Flip(elapsed/duration), 3);
            elapsed += Time.deltaTime;
            yield return null;
        }
        splashes.alpha = 0;

        elapsed = 0;
        while (elapsed < duration)
        {
            controls.alpha = LerpKit.EaseIn(elapsed/duration, 3);
            elapsed += Time.deltaTime;
            yield return null;
        }
        controls.alpha = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (!splashed) return;
        if (loaded) return;
        if (LevelLoader.Instance.GetAnimator().gameObject.activeInHierarchy) return;
        if (!InputHandler.GetAffirmDown()) return;
        
        loaded = true;

        if (!saveHandler.GetPlayedBefore())
        {
            LevelLoader.Instance.LoadLevel("Cutscene Intro");
        }
        else
        { // Not our first rodeo
            LevelLoader.Instance.LoadLevel("MainMenu");
        }
    }
}
