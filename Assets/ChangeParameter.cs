using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeParameter : StateMachineBehaviour
{
    public string parameterName;
    public float desiredFloatValue;
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
            Debug.LogError("ChangeParameter Error: Parameter Name variable not defined");
            return;
        }
        animator.SetFloat(parameterName, desiredFloatValue);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!onUpdate)
            return;
        if (parameterName == null || parameterName.Length <= 0)
        {
            Debug.LogError("ChangeParameter Error: Parameter Name variable not defined");
            return;
        }
        animator.SetFloat(parameterName, desiredFloatValue);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!onExit)
            return;
        if (parameterName == null || parameterName.Length <= 0)
        {
            Debug.LogError("ChangeParameter Error: Parameter Name variable not defined");
            return;
        }
        animator.SetFloat(parameterName, desiredFloatValue);
    }
}
