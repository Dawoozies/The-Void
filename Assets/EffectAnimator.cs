using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAnimator : MonoBehaviour, AnimationEffectInterface
{
    Animator animator;
    SpriteRenderer spriteRenderer;
    public void InitialiseEffectAnimator()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.color = Color.clear;
    }
    void Update()
    {
        //Then this effect sprite is visible
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Null_FX") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            //Then we just played an animation and it is now over so we should make this sprite transparent again
            animator.Play("Null_FX");
        }
    }
    public bool TryPlayEffect(EffectData effectData, Vector3 effectOrigin, Vector3 entityLocalScale, bool copyEntityDirection)
    {
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Null_FX") || animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            spriteRenderer.color = Color.white;
            animator.Play(effectData.clip.name, 0, 0f);
            if(copyEntityDirection)
            {
                transform.localScale = entityLocalScale;
            }
            else
            {
                transform.localScale = Vector3.one;
            }

            transform.position = effectOrigin + new Vector3(transform.localScale.x * effectData.position.x, transform.localScale.y * effectData.position.y, 0f);
            return true;
        }

        return false;
    }

    public bool TryPlayEffectAtPosition(EffectData effectData, Vector3 worldPositionToPlayAt, Vector3 entityLocalScale, bool copyEntityDirection)
    {
        //We really only want to use this when the position of the animation is determined only by external checks
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Null_FX") || animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            spriteRenderer.color = Color.white;
            animator.Play(effectData.clip.name, 0, 0f);

            if(copyEntityDirection)
            {
                transform.localScale = entityLocalScale;
            }
            else
            {
                transform.localScale = Vector3.one;
            }

            transform.position = worldPositionToPlayAt;
            return true;
        }

        return false;
    }
}
