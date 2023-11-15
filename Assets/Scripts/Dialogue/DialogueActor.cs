using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueActor : MonoBehaviour
{
    // ==============================================================
    // Internal variables
    // ==============================================================

    private Animator anim;
    private HashSet<string> validParameters = new();

    // ==============================================================
    // Initialization methods
    // ==============================================================

    private void Start()
    {
        // Start is called before the first frame update. We use it to define our animator.
        // ================

        anim = GetComponent<Animator>();

        // Store all the valid parameters, to protect against invalid triggers.
        if (anim)
        {
            for (int i = 0; i < anim.parameterCount; i++)
            {
                // If it's a trigger, add it. We check in case there are non-trigger parameters with the same
                // name as an invalid trigger.
                if (anim.parameters[i].type == AnimatorControllerParameterType.Trigger)
                {
                    validParameters.Add(anim.parameters[i].name);
                }
            }
        }
    }

    // ==============================================================
    // Misc methods
    // ==============================================================

    public void Trigger(string action)
    {
        // Called from the DialogueHandler.
        // ================

        if (validParameters.Contains(action))
        {
            anim.SetTrigger(action);
        }
        else
        {
            Debug.LogWarning($"DialogueActor warning: Trigger ran into unexpected input. No trigger parameter named \"{action}\" exists "
                            +$"on the animator on this object.", this);
        }
    }
}