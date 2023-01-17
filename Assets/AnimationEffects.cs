using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEffects : MonoBehaviour
{
    //Will need to get current animation clip to know what kind of effects we require
    Animator animator;
    AnimatorStateInfo stateInfo;
    AnimatorClipInfo clipInfo;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(animator != null)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            clipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];
        }
    }

    public void RequestEffect()
    {
        Debug.Log("Effect Event Has Fired");
    }
}
