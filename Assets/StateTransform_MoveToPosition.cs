using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LinearAlgebra;
public class StateTransform_MoveToPosition : StateMachineBehaviour
{
    public Vector3 position;
    public float distanceThreshold;
    public float speed;
    public string stateTarget;
    ParametrisedLine parametrisedLine = new ParametrisedLine();
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        parametrisedLine.pathStart = animator.transform.position;
        parametrisedLine.pathEnd = position;
        float distance = Vector3.Distance(animator.transform.position, position);
        Vector3 translation = parametrisedLine.direction * distance * Time.deltaTime * speed;
        if(distance > distanceThreshold)
        {
            animator.transform.Translate(translation);
        }
        else
        {
            animator.Play(stateTarget);
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
