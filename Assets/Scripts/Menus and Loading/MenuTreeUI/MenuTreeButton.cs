using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuTreeButton : MonoBehaviour
{
    public enum Style { Main, Back }

    // ==============================================================
    // Parameters
    // ==============================================================

    [SerializeField, Tooltip("A binder of UI icons that pairs string IDs with sprites.")]
    private UISpriteBinder iconSprites;
    [SerializeField, Tooltip("The image displaying our icon.")]
    private Image icon;
    [SerializeField, Tooltip("The amount of time it takes our icon to fully rotate when the button is hovered over."
                            +"\n\nDefault: 0.5")]
    private float spinDuration = 0.5f;

    // ==============================================================
    // Internal variables
    // ==============================================================

    // The child node of the current that this button is tracking.
    private MenuTreeNode node;
    // The base scale of this object.
    private Vector3 baseScale;

    // ==============================================================
    // Initializers
    // ==============================================================

    void Awake()
    {
        baseScale = transform.localScale;
    }

    public void Initialize(MenuTreeNode _node)
    {
        node = _node;

        // If the node is null, treat this as a Back button.
        // Hacky, but it'll work for our purposes.
        if (_node == null) {
            icon.sprite = iconSprites.GetDefault();
        } else {
            icon.sprite = iconSprites.Query(node.id);
        }

        icon.transform.rotation = Quaternion.identity;
    }

    // ==============================================================
    // Manipulators
    // ==============================================================

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

    public void SpinIcon()
    {
        StartCoroutine(SpinIconRoutine());
    }

    private IEnumerator SpinIconRoutine()
    {
        // Spins our icon!
        // ================

        float elapsed = 0;

        while (elapsed < spinDuration) {
            float amount = LerpKit.EaseInOut(elapsed/spinDuration, 3);
            icon.transform.rotation = Quaternion.Euler(0, amount*360, 0);

            yield return null;
            elapsed += Time.deltaTime;
        }

        icon.transform.rotation = Quaternion.identity;
    }
}
