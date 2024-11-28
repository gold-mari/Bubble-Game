using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SliderText : MonoBehaviour
{
    [SerializeField, Tooltip("The format string that (value*scalar) is applied to.\n\nDefault: '(##0.%)'")]
    private string formatString = "(##0.%)";


    private TMP_Text textObj;


    private void Awake()
    {
        textObj = GetComponent<TMP_Text>();
    }

    public void UpdateText(float value)
    {
        if (textObj != null) {
            textObj.text = value.ToString(formatString);
        }
    }
}
