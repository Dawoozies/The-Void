using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollImpact_FX : StateMachineBehaviour
{
    AnimationEffects animationEffectsSystem;
    SpriteRenderer spriteRenderer;
    public LayerMask layerMask;
    bool effectPlayed;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        effectPlayed = false;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animationEffectsSystem == null)
            animationEffectsSystem = animator.GetComponentInParent<AnimationEffects>();

        if (spriteRenderer == null)
            spriteRenderer = animator.GetComponent<SpriteRenderer>();

        Vector3 worldPositionToPlayAt = Physics2D.Raycast(animator.transform.position, Vector2.down, spriteRenderer.size.y / 2, layerMask).point;

        if (!effectPlayed)
            effectPlayed = animationEffectsSystem.PlayEffectAtPosition(animator.GetCurrentAnimatorClipInfo(0)[0].clip, stateInfo, worldPositionToPlayAt, animator.transform.localScale, true);
    }
}