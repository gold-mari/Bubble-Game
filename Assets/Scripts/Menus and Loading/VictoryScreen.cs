using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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


    [Tooltip("The text object we write our rank to.")]
    public TMP_Text rankTextObj;
    [Tooltip("Different colors for our ranks.")]
    public RankColors rankColors;


    [Tooltip("The SFX played on score counter incrementing.")]
    public FMODUnity.EventReference scoreClickerSFX;
    [Tooltip("The SFX played on score display.")]
    public FMODUnity.EventReference scoreSFX;



    private List<VictoryStatDisplay> statDisplays = new();
    private CanvasGroup group;
    private VictoryRankCalculator rankCalculator;
    private bool isVisible = false;
    private bool startedTicking = false, doneTicking = false;

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

        rankColors.InitializeDict();
        SetVisibility(startVisible);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isVisible) return;
        if (Time.timeScale == 0) return;
            
        if (InputHandler.GetAffirmDown() && startedTicking && !doneTicking) {
            StopAllCoroutines();
            FinishDisplaysImpatient();
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
            rankTextObj.text = rank;

            if (rankColors.pairDict.ContainsKey(rank)) {
                RankColors.RankColor rc = rankColors.pairDict[rank];
                rankTextObj.colorGradient = new(rc.color1, rc.color2, rc.color1, rc.color2);
            }
        }
    }

    private IEnumerator TickAllDisplays()
    {
        // To allow all colors in the displays to initialize.
        yield return null;

        startedTicking = true;

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

[System.Serializable]
public class RankColors
{
    [System.Serializable]
    public struct RankColor
    {
        public string rank;
        public Color color1, color2;
    }

    public RankColor[] pairs;
    public Dictionary<string, RankColor> pairDict = new();

    public void InitializeDict()
    {
        foreach (RankColor pair in pairs) {
            pairDict.Add(pair.rank, pair);
        }
    }
}
