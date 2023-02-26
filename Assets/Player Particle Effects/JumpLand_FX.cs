using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpLand_FX : StateMachineBehaviour
{
    AnimationEffects animationEffectsSystem;
    SpriteRenderer spriteRenderer;
    public LayerMask layerMask;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animationEffectsSystem = animator.GetComponentInParent<AnimationEffects>();
        if (animationEffectsSystem == null)
        {
            Debug.LogError(GetType().ToString() + " Error: Effects behaviour has no AnimationEffects on parent!");
            return;
        }

        if (spriteRenderer == null)
            spriteRenderer = animator.GetComponent<SpriteRenderer>();

        Vector3 worldPositionToPlayAt = Physics2D.Raycast(animator.transform.position, Vector2.down, spriteRenderer.size.y / 2, layerMask).point;
        //Debug.Log(Physics2D.Raycast(animator.transform.position, Vector2.down, spriteRenderer.size.y / 2, layerMask).collider);
        animationEffectsSystem.PlayEffectAtPosition(animator.GetCurrentAnimatorClipInfo(0)[0].clip, stateInfo, worldPositionToPlayAt, animator.transform.localScale, true);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
