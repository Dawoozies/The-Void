using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LinearAlgebra;
public class HalberdRecall : StateMachineBehaviour
{
    ParametrisedLine recallPath = new ParametrisedLine();
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        recallPath.pathStart = animator.transform.position;
        recallPath.pathEnd = PlayerDataManager.ins.RecallPosition;
        animator.SetFloat("DistanceToRecallPoint", Vector3.Distance(recallPath.pathStart, recallPath.pathEnd));
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        recallPath.pathStart = animator.transform.position;
        recallPath.pathEnd = PlayerDataManager.ins.RecallPosition;
        if(animator.GetFloat("DistanceToRecallPoint") > 1f)
        {
            animator.SetFloat("DistanceToRecallPoint", Vector3.Distance(recallPath.pathStart, recallPath.pathEnd));
            animator.transform.up = -recallPath.direction.normalized;
        }
        else
        {
            animator.SetFloat("DistanceToRecallPoint", 0f);
        }
    }
}
