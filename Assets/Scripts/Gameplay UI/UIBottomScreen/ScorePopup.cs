using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Buffers.Text;

public class ScorePopup : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================
    
    [SerializeField, Tooltip("The amount of time we wait, in seconds, after spawning before deleting ourselves.\n\nDefault: 1")]
    private float decayTime = 1f;
    [SerializeField, Tooltip("The amount of time it takes, in seconds, to cycle through all hues for a high combo.\n\nDefault: 1")]
    private float colorLoopPeriod = 1f;
    [SerializeField, Tooltip("The maximum character width we lerp on our text element.\n\nDefault: 20")]
    private float maxCharWidth = 20;
    [SerializeField, Tooltip("The default font size.\n\nDefault: 30")]
    private float baseFontSize = 30;
    [SerializeField, Tooltip("The amount we increase the font size by per combo rank.\n\nDefault: 2.5")]
    private float sizePerComboRank = 2.5f;
    [SerializeField, Tooltip("The boolVar holding whether or not we should reduce flashing.")]
    private boolVar reduceFlashing;

    // ==============================================================
    // Internal variables
    // ==============================================================

    private TMP_Text text;
    private Color baseColor;
    private float ourFontSize;

    // ==============================================================
    // Initialization methods
    // ==============================================================

    void Awake()
    {
        // Awake is called before Start.
        // ================

        text = GetComponent<TMP_Text>();
    }

    void Start()
    {
        // Start is called before the first frame update.
        // ================

        StartCoroutine(Decay());
    }

    public void Initialize(uint score, Color color)
    {
        // Lightweight call. Used for end bubble pops.
        // ================

        Initialize(score, 1, 2, 0, color);
    }

    public void Initialize(uint score, uint combo, uint exceptionalCombo, uint overpop, Color color)
    {
        // The whole shebang. Used for chain breaks.
        // ================

        text.text = score.ToString() + "!";
        float extraFontSize = 9 * Mathf.Log(((combo-1)*sizePerComboRank)+1, 10); // Magic formula, dampens out high combos.
        ourFontSize = baseFontSize + extraFontSize;
        text.fontSize = ourFontSize;

        if (overpop > 1)
        {
            text.text = $"<size=150%>OVERPOP! x{overpop}</size>\n" + text.text;
        }

        text.outlineColor = text.color = baseColor = color;
        if (combo >= exceptionalCombo)
        {
            StartCoroutine(LoopColor());
        }
    }

    // ==============================================================
    // Continuous methods
    // ==============================================================

    private IEnumerator Decay()
    {
        float elapsed = 0;
        Color clear;
        while (elapsed < decayTime)
        {
            text.characterSpacing = maxCharWidth * elapsed/decayTime;
            clear = new(baseColor.r,baseColor.g,baseColor.b,0);
            text.color = Color.Lerp(baseColor, clear, LerpKit.EaseIn(elapsed/decayTime));
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    private IEnumerator LoopColor()
    {
        while (true)
        {
            if (reduceFlashing.value) {
                // If we're reducing flashing, loop through color slower.
                colorLoopPeriod *= 3;
            }

            float elapsed = 0;
            while (elapsed < colorLoopPeriod)
            {
                text.outlineColor = baseColor = Color.HSVToRGB(elapsed/colorLoopPeriod, 0.9f, 1);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}
