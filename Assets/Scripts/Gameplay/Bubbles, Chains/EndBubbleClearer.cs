using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EndBubbleClearer : MonoBehaviour
{
    [SerializeField, Tooltip("The ScoreManager present in the scene.")]
    private ScoreManager scoreManager;
    [SerializeField, Tooltip("The amount of seconds we wait in between each bubble being cleared.\n\nDefault: 0.2")]
    private float popDelay = 0.2f;
    [SerializeField, Tooltip("A unity event called when all bubbles have been popped.")]
    public FMODUnity.EventReference burstMiniSFX;
    [SerializeField, Tooltip("The SFX played on remaining buubbles popping.")]
    private UnityEvent OnAllPopped;

    public void Clear()
    {
        StartCoroutine(ClearRoutine());
    }

    private IEnumerator ClearRoutine() 
    {
        WaitForSeconds wait = new(popDelay);

        // Find a new bubble that is not being destroyed, destroy it.
        // This is messy and bad, but at each step we just recalculate the list of all bubbles.
        bool done = false;
        int index = 1;
        while (!done) {
            Bubble[] bubbles = GetComponentsInChildren<Bubble>();
            Bubble[] freeBubbles = bubbles.Where(b => !b.isDestroying).ToArray();

            if (freeBubbles.Length == 0) {
                done = true;
            } else {
                Bubble bubble = freeBubbles[0];
                scoreManager.LogEndPop(bubble, index);
                bubble.DestroyBubble(bubble.isBomb);

                FMOD.Studio.EventInstance burstMiniSFX_i = FMODUnity.RuntimeManager.CreateInstance(burstMiniSFX);
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Flavor", (int)bubble.bubbleFlavor-1);
                burstMiniSFX_i.start();
                burstMiniSFX_i.release();
            }

            index++;
            yield return wait;
        }

        OnAllPopped?.Invoke();
    }
}
