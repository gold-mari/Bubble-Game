using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class ComboTextColor : MonoBehaviour
{
    [SerializeField, Tooltip("The uintVar storing what size of a combo is considered 'exceptional'. Combos at or past exceptional get extra UI effects.\n\nDefault: 5")]
    uintVar exceptionalCombo;
    [SerializeField, Tooltip("The uintVar storing our current combo.")]
    uintVar currentCombo;

    TMP_Text text;
    float hue = 0;
    Color baseColor;


    void Awake()
    {
        // Awake is called before Start.
        // ================

        text = GetComponent<TMP_Text>();
        baseColor = text.color;
    }

    void Update()
    {
        // Update is called once per frame.
        // ================

        // Do nothing if our combo is not exceptional.
        if (currentCombo.value < exceptionalCombo.value)
        {
            if (text.color != baseColor) {
                text.color = baseColor;
            }
            return;
        }
        
        text.color = Color.HSVToRGB(hue,1,0.8f);
        hue = (hue+Time.deltaTime)%1;
    }
}
