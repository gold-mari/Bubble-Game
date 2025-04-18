using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================

    [SerializeField, Tooltip("The boolVar, corresponding to if dangerAmount is greater than or equal to "
                           + "dangerThreshold in DangerManager.")]
    private boolVar inDanger;


    // ==============================================================
    // Internal variables
    // ==============================================================

    // The animator on this gameObject.
    private Animator animator;

    // ==============================================================
    // Default methods
    // ==============================================================

    void Start()
    {
        // Start is called before the first frame update. We use it to define animator.
        // ================

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Update is called once per frame. Used to communicate the value of inDanger to
        // the animator.
        // ================

        if (animator)
        {
            animator.SetBool("playerDanger", inDanger.value);
        }
    }

    // ==============================================================
    // Event-handling methods
    // ==============================================================

    public void OnChainBreak()
    {
        // A public function called by ChainBreakHandler via an event when chains break.
        // Sets the chainBreak trigger in the animator.
        // ================

        if (animator)
        {
            animator.SetTrigger("chainBreak");
        }
    }

    public void OnPlayerWin()
    {
        // A public function called by TimekeeperManager via an event when we win.
        // Sets the triggerWin trigger in the animator.
        // ================

        if (animator)
        {
            animator.SetTrigger("playerWin");
        }
    }

    public void OnPlayerLoss()
    {
        // A public function called by DangerManager via an event when we lose.
        // Sets the triggerLoss trigger in the animator.
        // ================

        if (animator)
        {
            animator.SetTrigger("playerLoss");
        }
    }
}
