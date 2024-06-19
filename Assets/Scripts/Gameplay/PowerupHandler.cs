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
    [SerializeField, Tooltip("The BubbleSpawner in this scene (to access bubble size).")]
    private BubbleSpawner bubbleSpawner;
    [SerializeField, Tooltip("The amount we scale bubble size by. 1 is no change in size.\n\nDefault: 0.8")]
    private float bubbleScale = 0.8f;


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

        bubbleSpawner.SetBubbleScale(bubbleScale);
    } 

    void OnDestroy()
    {
        // OnDestroy is called when this component is destroyed, including on scene end. 
        // We use it to RESET to our initial values.
        // ================

        maxChainLength.value = (uint)chainLengthValues.x;
    }
}
