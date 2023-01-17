using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEffect
{
    AnimationClip clip;
    List<List<(AnimationClip, Vector2)>> effects;
    //When making a new animation effect the Clip we are making effects for and its frameCount must be specified
    public AnimationEffect(AnimationClip animClip, int frameCount)
    {
        clip = animClip;
        effects = new List<List<(AnimationClip, Vector2)>>(frameCount);
    }

    public void AddEffectToFrame(int frame, AnimationClip effectClip, Vector2 effectPosition)
    {
        if(effects == null || effects.Count <= 0)
        {
            Debug.LogError("AnimationEffect must be initialised before adding effects!");
            return;
        }

        if (effects[frame] == null || effects[frame].Count <= 0)
        {
            //Then initialise the list
            
        }
    }
}
