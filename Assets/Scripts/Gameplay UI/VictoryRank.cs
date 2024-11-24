using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryRank : MonoBehaviour
{
    [SerializeField, Tooltip("The SFX played on revealing the rank.")]
    private FMODUnity.EventReference rankRevealSFX;

    public void Appear()
    {
        gameObject.SetActive(true);
        FMODUnity.RuntimeManager.PlayOneShot(rankRevealSFX);
    }
}
