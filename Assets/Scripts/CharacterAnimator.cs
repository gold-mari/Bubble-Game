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
    private Animator animator;

    // ==============================================================
    // Default methods
    // ==============================================================

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("inDanger", inDanger.value);
    }

    // ==============================================================
    // Event-handling methods
    // ==============================================================

    public void OnChainBreak()
    {
        animator.SetTrigger("chainBreak");
    }

    public void OnTriggerWin()
    {
        animator.SetTrigger("triggerWin");
    }

    public void OnTriggerLoss()
    {
        animator.SetTrigger("triggerLoss");
    }
}
