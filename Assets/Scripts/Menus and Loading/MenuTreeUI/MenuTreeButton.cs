using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuTreeButton : MonoBehaviour
{
    public enum Style { Main, Back }

    private TMP_Text labelText;
    private Vector3 baseScale;

    // Start is called before the first frame update
    void Awake()
    {
        labelText = GetComponentInChildren<TMP_Text>();
        baseScale = transform.localScale;
    }

    public void ChangeText(string newText)
    {
        labelText.text = newText;
    }

    public void SetStyle(Style style)
    {
        switch (style)
        {
            case Style.Main: {
                transform.localScale = baseScale;
                break;
            }
            case Style.Back: {
                transform.localScale = 0.5f*baseScale;
                break;
            }
        }
    }

}
