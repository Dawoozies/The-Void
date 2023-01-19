using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAnimator : MonoBehaviour, AnimationEffectInterface
{
    Animator animator;

    public void InitialiseEffectAnimator()
    {
        animator = GetComponent<Animator>();
    }

    public bool TryPlayEffect(EffectData effectData, Vector3 effectOrigin)
    {
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Null_FX") || animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            animator.Play(effectData.clip.name, 0, 0f);
            transform.position = effectOrigin + (Vector3)effectData.position;
            return true;
        }

        return false;
    }
}
