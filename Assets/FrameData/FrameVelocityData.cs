using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameVelocityData : ScriptableObject
{
    public AnimationClip clip;
    public List<Vector3> dataList;
    public List<float> dataListSecondary;
    public Vector3 VelocityAtFrame(int frame)
    {
        return new Vector3(dataList[frame].x, dataList[frame].y, 0f);
    }
    public float MagnitudeAtFrame(int frame)
    {
        return dataList[frame].z;
    }
    public float DragAtFrame(int frame)
    {
        return dataListSecondary[frame];
    }
}