using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;

public class SIXAttractVid : MonoBehaviour
{
    public SIXLevelSelect levelSelect;
    public MenuTree menuTree;
    public CanvasGroup mainGroup;
    public VideoPlayer player;
    public CanvasGroup transGroup;

    public float transTime = 0.2f;

    private WaitForSecondsRealtime transWait;
    private EventSystem eventSystem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transWait = new(transTime);
        eventSystem = EventSystem.current;
    }

    [Button]
    public void EnterAttract()
    {
        StopCoroutine(EnterAttractRoutine());
        StartCoroutine(EnterAttractRoutine());

        IEnumerator EnterAttractRoutine()
        {
            eventSystem.enabled = false;
            levelSelect.enabled = false;
            float elapsed = 0;

            mainGroup.alpha = 0;
            transGroup.alpha = 1;
            while (elapsed < transTime) {
                mainGroup.alpha = LerpKit.EaseIn(elapsed/transTime);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
                Debug.Log($"elapsed:{elapsed} | elapsed<transTime:{elapsed<transTime}");
            }

            mainGroup.alpha = 1;

            player.time = 0;
            player.Prepare();
            
            yield return transWait;
            player.Play();
            
            elapsed = 0;
            while (elapsed < transTime) {
                transGroup.alpha = LerpKit.EaseIn(LerpKit.Flip(elapsed/transTime));
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            transGroup.alpha = 0;
        }
    }

    [Button]
    public void ExitAttract()
    {
        StopCoroutine(ExitAttractRoutine());
        StartCoroutine(ExitAttractRoutine());

        IEnumerator ExitAttractRoutine()
        {
            float elapsed = 0;

            mainGroup.alpha = 1;
            transGroup.alpha = 0;
            while (elapsed < transTime) {
                transGroup.alpha = LerpKit.EaseIn(elapsed/transTime);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            transGroup.alpha = 1;
            player.Stop();
            menuTree.WarpToRoot();

            yield return transWait;
                        
            elapsed = 0;
            while (elapsed < transTime) {
                mainGroup.alpha = LerpKit.EaseIn(LerpKit.Flip(elapsed/transTime));
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            mainGroup.alpha = 0;
            eventSystem.enabled = true;
            levelSelect.enabled = true;
        }
    }
}
