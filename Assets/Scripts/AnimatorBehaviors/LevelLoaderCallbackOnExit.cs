using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoaderCallbackOnExit : StateMachineBehaviour
{
    /*override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // OnStateEnter is called when a transition starts and the state machine starts
        // to evaluate this state.
        // ================  
    }*/

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{ 
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LevelLoader loader = animator.transform.parent.GetComponent<LevelLoader>();
        if (!loader)
        {
            Debug.LogError("LevelLoaderCallbackOnExit error: OnStateExit() failed. The parent to this animator does not "
                         + "have a LevelLoader component.", animator.transform);
            return;
        }

        loader.LoadQueuedLevel();
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
