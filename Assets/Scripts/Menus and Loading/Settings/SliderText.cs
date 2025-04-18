using UnityEngine;
using TMPro;

public class SliderText : MonoBehaviour
{
    [SerializeField, Tooltip("The text object we're updating the contents of.")]
    private TMP_Text textObj;
    [SerializeField, Tooltip("The format string that (value*scalar) is applied to.\n\nDefault: '(##0.%)'")]
    private string formatString = "(##0.%)";


    public void UpdateText(float value)
    {
        if (textObj != null) {
            textObj.text = value.ToString(formatString);
        } else print("Was null");
    }
}
