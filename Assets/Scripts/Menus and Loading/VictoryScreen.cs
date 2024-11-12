using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class VictoryScreen : MonoBehaviour
{ 
    [SerializeField, Tooltip("The event that is called when this screen is dismissed.")]
    private UnityEvent onDismiss;
    [SerializeField, Tooltip("Whether or not this screen starts visible.\n\nDefault: false")]
    private bool startVisible = false;
    [SerializeField, Tooltip("The transform all our stat tickers live under.")]
    private Transform statTickerParent;

    private List<VictoryStatDisplay> statDisplays = new();
    private CanvasGroup group;
    private bool isVisible = false;

    [Tooltip("The SFX played on score counter incrementing.")]
    public FMODUnity.EventReference scoreClickerSFX;

    [Tooltip("The SFX played on score display.")]
    public FMODUnity.EventReference scoreSFX;


    private void Awake()
    {
        // Awake is called before all Start().
        // ================

        // GetComponentsInChildren does not have a deterministic order.
        // Go by transform index order.
        for (int i = 0; i < statTickerParent.childCount; i++) {
            print($"First child searched was named {statTickerParent.GetChild(i).name}");
            VictoryStatDisplay display = statTickerParent.GetChild(i).GetComponentInChildren<VictoryStatDisplay>();
            if (display != null && display.gameObject.activeInHierarchy) statDisplays.Add(display);
        }
    }

    void Start()
    {
        // Start is called before the first frame update.
        // ================

        group = GetComponent<CanvasGroup>();
        group.alpha = 0;

        SetVisibility(startVisible);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isVisible) return;
        if (Time.timeScale == 0) return;
            
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
        // To allow all colors in the displays to initialize.
        yield return null;

        foreach (VictoryStatDisplay display in statDisplays) {
            display.setSFXRefs(scoreClickerSFX, scoreSFX);
            display.StartTicking();
            yield return new WaitUntil(display.IsDone);
        }

        print("VictoryScreen: All displays done ticking.");
    }
}
