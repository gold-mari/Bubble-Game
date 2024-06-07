using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using System;

public class ScoreManager : MonoBehaviour
{
    // ================================================================
    // Parameters
    // ================================================================

    [SerializeField, Tooltip("How many points a standard chain break gets us.\n\nDefault: 100")]
    uint baseScoreAmount = 100;
    [SerializeField, Tooltip("The number of points a base end pop gets us.\n\nDefault: 5")]
    uint baseEndPopAmount = 5;
    [SerializeField, Tooltip("The uintVar storing what size of a combo is considered 'exceptional'. Combos at or past exceptional get extra UI effects.\n\nDefault: 5")]
    uintVar exceptionalCombo;
    [SerializeField, Tooltip("The music manager present in the scene.")]
    private MusicManager manager;
    [SerializeField, Tooltip("The screenshaker present in the scene.")]
    private NaiveScreenshake screenshake;
    [SerializeField, Tooltip("The amount of time, in quarter notes, after a chain break before our combo goes down a rank.\n\nDefault: 4")]
    private uint quarterDuration = 4;

    [SerializeField, Tooltip("The UI image that represents our combo cooldown.")]
    Image cooldownMeter;
    [SerializeField, Tooltip("The uintVar storing our current combo.")]
    uintVar currentCombo;
    [SerializeField, Tooltip("The uintVar storing our max combo this stage.")]
    uintVar maxCombo;
    [SerializeField, Tooltip("The uintVar storing our total score.")]
    uintVar scoreVar;
    [SerializeField, Tooltip("The uintVar storing our total number of points from popped straggler bubbles.")]
    uintVar stragglerBonus;
    [SerializeField, Tooltip("The uintVar storing our total number of bubbles popped.")]
    uintVar bubblesPopped;
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
        maxCombo.value = currentCombo.value = 0;
        bubblesPopped.value = 0;
        stragglerBonus.value = 0;
        popupManager = GetComponent<ScorePopupManager>();
    }

    // ================================================================
    // Chain break methods
    // ================================================================

    public void LogChainBreak(Chain chain)
    {
        // Called from the ChainBreakHandler. Notes that a chain has broken, updates score,
        // updates combo, and calls screenshake.
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

        bubblesPopped.value += chain.length;

        uint overpopMultiplier = chain.length-maxChainLength.value + 1;
        uint score = (uint)(overpopMultiplier*comboLevel*baseScoreAmount);
        scoreVar.value += score;

        popupManager.OnChainBreak(chain, score, (uint)comboLevel, exceptionalCombo.value, overpopMultiplier);
        screenshake.ScaledShake(comboLevel * overpopMultiplier);
    }

    public void LogEndPop(Bubble bubble, int index)
    {
        // Called from the EndBubbleClearer. Notes that a bubble has popped, and calls
        // the appropriate UI code.
        // ================

        // Lazily initialize handler. If null, then assign it.
        handler ??= manager.handler;

        bubblesPopped.value++;
        uint score = (uint)(baseEndPopAmount * index);
        stragglerBonus.value += score;
        scoreVar.value += score;

        popupManager.OnEndPop(bubble, score);
        screenshake.ScaledShake(index * 0.25f);
    }

    // ================================================================
    // Update methods
    // ================================================================

    private void Update()
    {
        // Update is called once per frame.
        // ================

        currentCombo.value = (uint)comboLevel;

        if (comboLevel == 0) {
            cooldownMeter.fillAmount = 0;
        }
        if (comboLevel > maxCombo.value) {
            maxCombo.value = (uint)comboLevel;
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
