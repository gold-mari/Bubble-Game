using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using NaughtyAttributes;

public class BaseMenuContent : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================

    [SerializeField, Tooltip("The text object displaying the ID of the current menu node.")]
    private TMP_Text titleTextObj;
    [SerializeField, Tooltip("The text formula used to fill out titleTextObject.")]
    private string titleFormula;
    [SerializeField, Tooltip("The text object displaying the ID of the current menu node.")]
    private TMP_Text subtitleTextObj;
    [SerializeField, Tooltip("The actual text applied to idContent when hovering over a back button.")]
    private string backTitleContent;
    [SerializeField, Tooltip("The text formula used to fill out desciptionContent when hovering over a backButton.")]
    private string backSubtitleFormula;

    // ==============================================================
    // Internal variables
    // ==============================================================

    // The current node the menu is on. Used to determine this node's parent.
    private MenuTreeNode currentNode;

    // ==============================================================
    // Manipulator methods
    // ==============================================================

    public void UpdateCurrentNode(MenuTreeNode node)
    {
        currentNode = node;
    }

    public void ChangeText(MenuTreeNode node)
    {
        // Updates our currentMenuText based on the current node.
        // ================

        if (node == null) { // For MenuTreeButtons, null is "Back".
            if (currentNode.parent == null) {
                Debug.LogError("BaseMenuContent Error: node was null, implying we're hovering " +
                               "over a 'Back' button. However, BaseMenuContent's currentNode has no parent.");
            } else {
                titleTextObj.text = backTitleContent;
                FormatAndUpdate(subtitleTextObj, backSubtitleFormula, currentNode.parent.id);
            }
        } else { // Display something other than "Back".
            FormatAndUpdate(titleTextObj, titleFormula, node.id);
            subtitleTextObj.text = node.description;
        }
    }

    // ==============================================================
    // Helper methods
    // ==============================================================

    private void FormatAndUpdate(TMP_Text textObject, string formula, string id)
    {
        string tagOpen = "_{";
        string tagClose = "}";

        string processedString = formula;

        // Loop ends when from == -1: when there are no more opening tags.
        for (int from = formula.IndexOf(tagOpen); from > -1; from = formula.IndexOf(tagOpen, from+tagOpen.Length))
        {
            int to = formula.IndexOf(tagClose, from);
            string formulaID = formula[(from+tagOpen.Length)..to];

            string trueID = formulaID switch {
                "id" => id.ToLower(),
                "ID" => id.ToUpper(),
                _ => id,
            };

            processedString = processedString.Replace(tagOpen+formulaID+tagClose, trueID);
        }

        textObject.text = processedString;
    }
}
