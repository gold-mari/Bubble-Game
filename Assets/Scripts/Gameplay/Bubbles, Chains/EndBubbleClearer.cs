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
            }

            index++;
            yield return wait;
        }

        print("All done!");
        OnAllPopped?.Invoke();
    }
}
