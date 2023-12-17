using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField, Tooltip("Whether combo cooldowns have a fixed duration, or use a value from the music manager.")]
    private bool fixedCooldown = false;
    [HideIf("fixedCooldown"), SerializeField, Tooltip("The music manager present in the scene.")]
    private MusicManager manager;
    [HideIf("fixedCooldown"), SerializeField, Tooltip("The amount of time, in quarter notes, after a chain break before our combo goes down a rank.\n\nDefault: 4")]
    private uint quarterDuration = 4;
    [ShowIf("fixedCooldown"), SerializeField, Tooltip("The amount of time, in seconds, after a chain break before our combo goes down a rank.\n\nDefault: 2")]
    private float fixedDuration = 2f;
    [SerializeField, Tooltip("Whether combo cooldown decay is subtractive (shrinks by a constant subtracted amount) or multiplicative (shrinks by a constant multiplied factor).")]
    private bool subtractiveDecay = false;
    [HideIf("subtractiveDecay"), SerializeField, Tooltip("When decaying from a combo, the factor we multiply our decay time by each time we go down a combo rank.\n\nDefault: 0.8f")]
    private float relativeDecayFactor = 0.8f;

    [SerializeField, Tooltip("The UI image that represents our combo cooldown.")]
    Image cooldownMeter;
    [SerializeField, Tooltip("The uintVar storing our current combo.")]
    uintVar currentCombo;
    [SerializeField, Tooltip("The uintVar storing our total score.")]
    uintVar scoreVar;
    [SerializeField, Tooltip("The uintVar storing the max chain length. Used to calculate the overpop bonus.")]
    uintVar maxChainLength;

    private ScorePopupManager popupManager;
    private TimelineHandler handler;
    private Coroutine cooldown = null;
    private int comboLevel = 0;

    private void Awake()
    {
        scoreVar.value = 0;
        popupManager = GetComponent<ScorePopupManager>();
    }

    private void Update()
    {
        currentCombo.value = (uint)comboLevel;

        if (comboLevel == 0)
        {
            cooldownMeter.fillAmount = 0;
        }
    }

    public void LogChainBreak(Chain chain)
    {
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

        uint baseScore = 100;

        uint multiplier = chain.length-maxChainLength.value + 1;
        uint score = (uint)(multiplier*comboLevel*baseScore);
        scoreVar.value += score;

        if ( chain.length > maxChainLength.value ) print($"OVERPOP! scored {score} POINTS!! (X{multiplier} multiplier)");
        else print($"scored {score} POINTS!!");

        popupManager.OnChainBreak(chain, score, (uint)comboLevel, multiplier);
    }

    private IEnumerator ComboRoutineCooldown()
    {
        float duration = fixedCooldown ? fixedDuration : (float)(handler.length4th * quarterDuration);

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
            /*if (!subtractiveDecay) 
            {
                duration *= relativeDecayFactor; 
            }*/
        }

        cooldown = null;
    }

    public void LogScore(int amount)
    {

    }
}
