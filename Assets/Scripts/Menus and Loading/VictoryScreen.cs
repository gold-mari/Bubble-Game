using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField, Tooltip("The event that is called when this screen is dismissed.")]
    private UnityEvent onDismiss;

    private CanvasGroup group;
    private bool isVisible = false;

    // Start is called before the first frame update
    void Start()
    {
        group = GetComponent<CanvasGroup>();
        group.alpha = 0;
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
        group.alpha = 1;
    }
}
