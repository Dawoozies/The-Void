using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirDodge : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    Rigidbody2D rb;
    //We should in future get the air dodge speed from a player settings scriptable object or something
    public float airDodgeSpeed = 10f;
    Vector2 airDodgeVelocity;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (rb == null)
            rb = animator.GetComponentInParent<Rigidbody2D>();

        Vector2 rInput = InputManager.ins.L_Input;
        airDodgeVelocity = new Vector2(rInput.x, rInput.y) * airDodgeSpeed;

        animator.SetInteger("AirDodgeCount", 1);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb.velocity = airDodgeVelocity;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
