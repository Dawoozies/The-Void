using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Run : StateMachineBehaviour
{
    //Do state machine stuff when you really need it
    //But for activating the physics system we should definitely have a single script which deals with all the physics stuff depending on state
    //Maybe we could take a middle ground
    //The rigidbody related to the animator
    Rigidbody2D rb;
    public float speedMultiplier;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    void GetRigidbody(Animator animator)
    {
        if(rb == null)
        {
            //Since the animator is on a child object to the rigidbody2D
            rb = animator.GetComponentInParent<Rigidbody2D>();
        }
    }
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetRigidbody(animator);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetRigidbody(animator);
        if (rb != null)
        {
            if (Mathf.Abs(BasicInput.ins.InputLHorizontal) > 0f)
            {
                rb.AddForce(new Vector2(BasicInput.ins.InputLHorizontal * speedMultiplier, 0f), ForceMode2D.Impulse);
                Debug.Log(rb.velocity);
            }
        }
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
