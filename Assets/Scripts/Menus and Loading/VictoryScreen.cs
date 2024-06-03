using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class VictoryScreen : MonoBehaviour
{ 
    [SerializeField, Tooltip("The event that is called when this screen is dismissed.")]
    private UnityEvent onDismiss;
    [SerializeField, Tooltip("Whether or not this screen starts visible.\n\nDefault: false")]
    private bool startVisible = false;

    private VictoryStatDisplay[] statDisplays = null;
    private CanvasGroup group;
    private bool isVisible = false;

    // Start is called before the first frame update
    void Start()
    {
        statDisplays = GetComponentsInChildren<VictoryStatDisplay>();

        group = GetComponent<CanvasGroup>();
        group.alpha = 0;

        SetVisibility(startVisible);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isVisible) return;
            
        if (Input.GetButtonDown("Fire1")) {
            onDismiss?.Invoke();
        }
    }

    public void SetVisibility(bool visibility)
    {
        // Sets the visibility of the victory screen.
        // ================

        isVisible = visibility;
        group.alpha = visibility ? 1 : 0;

        if (visibility) {
            StartCoroutine(TickAllDisplays());
        }
    }

    private IEnumerator TickAllDisplays()
    {
        foreach (VictoryStatDisplay display in statDisplays) {
            display.StartTicking();
            yield return new WaitUntil(display.IsDone);
        }

        print("VictoryScreen: All displays done ticking.");
    }
}
