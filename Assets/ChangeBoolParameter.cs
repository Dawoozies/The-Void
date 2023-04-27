using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBoolParameter : StateMachineBehaviour
{
    public string parameterName;
    public bool desiredValue;
    public bool onEnter;
    public bool onUpdate;
    public bool onExit;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!onEnter)
            return;
        if (parameterName == null || parameterName.Length <= 0)
        {
            Debug.LogError("ChangeBoolParameter Error: Parameter Name variable not defined");
            return;
        }
        animator.SetBool(parameterName, desiredValue);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!onUpdate)
            return;
        if (parameterName == null || parameterName.Length <= 0)
        {
            Debug.LogError("ChangeBoolParameter Error: Parameter Name variable not defined");
            return;
        }
        animator.SetBool(parameterName, desiredValue);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!onExit)
            return;
        if (parameterName == null || parameterName.Length <= 0)
        {
            Debug.LogError("ChangeBoolParameter Error: Parameter Name variable not defined");
            return;
        }
        animator.SetBool(parameterName, desiredValue);
    }

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
