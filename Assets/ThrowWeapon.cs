using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods_Animator;
public class ThrowWeapon : StateMachineBehaviour
{
    public int frameToThrow;
    public Vector3 launchPosition;
    public Vector3 launchVelocity;
    int lastFrame;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        lastFrame = 0;   
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (lastFrame != animator.CurrentFrame())
        {
            lastFrame = animator.CurrentFrame();
            Debug.Log(animator.CurrentFrame());
            if (animator.CurrentFrame() == frameToThrow)
            {
                Debug.Log("We should throw the weapon now");
                Vector3 finalPosition = animator.transform.position + new Vector3(launchPosition.x*animator.transform.localScale.x, launchPosition.y, launchPosition.z);
                Vector3 finalVelocity = new Vector3(launchVelocity.x * animator.transform.localScale.x, launchVelocity.y, launchVelocity.z);
                ThrownWeaponTest.ins.LaunchHalberd(finalPosition, finalVelocity);
                PlayerDataManager.ins.ThrownHalberd = true;
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
