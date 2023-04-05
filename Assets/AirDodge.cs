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

        Vector2 L_Input = InputManager.ins.L_Input;
        Vector2 waveDashHelp = new Vector2();
        if(Mathf.Abs(L_Input.x) > 0.35f && L_Input.y < -0.25f)
        {
            //Then add some x velocity in the direction the player is trying to go
            waveDashHelp.x = Mathf.Sign(L_Input.x) * 10f;
            waveDashHelp.y = -10f;
        }
        airDodgeVelocity = new Vector2(L_Input.x, L_Input.y) * airDodgeSpeed + waveDashHelp;

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
