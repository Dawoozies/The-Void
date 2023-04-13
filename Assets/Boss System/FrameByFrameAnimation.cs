using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods_Animator;
public class FrameByFrameAnimation : MonoBehaviour
{
    Animator animator;
    AnimationClip currentClip;
    AnimatorStateInfo currentStateInfo;
    public int totalFrames;
    public float time;
    public float frameRate;
    public bool overrideStateSpeed;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator.GetBool("STATETYPE:NoFrameByFrame"))
            return;

        if(currentClip == null || currentClip.name != animator.GetCurrentAnimatorClipInfo(0)[0].clip.name)
        {
            currentClip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
            totalFrames = animator.TotalFrames();
        }

        if(currentStateInfo.fullPathHash != animator.GetCurrentAnimatorStateInfo(0).fullPathHash)
        {
            currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }

        if(overrideStateSpeed)
        {
            //This should only be used while actually making the animations and getting the timing right
            time += Time.deltaTime * frameRate;
        }
        else
        {
            time += Time.deltaTime * currentStateInfo.speed;
        }

        float normalizedTime = (float)Mathf.FloorToInt(time) / totalFrames;
        animator.Play(currentStateInfo.fullPathHash, -1, normalizedTime);

        if (normalizedTime > 1)
            time = 0;
    }
}
