using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameVelocityData : ScriptableObject
{
    public AnimationClip clip;
    public List<VelocityData> velocityDataList;
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
    public void CopyFromFrameToAll(int frame)
    {
        for (int i = 0; i < velocityDataList.Count; i++)
        {
            if (i == frame)
                continue;
            velocityDataList[i].PasteVelocityData(velocityDataList[frame]);
        }
    }
    public void PasteFrameVelocityData(FrameVelocityData newFrameVelocityData)
    {
        velocityDataList = new List<VelocityData>();
        for (int i = 0; i < newFrameVelocityData.velocityDataList.Count; i++)
        {
            VelocityData velocityData = new VelocityData();
            velocityData.PasteVelocityData(newFrameVelocityData.velocityDataList[i]);
            velocityDataList.Add(velocityData);
        }
    }
}
[System.Serializable]
public class VelocityData
{
    public List<VelocityComponent> velocityComponents;
    public void PasteVelocityData(VelocityData newVelocityData)
    {
        velocityComponents = new List<VelocityComponent>();
        for (int i = 0; i < newVelocityData.velocityComponents.Count; i++)
        {
            VelocityComponent newComponent = new VelocityComponent();
            newComponent.velocityBase = newVelocityData.velocityComponents[i].velocityBase;
            newComponent.maxSpeed = newVelocityData.velocityComponents[i].maxSpeed;
            newComponent.multiplier = newVelocityData.velocityComponents[i].multiplier;
            newComponent.isImpulse = newVelocityData.velocityComponents[i].isImpulse;
            newComponent.isGravitational = newVelocityData.velocityComponents[i].isGravitational;
            newComponent.isConstant = newVelocityData.velocityComponents[i].isConstant;
            newComponent.useLocalSpace = newVelocityData.velocityComponents[i].useLocalSpace;
            newComponent.useLStick = newVelocityData.velocityComponents[i].useLStick;
            newComponent.useRStick = newVelocityData.velocityComponents[i].useRStick;
            newComponent.useGravity = newVelocityData.velocityComponents[i].useGravity;
            newComponent.useTransformUp = newVelocityData.velocityComponents[i].useTransformUp;
            newComponent.useTransformRight = newVelocityData.velocityComponents[i].useTransformRight;
            newComponent.useTransformForward = newVelocityData.velocityComponents[i].useTransformForward;
            newComponent.useVelocityDirection = newVelocityData.velocityComponents[i].useVelocityDirection;
            newComponent.useVelocity = newVelocityData.velocityComponents[i].useVelocity;
            newComponent.parameterMultipliers = newVelocityData.velocityComponents[i].parameterMultipliers;
            velocityComponents.Add(newComponent);
        }
    }
}
[System.Serializable]
public class VelocityComponent
{
    public Vector3 velocityBase;
    public float maxSpeed; //In ANY direction
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
    public bool useVelocity;
    public List<string> parameterMultipliers;
    public VelocityComponent()
    {
        velocityBase = Vector3.zero;
        maxSpeed = 40f;
        multiplier = 1f;
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
        useVelocity = false;
        parameterMultipliers = new List<string>();
    }
}