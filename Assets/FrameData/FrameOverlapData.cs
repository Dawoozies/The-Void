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
    public Color circleColor;
    public Color radiusColor;
    public List<Geometry.Circle> circles;
    private DefinedLayerMask _definedLayerMask;
    public int collisionLayer;
    public List<ParameterComponent> parameterComponents; //Parameters that get changed when overlap happens
    public OverlapComponent ()
    {
        componentName = "";
        circleColor = Color.white;
        radiusColor = Color.white;
        circles = new List<Geometry.Circle>();
        _definedLayerMask = 0;
        parameterComponents = new List<ParameterComponent>();
    }
    public LayerMask targetLayerMask
    {
        get => UpdateTargetLayerMask();
    }
    public DefinedLayerMask definedLayerMask
    {
        get => _definedLayerMask;
        set
        {
            _definedLayerMask = value;
            UpdateTargetLayerMask();
        }
    }
    LayerMask UpdateTargetLayerMask()
    {
        List<string> layerMask = new List<string>();
        if ((_definedLayerMask & DefinedLayerMask.Default) == DefinedLayerMask.Default)
            layerMask.Add(DefinedLayerMask.Default.ToString());
        if ((_definedLayerMask & DefinedLayerMask.Player) == DefinedLayerMask.Player)
            layerMask.Add(DefinedLayerMask.Player.ToString());
        if ((_definedLayerMask & DefinedLayerMask.Ground) == DefinedLayerMask.Ground)
            layerMask.Add(DefinedLayerMask.Ground.ToString());
        if ((_definedLayerMask & DefinedLayerMask.Enemy) == DefinedLayerMask.Enemy)
            layerMask.Add(DefinedLayerMask.Enemy.ToString());
        if ((_definedLayerMask & DefinedLayerMask.Projectile) == DefinedLayerMask.Projectile)
            layerMask.Add(DefinedLayerMask.Projectile.ToString());
        if ((_definedLayerMask & DefinedLayerMask.Halberd) == DefinedLayerMask.Halberd)
            layerMask.Add(DefinedLayerMask.Halberd.ToString());
        if ((_definedLayerMask & DefinedLayerMask.Hammer) == DefinedLayerMask.Hammer)
            layerMask.Add(DefinedLayerMask.Hammer.ToString());
        return LayerMask.GetMask(layerMask.ToArray());
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
[Flags, Serializable]
public enum DefinedLayerMask
{
    None = 0, //This will prevent the overlap from doing anything
    Default = 1,
    Player = 2,
    Ground = 4,
    Enemy = 8,
    Projectile = 16,
    Halberd = 32,
    Hammer = 64,
    //layer8 = 128,
    //layer9 = 256,
    //layer10 = 512,
    //layer11 = 1024,
}