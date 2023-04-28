using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Geometry;
public class FrameOverlapData : ScriptableObject
{
    public AnimationClip clip;
    public List<OverlapData> overlapDataList;
    public void InitializeFrameData(int totalFrames)
    {
        overlapDataList = new List<OverlapData>();
        for (int i = 0; i < totalFrames; i++)
        {
            overlapDataList.Add(new OverlapData());
        }
    }
    public List<OverlapComponent> OverlapComponentsAtFrame(int frame)
    {
        return overlapDataList[frame].overlapComponents;
    }
}
[Serializable]
public class OverlapData
{
    public List<OverlapComponent> overlapComponents;
    public OverlapData ()
    {
        overlapComponents = new List<OverlapComponent>();
    }
    public List<ParameterComponent> ParameterComponentsAtIndex(int index)
    {
        return overlapComponents[index].parameterComponents;
    }
}
[Serializable]
public class OverlapComponent
{
    public string componentName; //Just for ease of remembering what the overlap component is for
    public List<Geometry.Circle> circles;
    public LayerMask targetLayerMask;
    public List<ParameterComponent> parameterComponents; //Parameters that get changed when overlap happens
    public OverlapComponent ()
    {
        componentName = "";
        circles = new List<Geometry.Circle>();
        targetLayerMask = 0;
        parameterComponents = new List<ParameterComponent>();
    }
}
[Serializable]
public class ParameterComponent
{
    public string parameterName;
    public AnimatorControllerParameterType parameterType;
    public float floatValue;
    public int integerValue;
    public bool boolValue;
    private Action<Animator> componentAction;
    public ParameterComponentOptions componentOptions;
    public ParameterComponent ()
    {
        parameterName = "";
        parameterType = AnimatorControllerParameterType.Bool;
        floatValue = 0f;
        integerValue = 0;
        boolValue = false;
        componentAction = null;
        componentOptions = 0;
    }
    public void InvokeComponentAction (Animator animator)
    {
        if ((componentOptions & ParameterComponentOptions.Overwrite) == ParameterComponentOptions.Overwrite)
            componentAction += OverwriteValue;
        if ((componentOptions & ParameterComponentOptions.Add) == ParameterComponentOptions.Add)
            componentAction += AddValue;
        if ((componentOptions & ParameterComponentOptions.Multiply) == ParameterComponentOptions.Multiply)
            componentAction += MultiplyValue;

        componentAction?.Invoke(animator);
        componentAction = null;
    }
    public void OverwriteValue(Animator animator)
    {
        if (parameterType == AnimatorControllerParameterType.Float)
            animator.SetFloat(parameterName, floatValue);
        if (parameterType == AnimatorControllerParameterType.Int)
            animator.SetInteger(parameterName, integerValue);
        if (parameterType == AnimatorControllerParameterType.Bool)
            animator.SetBool(parameterName, boolValue);
    }
    public void AddValue(Animator animator)
    {
        if (parameterType == AnimatorControllerParameterType.Float)
            animator.SetFloat(parameterName, animator.GetFloat(parameterName) + floatValue);
        if (parameterType == AnimatorControllerParameterType.Int)
            animator.SetInteger(parameterName, animator.GetInteger(parameterName) + integerValue);
        if (parameterType == AnimatorControllerParameterType.Bool)
            animator.SetBool(parameterName, animator.GetBool(parameterName) | boolValue);
    }
    public void MultiplyValue(Animator animator)
    {
        if (parameterType == AnimatorControllerParameterType.Float)
            animator.SetFloat(parameterName, animator.GetFloat(parameterName) * floatValue);
        if (parameterType == AnimatorControllerParameterType.Int)
            animator.SetInteger(parameterName, animator.GetInteger(parameterName) * integerValue);
        if (parameterType == AnimatorControllerParameterType.Bool)
            animator.SetBool(parameterName, animator.GetBool(parameterName) & boolValue);
    }
}
[Flags, Serializable]
public enum ParameterComponentOptions
{
    None = 0,
    Overwrite = 1,
    Add = 2,
    Multiply = 4,
}