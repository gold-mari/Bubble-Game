using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class RecedeOnMarker : ActionOnSwitchMap
{
    [SerializeField, Tooltip("The map name we listen for. If we encounter it, we appear. Otherwise, we recede.")]
    string targetMap;
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
        print($"====== SWITCHED TO {mapName} ======");
        if (mapName == targetMap)
        {
            animator.ResetTrigger("goBack");
            animator.SetTrigger("goFront");
        }
        else
        {
            animator.ResetTrigger("goFront");
            animator.SetTrigger("goBack");
        }
    }
}
