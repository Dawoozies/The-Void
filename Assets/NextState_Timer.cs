using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextState_Timer : StateMachineBehaviour
{
    public string statePrefix;
    public float stateTime;
    float time;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        time = 0f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (time > stateTime)
            animator.SetTrigger($"NextState_{statePrefix}");

        time += Time.deltaTime;
    }
}
