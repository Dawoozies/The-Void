using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallLeapDirection : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.transform.localScale.x > 0)
        {
            animator.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            animator.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
