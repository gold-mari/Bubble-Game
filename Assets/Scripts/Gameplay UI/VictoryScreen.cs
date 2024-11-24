using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class VictoryScreen : MonoBehaviour
{ 
    [SerializeField, Tooltip("The event that is called when this screen is dismissed.")]
    private UnityEvent onDoneTicking;
    [SerializeField, Tooltip("Whether or not this screen starts visible.\n\nDefault: false")]
    private bool startVisible = false;
    [SerializeField, Tooltip("The transform all our stat tickers live under.")]
    private Transform statTickerParent;
    [SerializeField, Tooltip("The VictoryMusicHandler that fully starts us.")]
    private VictoryMusicHandler musicHandler;



    [Tooltip("The SFX played on score counter incrementing.")]
    public FMODUnity.EventReference scoreClickerSFX;

    [Tooltip("The SFX played on score display.")]
    public FMODUnity.EventReference scoreSFX;



    private List<VictoryStatDisplay> statDisplays = new();
    private CanvasGroup group;
    private VictoryRankCalculator rankCalculator;
    private bool isVisible = false;
    private bool doneTicking = false;

    private void Awake()
    {
        // Awake is called before all Start().
        // ================

        // GetComponentsInChildren does not have a deterministic order.
        // Go by transform index order.
        for (int i = 0; i < statTickerParent.childCount; i++) {
            // print($"VictoryScreen: Child {i} searched was named {statTickerParent.GetChild(i).name}");
            VictoryStatDisplay display = statTickerParent.GetChild(i).GetComponentInChildren<VictoryStatDisplay>(true);
            if (display != null) statDisplays.Add(display);
        }
    }

    void Start()
    {
        // Start is called before the first frame update.
        // ================

        group = GetComponent<CanvasGroup>();
        group.alpha = 0;

        rankCalculator = GetComponent<VictoryRankCalculator>();

        SetVisibility(startVisible);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isVisible) return;
        if (Time.timeScale == 0) return;
            
        if (Input.GetButtonDown("Fire1")) {
            if (!doneTicking) {
                StopAllCoroutines();
                FinishDisplaysImpatient();
            }
        }
    }

    public void SetVisibility(bool visibility)
    {
        // Sets the visibility of the victory screen.
        // ================

        isVisible = visibility;
        group.alpha = visibility ? 1 : 0;

        if (visibility) {
            // Prepare to start ticking.
            doneTicking = false;

            // When the music tells us to show the body,
            // start ticking up our displays.
            musicHandler.ShowBody += () => {
                StartCoroutine(TickAllDisplays());
            };

            // Start the music.
            musicHandler.StartMusic();
            // Calculate the rank.
            string rank = rankCalculator.CalculateRank();
            Debug.Log( "=================================");
            Debug.Log($"          our rank is: {rank}         ");
            Debug.Log( "=================================");
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

        // print("VictoryScreen: All displays done ticking.");
        doneTicking = true;
        onDoneTicking?.Invoke();
    }

    private void FinishDisplaysImpatient()
    {
        foreach (VictoryStatDisplay display in statDisplays) {
            display.FinishTickingImpatient();
        }

        // print("VictoryScreen: All displays done ticking.");
        doneTicking = true;
        onDoneTicking?.Invoke();
    }
}
