using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuTreeButton : MonoBehaviour
{
    public enum Style { Main, Back }

    public MenuTreeNode node;
    private TMP_Text labelText;
    private Vector3 baseScale;

    // Start is called before the first frame update
    void Awake()
    {
        labelText = GetComponentInChildren<TMP_Text>();
        baseScale = transform.localScale;
    }

    public void Initialize(MenuTreeNode _node)
    {
        node = _node;

        // If the node is null, treat this as a Back button.
        // Hacky, but it'll work for our purposes.
        if (_node == null) {
            labelText.text = "Back";
        } else {
            labelText.text = node.id;
        }
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
