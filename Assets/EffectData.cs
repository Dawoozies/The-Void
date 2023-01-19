using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class EffectList
{
    public List<EffectData> effects;

    public EffectList ()
    {
        effects = new List<EffectData>();
    }
}

[System.Serializable]
public class EffectData
{
    public AnimationClip clip;
    public Vector2 position;
    public EffectData(AnimationClip clip)
    {
        this.clip = clip;
        position = Vector2.zero;
    }

    public EffectData(AnimationClip clip, Vector2 position)
    {
        this.clip = clip;
        this.position = position;
    }
}