using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroDirector : DialogueActor
{
    [SerializeField]
    private SlowScrollUp backScroll;
    [SerializeField]
    private SlowScrollUp textScroll;

    private void Start()
    {
        
    }

    public override void Trigger(string action)
    {
        // Called from the DialogueHandler.
        // ================

        if (action == "Reset")
        {  
            backScroll.Reset(); 
            textScroll.Reset(); 
        }
        else if (action == "Hide")
        {
            backScroll.Hide(); 
            textScroll.Hide(); 
        }
        else if (action == "Show")
        {
            backScroll.Show(); 
            textScroll.Show(); 
        }
        else
        {
            string[] tokens = action.Split('/');
            if (tokens.Length == 2 && tokens[1].Length == 1 && tokens[0] == "ScanToKeyframe")
            {
                int param = int.Parse(tokens[1]);
                backScroll.ScanToKeyframe(param);
                textScroll.ScanToKeyframe(param);
            }
            else 
            {
                Debug.LogWarning($"IntroDirector warning: Trigger ran into unexpected input. No trigger parameter named \"{action}\" exists "
                                +$"for this object.", this);
            }
        }
    }
}
