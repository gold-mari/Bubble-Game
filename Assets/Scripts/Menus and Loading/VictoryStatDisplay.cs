using System;
using System.Collections;
using System.Timers;
using NaughtyAttributes;
using UnityEngine;

public class VictoryStatDisplay : uintVarMonitor 
{
    [Header("VictoryStatDisplay Parameters")]
    [SerializeField, Tooltip("The default numeric value of this text from which we count to our true value.\n\nDefault: 0")]
    private uint initialValue = 0;
    [SerializeField, Tooltip("The amount of time, in seconds, it takes this object to tick up.")]
    private float tickTime = 1;
    [SerializeField, Tooltip("The exponential power applied to our lerp functions.\n\nDefault: 2")]
    private float lerpPower = 2;
    [SerializeField, Tooltip("The maximum character spacing.\n\nDefault: 0")]
    private float baseCharSpacing = 0;
    [SerializeField, Tooltip("The amount of time, in seconds, from the base char spacing to the max.\n\nDefault: 0.2")]
    private float growTime = 0.2f;
    [SerializeField, Tooltip("The maximum character spacing.\n\nDefault: 25")]
    private float maxCharSpacing = 25;
    [SerializeField, Tooltip("The amount, in points, we increase our font size by when reaching the end of our tick anim.\n\nDefault: 10")]
    private float fontSizeDelta = 10;
    [SerializeField, Tooltip("The alpha value of this text when inactive (not yet ticked).\n\nDefault: 0.2")]
    private float inactiveAlpha = 0.2f;

    [SerializeField, Tooltip("The SFX played on score counter incrementing.")]
    private FMODUnity.EventReference scoreClickerSFX;

    [SerializeField, Tooltip("The SFX played on score display.")]
    private FMODUnity.EventReference scoreSFX;

    // The lerp amount used with GetUInt.
    private float lerpAmount;
    // The screenshake component on this component.
    private NaiveScreenshake shaker;
    // The base font size on our textObject.
    private float baseFontSize;
    // The base / transparent text color on our textObject.
    private Color baseFontColor, inactiveFontColor;

    // ================================================================
    // Initializer methods
    // ================================================================

    private void Start()
    {
        shaker = GetComponent<NaiveScreenshake>();

        baseFontSize = textObject.fontSize;
        baseFontColor = textObject.color;
        inactiveFontColor = new Color(baseFontColor.r, baseFontColor.g, baseFontColor.b, inactiveAlpha);
        textObject.color = inactiveFontColor;
    }

    // ================================================================
    // Text-getting methods
    // ================================================================

    protected override uint GetUInt()
    {
        // Returns the value of our UIntVar, used in Update().
        // Overridden in child classes.
        // ================

        return (uint)Mathf.Lerp(initialValue, UIntVar.value, lerpAmount);
    }

    // ================================================================
    // Accessor methods
    // ================================================================

    public bool IsDone()
    {
        return lerpAmount == 1;
    }

    public void StartTicking()
    {
        StartCoroutine(DisplayTickUpRoutine());
    }

    public void setSFXRefs(FMODUnity.EventReference scoreClickerSFXRef, FMODUnity.EventReference scoreSFXRef)
    {
        scoreClickerSFX = scoreClickerSFXRef;
        scoreSFX = scoreSFXRef;
    }

    // ================================================================
    // Animation methods
    // ================================================================

    private IEnumerator DisplayTickUpRoutine() 
    {
        float elapsed = 0;
        lerpAmount = 0;
        textObject.color = inactiveFontColor;

        while (elapsed < growTime) {
            textObject.characterSpacing = Mathf.Lerp(baseCharSpacing, maxCharSpacing, LerpKit.EaseOut(elapsed/growTime));
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0;
        while (elapsed < tickTime) {
            lerpAmount = LerpKit.EaseInOut(elapsed/tickTime, lerpPower);
            textObject.characterSpacing = Mathf.Lerp(maxCharSpacing, baseCharSpacing, LerpKit.EaseOut(elapsed/tickTime));
            textObject.color = Color.Lerp(inactiveFontColor, baseFontColor, LerpKit.EaseOut(elapsed/tickTime));

            FMOD.Studio.EventInstance scoreClickerInstance = FMODUnity.RuntimeManager.CreateInstance(scoreClickerSFX);
            scoreClickerInstance.start();
            scoreClickerInstance.release(); 
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        lerpAmount = 1;
        textObject.characterSpacing = baseCharSpacing;
        textObject.color = baseFontColor;
        
        if (shaker) shaker.BaseShake();

        FMOD.Studio.EventInstance scoreInstance = FMODUnity.RuntimeManager.CreateInstance(scoreSFX);
        scoreInstance.start();
        scoreInstance.release(); 

        StartCoroutine(ShrinkRoutine());
    }

    private IEnumerator ShrinkRoutine() 
    {
        float elapsed = 0;
        float animTime = (shaker != null) ? shaker.BaseShakeDuration : 0;
        float bigFontSize = baseFontSize+fontSizeDelta;
        textObject.fontSize = bigFontSize;

        while (elapsed < animTime) {
            textObject.fontSize = Mathf.Lerp(bigFontSize, baseFontSize, LerpKit.EaseOut(elapsed/animTime, 2.5f));

            elapsed += Time.deltaTime;
            yield return null;
        }

        textObject.fontSize = baseFontSize;
    }

}