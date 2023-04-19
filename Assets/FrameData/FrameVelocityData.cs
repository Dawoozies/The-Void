using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameVelocityData : ScriptableObject
{
    public AnimationClip clip;
    public List<Vector3> dataList;
    public List<float> dataListSecondary;
    public List<Vector3> dataListLeftStick;
    public List<Vector3> dataListRightStick;
    public bool velocityAdditive;
    public bool leftStickVelocityAdditive;
    public bool rightStickVelocityAdditive;
    public Vector3 VelocityAtFrame(int frame)
    {
        return dataList[frame];
    }
    public float MagnitudeAtFrame(int frame)
    {
        return dataList[frame].z;
    }
    public float DragAtFrame(int frame)
    {
        return dataListSecondary[frame];
    }
    public Vector3 LeftStickVelocityAtFrame(int frame)
    {
        return dataListLeftStick[frame];
    }
    public Vector3 RightStickVelocityAtFrame(int frame)
    {
        return dataListRightStick[frame];
    }
}