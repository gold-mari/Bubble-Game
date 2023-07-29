using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================

    [SerializeField]
    private boolVar inDanger;
    // Used to detect when inDanger changes from false to true. Stores inDanger's last seen value.
    private bool lastInDanger;
    private Animator animator;

    // ==============================================================
    // Default methods
    // ==============================================================

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        lastInDanger = inDanger.value;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("inDanger", inDanger.value);

        // If we changed what state of danger we're in,
        if ( lastInDanger != inDanger.value ) {
            // update lastInDanger.
            lastInDanger = inDanger.value;
            // If we are now in danger, change the state.
            if ( inDanger.value ) {
                animator.SetTrigger("changeState");
            }
        }
    }

    // ==============================================================
    // Event-handling methods
    // ==============================================================

    public void OnChainBreak()
    {
        animator.SetTrigger("chainBreak");
        animator.SetTrigger("changeState");
    }

    public void OnTriggerWin()
    {
        animator.SetTrigger("triggerWin");
        animator.SetTrigger("changeState");
    }

    public void OnTriggerLoss()
    {
        animator.SetTrigger("triggerLoss");
        animator.SetTrigger("changeState");
    }
}
