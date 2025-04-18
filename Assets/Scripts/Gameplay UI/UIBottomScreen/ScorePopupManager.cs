using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScorePopupManager : MonoBehaviour
{
    [SerializeField]
    private GameObject popup;

    public void OnChainBreak(Chain chain, uint score, uint combo, uint exceptionalCombo, uint overpop)
    {
        Bubble[] members = chain.members.ToArray();
        Vector3 centroid = Vector2.zero;
        foreach (Bubble member in members)
        {
            // In case a bubble is destroyed before we can loop to it.
            if (member != null)
            {
                centroid += member.transform.position;
            }
        }
        centroid /= members.Length;

        ScorePopup scorePopup = Instantiate(popup, centroid, Quaternion.identity, transform).GetComponent<ScorePopup>();
        scorePopup.Initialize(score, combo, exceptionalCombo, overpop, BubbleFlavorMethods.GetColor(chain.chainFlavor));
    }

    public void OnEndPop(Bubble bubble, uint score)
    {
        ScorePopup scorePopup = Instantiate(popup, bubble.transform.position, Quaternion.identity, transform).GetComponent<ScorePopup>();
        scorePopup.Initialize(score, BubbleFlavorMethods.GetColor(bubble.bubbleFlavor));
    }
}
