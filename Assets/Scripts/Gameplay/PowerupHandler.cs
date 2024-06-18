using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupHandler : MonoBehaviour
{
    [SerializeField, Tooltip("The uintVar holding our max chain length setting.")]
    private uintVar maxChainLength;
    [SerializeField, Tooltip("The (before, after) values for maxChainLength.\n\nDefault: (4, 3)")]
    private Vector2Int chainLengthValues = new(4,3);
    [SerializeField, Tooltip("The ScoreManager in this scene (to access combo duration).")]
    private ScoreManager scoreManager;
    [SerializeField, Tooltip("The amount we scale combo time by.\n\nDefault: 2")]
    private float comboTimeScale = 2;


    void Awake()
    {
        // Awake is called before start. We use it to set our initial values.
        // ================

        maxChainLength.value = (uint)chainLengthValues.x;
    }

    public void PowerUp()
    {
        // Called from the TimeoutHandler. Sets our final values.
        // ================

        maxChainLength.value = (uint)chainLengthValues.y;

        uint newComboDuration = (uint)(scoreManager.GetCooldownQuarterDuration() * comboTimeScale);
        scoreManager.SetCooldownQuarterDuration(newComboDuration);
    }    
}
