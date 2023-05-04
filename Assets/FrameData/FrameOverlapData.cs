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
    public void DuplicatePreviousFrame(int frame)
    {
        if (frame <= 0)
            return;

        overlapDataList[frame].PasteOverlapComponents(overlapDataList[frame - 1].overlapComponents);
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
    public void PasteOverlapComponents(List<OverlapComponent> copiedOverlapComponents)
    {
        overlapComponents = new List<OverlapComponent>();
        for (int i = 0; i < copiedOverlapComponents.Count; i++)
        {
            OverlapComponent newOverlapComponent = new OverlapComponent();
            newOverlapComponent.PasteOverlapComponent(copiedOverlapComponents[i]);
            overlapComponents.Add(newOverlapComponent);
        }
    }
}
[Serializable]
public class OverlapComponent
{
    public string componentName; //Just for ease of remembering what the overlap component is for
    public Color circleColor;
    public Color radiusColor;
    public List<Geometry.Circle> circles;
    public DefinedLayerMask definedLayerMask;
    public int collisionLayer;
    public OverlapComponentType overlapComponentType;
    public AxisTransform axisTransform;
    public List<ParameterComponent> parameterComponents; //Parameters that get changed when overlap happens
    public Vector3 GetCircleWorldPosition(Transform parentObject, Geometry.Circle circle)
    {
        Vector3 circleWorldPosition = Vector3.zero;
        if (axisTransform == AxisTransform.LocalScale)
            circleWorldPosition = parentObject.position + Vector3.Scale(circle.center, parentObject.localScale);
        if (axisTransform == AxisTransform.TransformDirection)
            circleWorldPosition = parentObject.position
                + circle.center.x * parentObject.localScale.x * parentObject.right
                + circle.center.y * parentObject.localScale.y * parentObject.up
                + circle.center.z * parentObject.localScale.z * parentObject.forward;
        //circleWorldPosition = new Vector3
        //    (
        //    parentObject.position.x + circle.center.x * parentObject.localScale.x,
        //    parentObject.position.y + circle.center.y * parentObject.localScale.y,
        //    parentObject.position.z + circle.center.z * parentObject.localScale.z
        //    );
        return circleWorldPosition;
    }
    public List<Collider2D> CastCircles(Transform parentObject)
    {
        if (circles == null || circles.Count == 0)
            return null; //For things like empty groundboxes
        List<Collider2D> result = new List<Collider2D>();
        for (int i = 0; i < circles.Count; i++)
        {
            result.AddRange(Physics2D.OverlapCircleAll(GetCircleWorldPosition(parentObject, circles[i]), circles[i].radius, targetLayerMask));
        }
        return result;
    }
    public void PasteOverlapComponent(OverlapComponent copiedOverlapComponent)
    {
        componentName = copiedOverlapComponent.componentName;
        circleColor = copiedOverlapComponent.circleColor;
        radiusColor = copiedOverlapComponent.radiusColor;
        circles = new List<Geometry.Circle>();
        for (int i = 0; i < copiedOverlapComponent.circles.Count; i++)
        {
            circles.Add(copiedOverlapComponent.circles[i].CopyCircle());
        }
        definedLayerMask = copiedOverlapComponent.definedLayerMask;
        collisionLayer = copiedOverlapComponent.collisionLayer;
        axisTransform = copiedOverlapComponent.axisTransform;
        parameterComponents = new List<ParameterComponent>();
        for (int i = 0; i < copiedOverlapComponent.parameterComponents.Count; i++)
        {
            parameterComponents.Add(copiedOverlapComponent.parameterComponents[i].CopyParameterComponent());
        }
        overlapComponentType = copiedOverlapComponent.overlapComponentType;
    }
    public OverlapComponent ()
    {
        componentName = "";
        circleColor = Color.white;
        radiusColor = Color.white;
        circles = new List<Geometry.Circle>();
        definedLayerMask = 0;
        overlapComponentType = 0;
        axisTransform = 0;
        parameterComponents = new List<ParameterComponent>();
    }
    public LayerMask targetLayerMask
    {
        get => UpdateTargetLayerMask();
    }
    LayerMask UpdateTargetLayerMask()
    {
        List<string> layerMask = new List<string>();
        if ((definedLayerMask & DefinedLayerMask.Default) == DefinedLayerMask.Default)
            layerMask.Add(DefinedLayerMask.Default.ToString());
        if ((definedLayerMask & DefinedLayerMask.Player) == DefinedLayerMask.Player)
            layerMask.Add(DefinedLayerMask.Player.ToString());
        if ((definedLayerMask & DefinedLayerMask.Ground) == DefinedLayerMask.Ground)
            layerMask.Add(DefinedLayerMask.Ground.ToString());
        if ((definedLayerMask & DefinedLayerMask.Enemy) == DefinedLayerMask.Enemy)
            layerMask.Add(DefinedLayerMask.Enemy.ToString());
        if ((definedLayerMask & DefinedLayerMask.Projectile) == DefinedLayerMask.Projectile)
            layerMask.Add(DefinedLayerMask.Projectile.ToString());
        if ((definedLayerMask & DefinedLayerMask.Halberd) == DefinedLayerMask.Halberd)
            layerMask.Add(DefinedLayerMask.Halberd.ToString());
        if ((definedLayerMask & DefinedLayerMask.Hammer) == DefinedLayerMask.Hammer)
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
    private Action<Animator, List<Collider2D>> componentAction;
    public ParameterComponentOptions componentOptions;
    public ParameterComponent CopyParameterComponent()
    {
        ParameterComponent copyParameterComponent = new ParameterComponent();
        copyParameterComponent.parameterName = parameterName;
        copyParameterComponent.parameterType = parameterType;
        copyParameterComponent.floatValue = floatValue;
        copyParameterComponent.integerValue = integerValue;
        copyParameterComponent.boolValue = boolValue;
        copyParameterComponent.componentOptions = componentOptions;
        return copyParameterComponent;
    }
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
    public void InvokeComponentAction (Animator animator, List<Collider2D> result)
    {
        if ((componentOptions & ParameterComponentOptions.Overwrite) == ParameterComponentOptions.Overwrite)
            componentAction += OverwriteValue;
        if ((componentOptions & ParameterComponentOptions.Add) == ParameterComponentOptions.Add)
            componentAction += AddValue;
        if ((componentOptions & ParameterComponentOptions.Multiply) == ParameterComponentOptions.Multiply)
            componentAction += MultiplyValue;

        componentAction?.Invoke(animator, result);
        componentAction = null;
    }
    public void OverwriteValue(Animator animator, List<Collider2D> result)
    {
        if (parameterType == AnimatorControllerParameterType.Float)
            animator.SetFloat(parameterName, (result == null || result.Count == 0) ? animator.GetFloat(parameterName) : floatValue);
        if (parameterType == AnimatorControllerParameterType.Int)
        {
            animator.SetInteger(parameterName, (result == null || result.Count == 0) ? animator.GetInteger(parameterName) : integerValue);
            if (result != null && result.Count > 0)
                Debug.Log("Overwriting WallGrabDirection at time " + Time.time);
        }
        if (parameterType == AnimatorControllerParameterType.Bool)
            animator.SetBool(parameterName, (result == null || result.Count == 0) ? !boolValue : boolValue);
    }
    public void AddValue(Animator animator, List<Collider2D> result)
    {
        if (parameterType == AnimatorControllerParameterType.Float)
            animator.SetFloat(parameterName, (result == null || result.Count == 0) ? animator.GetFloat(parameterName) : (animator.GetFloat(parameterName) + floatValue));
        if (parameterType == AnimatorControllerParameterType.Int)
            animator.SetInteger(parameterName, (result == null || result.Count == 0) ? animator.GetInteger(parameterName) : (animator.GetInteger(parameterName) + integerValue));
        if (parameterType == AnimatorControllerParameterType.Bool)
            animator.SetBool(parameterName, (result == null || result.Count == 0) ? animator.GetBool(parameterName) : (animator.GetBool(parameterName) | boolValue));
    }
    public void MultiplyValue(Animator animator, List<Collider2D> result)
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
public enum OverlapComponentType
{
    None = 0,
    Cast = 1,
    Collider = 2,
    Trigger = 4,
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
[Serializable]
public enum AxisTransform
{
    LocalScale = 0,
    TransformDirection = 1,
}