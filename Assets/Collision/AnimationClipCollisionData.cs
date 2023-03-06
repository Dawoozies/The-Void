using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAnimationClipCollisionData", menuName = "Collision System/AnimationClipCollisionData")]
public class AnimationClipCollisionData : ScriptableObject
{
    public AnimationClip animationClip;
    public List<FrameCollisionData> hitboxes;
    public List<FrameCollisionData> hurtboxes;
    public List<FrameCollisionData> groundboxes;

    public AnimationClipCollisionData()
    {
        this.animationClip = null;
        this.hitboxes = new List<FrameCollisionData>();
        this.hurtboxes = new List<FrameCollisionData>();
    }
}