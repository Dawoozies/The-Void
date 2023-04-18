using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePhysics_SetLinearDrag : StateMachineBehaviour
{
    public float onEnterDrag = -1;
    public float onUpdateDrag = -1;
    public float onExitDrag = -1;
    Rigidbody2D rb;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (rb == null)
            rb = animator.GetComponentInParent<Rigidbody2D>();
        if (onEnterDrag < 0)
            return;
        rb.drag = onEnterDrag;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (rb == null)
            rb = animator.GetComponentInParent<Rigidbody2D>();
        if (onUpdateDrag < 0)
            return;
        rb.drag = onUpdateDrag;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (rb == null)
            rb = animator.GetComponentInParent<Rigidbody2D>();
        if (onExitDrag < 0)
            return;
        rb.drag = onExitDrag;
    }
}
