using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewAnimationEffect", menuName = "Effects/AnimationEffect")]
public class AnimationEffect : ScriptableObject
{
    public AnimationClip clip;
    public List<EffectList> effectsList;
    //When making a new animation effect the Clip we are making effects for and its frameCount must be specified
    public AnimationEffect(AnimationClip animClip, int frameCount)
    {
        clip = animClip;
        effectsList = new List<EffectList>(frameCount);
        Debug.Log(frameCount);
        for (int i = 0; i < effectsList.Count; i++)
        {
            effectsList[i] = new EffectList();
        }
    }

    public void AddEffectToFrame(int frame, AnimationClip effectClip, Vector2 effectPosition)
    {
        if(effectsList == null || effectsList.Count <= 0)
        {
            Debug.LogError("AnimationEffect must be initialised before adding effects!");
            return;
        }

        effectsList[frame].effects.Add(new EffectData(effectClip, effectPosition));
    }
}
