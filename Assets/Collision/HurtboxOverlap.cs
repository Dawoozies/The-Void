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

    //Hurtbox data (Data about the hurtbox which overlapped)
    public Vector3 hurtboxWorldCenter;
    public float hurtboxRadius;

    public void SetStateData(AnimatorStateInfo stateInfo)
    {
        stateLength = stateInfo.length;
        stateNormalizedTime = stateInfo.normalizedTime;
        stateSpeed = stateInfo.speed;
    }

    public void SetHurtboxData(Vector3 circlePos, float circleRadius)
    {
        hurtboxWorldCenter = circlePos;
        hurtboxRadius = circleRadius;
    }
}
