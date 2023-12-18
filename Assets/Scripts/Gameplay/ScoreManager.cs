using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [SerializeField, Tooltip("How many points a standard chain break gets us.\n\nDefault: 100")]
    uint baseScoreAmount = 100;
    [SerializeField, Tooltip("What size of a combo is considered 'exceptional'. Combos at or past exceptional get extra UI effects.\n\nDefault: 5")]
    uint exceptionalCombo = 5;
    [SerializeField, Tooltip("The music manager present in the scene.")]
    private MusicManager manager;
    [SerializeField, Tooltip("The amount of time, in quarter notes, after a chain break before our combo goes down a rank.\n\nDefault: 4")]
    private uint quarterDuration = 4;

    [SerializeField, Tooltip("The UI image that represents our combo cooldown.")]
    Image cooldownMeter;
    [SerializeField, Tooltip("The uintVar storing our current combo.")]
    uintVar currentCombo;
    [SerializeField, Tooltip("The uintVar storing our total score.")]
    uintVar scoreVar;
    [SerializeField, Tooltip("The uintVar storing the max chain length. Used to calculate the overpop bonus.")]
    uintVar maxChainLength;

    // ================================================================
    // Internal variables
    // ================================================================

    private ScorePopupManager popupManager;
    private TimelineHandler handler;
    private Coroutine cooldown = null;
    private int comboLevel = 0;

    // ================================================================
    // Initializer methods
    // ================================================================

    private void Awake()
    {
        // Awake is called before Start.
        // ================

        scoreVar.value = 0;
        popupManager = GetComponent<ScorePopupManager>();
    }

    // ================================================================
    // Chain break methods
    // ================================================================

    public void LogChainBreak(Chain chain)
    {
        // Called from the ChainBreakHandler. Notes that a chain has broken, and 
        // ================

        // Lazily initialize handler. If null, then assign it.
        handler ??= manager.handler;

        cooldownMeter.fillAmount = 1;

        if (cooldown == null)
        {
            comboLevel = 1;
            cooldown = StartCoroutine(ComboRoutineCooldown());        
        }
        else
        {
            comboLevel++;
            StopCoroutine(cooldown);
            cooldown = StartCoroutine(ComboRoutineCooldown());        
        }

        uint multiplier = chain.length-maxChainLength.value + 1;
        uint score = (uint)(multiplier*comboLevel*baseScoreAmount);
        scoreVar.value += score;

        popupManager.OnChainBreak(chain, score, (uint)comboLevel, exceptionalCombo, multiplier);
    }

    // ================================================================
    // Update methods
    // ================================================================

    private void Update()
    {
        // Update is called once per frame.
        // ================

        currentCombo.value = (uint)comboLevel;

        if (comboLevel == 0)
        {
            cooldownMeter.fillAmount = 0;
        }
    }

    private IEnumerator ComboRoutineCooldown()
    {
        float duration = (float)(handler.length4th * quarterDuration);

        while (comboLevel > 0)
        {
            cooldownMeter.fillAmount = 1;
            float elapsed = 0;
            while (elapsed < duration)
            {
                cooldownMeter.fillAmount = 1-elapsed/duration;
                elapsed += Time.deltaTime;
                yield return null;
            }
            cooldownMeter.fillAmount = 0;

            comboLevel--;
        }

        cooldown = null;
    }
}
