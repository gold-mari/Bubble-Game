using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorRingAnimator : MonoBehaviour
{
    [Tooltip("The boolVar which signals if gravity is flipped to point outwards instead of inwards.")]
    [SerializeField]
    private boolVar gravityFlipped;
    // Updated whenever gravityFlipped changes to store the previous value of brightness. We
    // compare this to the current value of gravityFlipped to detect if it has changed.
    private bool lastGravityFlipped;
    // The animator on this object.
    Animator animator;

    void Start()
    {
        // Start is called before the first frame update. We use it to initialize
        // lastGravityFlipped and define animator.
        // ================
        
        lastGravityFlipped = gravityFlipped.value;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Update is called once per frame. We use it to compare gravityFlipped against
        // lastGravityFlipped. If they differ, we update the animator.
        // ================

        if ( lastGravityFlipped != gravityFlipped.value ) {
            lastGravityFlipped = gravityFlipped.value;
            animator.SetBool("gravityFlipped", gravityFlipped.value);
        }
    }
}
