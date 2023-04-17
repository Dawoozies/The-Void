using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LinearAlgebra;
public class StateTransform_MoveToPlayer : StateMachineBehaviour
{
    public Vector3 pathEndOffset;
    public Vector2 yBoundary; //Min then Max
    public Vector3 lastPlayerPosition;
    Transform playerTransform;
    public float distanceThreshold;
    public float speed;
    public string stateTarget;
    ParametrisedLine parametrisedLine = new ParametrisedLine();
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerTransform == null)
            playerTransform = GameObject.Find("Player").transform;

        lastPlayerPosition = playerTransform.position;

        lastPlayerPosition.y = Mathf.Clamp(lastPlayerPosition.y, yBoundary.x, yBoundary.y);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        parametrisedLine.pathStart = animator.transform.position;
        parametrisedLine.pathEnd = lastPlayerPosition + pathEndOffset;
        float distance = Vector3.Distance(parametrisedLine.pathStart, parametrisedLine.pathEnd);
        Vector3 translation = parametrisedLine.direction*Time.deltaTime*speed;
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
