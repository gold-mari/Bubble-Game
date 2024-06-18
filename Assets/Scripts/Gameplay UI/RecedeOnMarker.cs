using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class RecedeOnMarker : ActionOnSwitchMap
{
    [SerializeField, Tooltip("The map name we listen for. If we encounter it, we appear. Otherwise, we recede.")]
    string targetMap;
    [SerializeField, Tooltip("If this is true, instead of only appearing for the targetMap, we only HIDE for the " +
                             "targetMap.\n\nDefault: false")]
    bool invertLogic = false;
    [SerializeField, Tooltip("Whether or not we're visible by default.")]
    bool visibleByDefault = false;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        if (!visibleByDefault)
        {
            animator.SetTrigger("startBack");
        }
    }

    protected override void OnSwitchMap(string mapName)
    {
        bool isTarget = mapName == targetMap;

        // We should go front if:
        //    1. Logic isn't inverted and we're on our target map
        //    2. Logic IS inverted and we're NOT on our target map
        // And should go back otherwise. XOR does this.

        if (invertLogic ^ isTarget)
        {
            animator.ResetTrigger("goBack");
            animator.SetTrigger("goFront");
        }
        else
        {
            animator.ResetTrigger("goFront");
            animator.SetTrigger("goBack");
            Debug.Log($"{name} is going back");
        }
    }
}
