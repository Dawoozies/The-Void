using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtensionMethods_Animator
{
    public static class Extensions
    {
        public static int CurrentFrame(this Animator a)
        {
            AnimatorStateInfo stateInfo = a.GetCurrentAnimatorStateInfo(0);
            AnimatorClipInfo clipInfo = a.GetCurrentAnimatorClipInfo(0)[0];

            int totalFrames = Mathf.RoundToInt(clipInfo.clip.length);
            float clipTime = stateInfo.normalizedTime - Mathf.FloorToInt(stateInfo.normalizedTime);
            int frame = Mathf.FloorToInt(totalFrames * clipTime);

            if (!clipInfo.clip.isLooping)
                if (stateInfo.normalizedTime > 1)
                    frame = totalFrames - 1;

            return frame;
        }

        public static int TotalFrames(this Animator a)
        {
            AnimatorStateInfo stateInfo = a.GetCurrentAnimatorStateInfo(0);
            AnimatorClipInfo clipInfo = a.GetCurrentAnimatorClipInfo(0)[0];

            int totalFrames = Mathf.RoundToInt(clipInfo.clip.length);

            return totalFrames;
        }
    }
}
