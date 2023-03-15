using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HurtboxOverlap
{
    //Clip data
    public AnimationClip clip; 
    
    //State data
    public float stateLength;
    public float stateNormalizedTime;
    public float stateSpeed;

    //Collider data
    public Collider2D collider;

    public void SetStateData(AnimatorStateInfo stateInfo)
    {
        stateLength = stateInfo.length;
        stateNormalizedTime = stateInfo.normalizedTime;
        stateSpeed = stateInfo.speed;
    }
}
