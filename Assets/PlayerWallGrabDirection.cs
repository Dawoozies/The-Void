using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallGrabDirection : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("We got to OnStateEnter at time " + Time.time);
        animator.transform.localScale = new Vector3
            (
                - animator.transform.localScale.x * animator.GetInteger("WallGrabDirection"),
                animator.transform.localScale.y,
                animator.transform.localScale.z
            );
    }
}
