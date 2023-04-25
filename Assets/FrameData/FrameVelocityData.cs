using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameVelocityData : ScriptableObject
{
    public AnimationClip clip;
    public List<VelocityData> velocityDataList;
    public List<Vector3> dataList;
    public List<float> dataListSecondary;
    public List<Vector3> dataListLeftStick;
    public List<Vector3> dataListRightStick;
    public List<bool> dataListVelocityAdditive;
    public List<bool> dataListLeftStickVelocityAdditive;
    public List<bool> dataListRightStickVelocityAdditive;
    public void InitializeFrameData(int totalFrames)
    {
        List<VelocityData> initVelocityDataList = new List<VelocityData>();
        for (int i = 0; i < totalFrames; i++)
        {
            VelocityData velocityData = new VelocityData();
            velocityData.velocityComponents = new List<VelocityComponent>();
            initVelocityDataList.Add(velocityData);
        }
        velocityDataList = initVelocityDataList;
    }
    public List<VelocityComponent> VelocityComponentsAtFrame(int frame)
    {
        return velocityDataList[frame].velocityComponents;
    }
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
    public bool IsVelocityAdditiveAtFrame(int frame)
    {
        return dataListVelocityAdditive[frame];
    }
    public bool IsLeftStickVelocityAdditiveAtFrame(int frame)
    {
        return dataListLeftStickVelocityAdditive[frame];
    }
    public bool IsRightStickVelocityAdditiveAtFrame(int frame)
    {
        return dataListRightStickVelocityAdditive[frame];
    }
}
[System.Serializable]
public class VelocityData
{
    public List<VelocityComponent> velocityComponents;
}
[System.Serializable]
public class VelocityComponent
{
    public Vector3 velocityBase;
    public Vector3 threshold;
    public float multiplier;
    public bool isImpulse; //Application instant, no frame repeat
    public bool isGravitational; //Application with timestepped, frame repeat ok
    public bool isConstant; //Application until threshold, frame repeat ok
    public bool useLocalSpace;
    public bool useLStick;
    public bool useRStick;
    public bool useGravity;
    public bool useTransformUp;
    public bool useTransformRight;
    public bool useTransformForward;
    public bool useVelocityDirection;

    public VelocityComponent()
    {
        velocityBase = Vector3.zero;
        threshold = Vector3.zero;
        multiplier = 0f;
        isImpulse = false;
        isGravitational = false;
        isConstant = false;
        useLocalSpace = false;
        useLStick = false;
        useRStick = false;
        useGravity = false;
        useTransformUp = false;
        useTransformRight = false;
        useTransformForward = false;
        useVelocityDirection = false;
    }
}