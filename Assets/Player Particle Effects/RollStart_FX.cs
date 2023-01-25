using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollStart_FX : StateMachineBehaviour
{
    AnimationEffects animationEffectsSystem;
    bool effectPlayed;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        effectPlayed = false;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animationEffectsSystem == null)
            animationEffectsSystem = animator.GetComponentInParent<AnimationEffects>();

        if(!effectPlayed)
            effectPlayed = animationEffectsSystem.PlayEffect(animator.GetCurrentAnimatorClipInfo(0)[0].clip, stateInfo, animator.transform.localScale, true);
    }
}
