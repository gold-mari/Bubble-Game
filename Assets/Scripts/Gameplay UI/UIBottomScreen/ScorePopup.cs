using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScorePopup : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================
    
    [SerializeField, Tooltip("The amount of time we wait, in seconds, after spawning before deleting ourselves.\n\nDefault: 1")]
    private float decayTime = 1f;
    [SerializeField, Tooltip("The maximum character width we lerp on our text element.\n\nDefault: 20")]
    private float maxCharWidth = 20;

    // ==============================================================
    // Internal variables
    // ==============================================================

    private TMP_Text text;
    static string[] texts = {"Cool", "Good!", "Great!!", "WOW!!", "!!!~DIGI BEAT BLAST~!!!"};

    // ==============================================================
    // Initialization methods
    // ==============================================================

    void Awake()
    {
        // Awake is called before Start.
        // ================

        text = GetComponent<TMP_Text>();
        text.text = texts[Random.Range(0,texts.Length)];
    }

    void Start()
    {
        // Start is called before the first frame update.
        // ================

        StartCoroutine(Decay());
    }

    // ==============================================================
    // Continuous methods
    // ==============================================================

    private IEnumerator Decay()
    {
        float elapsed = 0;
        Color clearWhite = new Color(1,1,1,0);
        while (elapsed < decayTime)
        {
            text.characterSpacing = maxCharWidth * elapsed/decayTime;
            text.color = Color.Lerp(Color.white, clearWhite, LerpKit.EaseOut(elapsed/decayTime));
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
