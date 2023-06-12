using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ControllerData : ScriptableObject
{
    public string controllerName;
    public DirectedCircleCollider[] directedCircleColliders;
    public DirectedCircleOverlap[] directedCircleOverlaps;
    public CircleSpriteMask[] circleSpriteMasks;
    public DirectedPoint[] directedPoints;
    public ControllerData()
    {
        controllerName = string.Empty;
        directedCircleColliders = new DirectedCircleCollider[] { };
        directedCircleOverlaps = new DirectedCircleOverlap[] { };
        circleSpriteMasks = new CircleSpriteMask[] { };
        directedPoints = new DirectedPoint[] { };
    }
    public DirectedCircleCollider[] GetAllDirectedCircleColliderData(string stateName)
    {
        List<DirectedCircleCollider> stateDatas = new();
        foreach (DirectedCircleCollider directedCircleCollider in directedCircleColliders)
        {
            if (directedCircleCollider.stateName == stateName)
                stateDatas.Add(directedCircleCollider);
        }
        return stateDatas.ToArray();
    }
    public DirectedCircleOverlap[] GetAllDirectedCircleOverlapData(string stateName)
    {
        List<DirectedCircleOverlap> stateDatas = new();
        foreach (DirectedCircleOverlap directedCircleOverlap in directedCircleOverlaps)
        {
            if (directedCircleOverlap.stateName == stateName)
                stateDatas.Add(directedCircleOverlap);
        }
        return stateDatas.ToArray();
    }
    public CircleSpriteMask[] GetAllCircleSpriteMaskData(string stateName)
    {
        List<CircleSpriteMask> stateDatas = new();
        foreach (CircleSpriteMask circleSpriteMask in circleSpriteMasks)
        {
            if(circleSpriteMask.stateName == stateName)
                stateDatas.Add(circleSpriteMask);
        }
        return stateDatas.ToArray();
    }
    public DirectedPoint[] GetAllDirectedPointData(string stateName)
    {
        List<DirectedPoint> stateDatas = new();
        foreach (DirectedPoint directedPoint in directedPoints)
        {
            if (directedPoint.stateName == stateName)
                stateDatas.Add(directedPoint);
        }
        return stateDatas.ToArray();
    }
}
[Serializable]
public class DirectedCircleCollider
{
    public string nickname;
    public string stateName;
    public List<int> assignedFrames;
    public Color color;
    public bool isTrigger;
    public int collisionLayer;
    public List<Vector2> centers;
    public List<float> radii;
    public List<Vector2> upDirections;
    public List<Vector2> rightDirections;
    public DirectedCircleCollider()
    {
        nickname = string.Empty;
        stateName = string.Empty;
        assignedFrames = new List<int>();
        color = Color.white;
        isTrigger = false;
        collisionLayer = 0;
        centers = new List<Vector2>();
        radii = new List<float>();
        upDirections = new List<Vector2>();
        rightDirections = new List<Vector2>();
    }
    public static DirectedCircleCollider CreateNew(string stateName)
    {
        DirectedCircleCollider directedCircleCollider = new()
        {
            nickname = "New Directed Circle Collider",
            stateName = stateName,
            assignedFrames = new List<int>(),
            isTrigger = false,
            collisionLayer = 0,
            centers = new List<Vector2>(),
            radii = new List<float>(),
            upDirections = new List<Vector2>(),
            rightDirections = new List<Vector2>()
        };
        return directedCircleCollider;
    }
    public static void AddNew(ControllerData controllerData, DirectedCircleCollider stateDataToAdd)
    {
        if(controllerData.directedCircleColliders == null || controllerData.directedCircleColliders.Length == 0)
        {
            controllerData.directedCircleColliders = new DirectedCircleCollider[] { stateDataToAdd };
            return;
        }
        DirectedCircleCollider[] newData = new DirectedCircleCollider[controllerData.directedCircleColliders.Length + 1];
        for (int i = 0; i < newData.Length; i++)
        {
            if(i == newData.Length - 1)
            {
                newData[i] = stateDataToAdd;
            }
            else
            {
                newData[i] = controllerData.directedCircleColliders[i];
            }
        }
        controllerData.directedCircleColliders = newData;
    }
    public static void RemoveAt(ControllerData controllerData, int index)
    {
        if(controllerData.directedCircleColliders == null || controllerData.directedCircleColliders.Length == 0)
        {
            return;
        }
        DirectedCircleCollider[] newData = new DirectedCircleCollider[controllerData.directedCircleColliders.Length - 1];
        for (int i = 0; i < controllerData.directedCircleColliders.Length; i++)
        {
            if(i < index)
            {
                newData[i] = controllerData.directedCircleColliders[i];
            }
            else if(i > index)
            {
                newData[i - 1] = controllerData.directedCircleColliders[i];
            }
        }
        controllerData.directedCircleColliders = newData;
    }
}
[Serializable]
public class DirectedCircleOverlap
{
    public string nickname;
    public string stateName;
    public List<int> assignedFrames;
    public Color color;
    public bool useNullResult;
    public LayerMask targetLayers;
    public float holdForNormalizedTime;
    public List<Vector2> centers;
    public List<float> radii;
    public List<Vector2> upDirections;
    public List<Vector2> rightDirections;
    public DirectedCircleOverlap()
    {
        nickname = string.Empty;
        stateName = string.Empty;
        assignedFrames = new List<int>();
        color = Color.white;
        useNullResult = false;
        targetLayers = 0;
        holdForNormalizedTime = 0;
        centers = new List<Vector2>();
        radii = new List<float>();
        upDirections = new List<Vector2>();
        rightDirections = new List<Vector2>();
    }
    public static DirectedCircleOverlap CreateNew(string stateName)
    {
        DirectedCircleOverlap directedCircleOverlap = new()
        {
            nickname = "New Directed Circle Overlap",
            stateName = stateName,
            assignedFrames = new List<int>(),
            useNullResult = false,
            targetLayers = 0,
            holdForNormalizedTime = 0f,
            centers = new List<Vector2>(),
            radii = new List<float>(),
            upDirections = new List<Vector2>(),
            rightDirections = new List<Vector2>()
        };
        return directedCircleOverlap;
    }
    public static void AddNew(ControllerData controllerData, DirectedCircleOverlap stateDataToAdd)
    {
        if(controllerData.directedCircleOverlaps == null || controllerData.directedCircleOverlaps.Length == 0)
        {
            controllerData.directedCircleOverlaps = new DirectedCircleOverlap[] { stateDataToAdd };
            return;
        }
        DirectedCircleOverlap[] newData = new DirectedCircleOverlap[controllerData.directedCircleOverlaps.Length + 1];
        for (int i = 0; i < newData.Length; i++)
        {
            if(i == newData.Length - 1)
            {
                newData[i] = stateDataToAdd;
            }
            else
            {
                newData[i] = controllerData.directedCircleOverlaps[i];
            }
        }
        controllerData.directedCircleOverlaps = newData;
    }
    public static void RemoveAt(ControllerData controllerData, int index)
    {
        if(controllerData.directedCircleOverlaps == null || controllerData.directedCircleOverlaps.Length == 0)
        {
            return;
        }
        DirectedCircleOverlap[] newData = new DirectedCircleOverlap[controllerData.directedCircleOverlaps.Length - 1];
        for (int i = 0; i < controllerData.directedCircleOverlaps.Length; i++)
        {
            if(i < index)
            {
                newData[i] = controllerData.directedCircleOverlaps[i];
            }
            else if(i > index)
            {
                newData[i - 1] = controllerData.directedCircleOverlaps[i];
            }
        }
        controllerData.directedCircleOverlaps = newData;
    }
}
[Serializable]
public class CircleSpriteMask
{
    public string nickname;
    public string stateName;
    public List<int> assignedFrames;
    public Color color;
    public bool isCustomRangeActive;
    public string sortingLayer;
    public int sortingOrder;
    public List<Vector2> centers;
    public List<float> radii;
    public CircleSpriteMask()
    {
        nickname = string.Empty;
        stateName = string.Empty;
        assignedFrames = new List<int>();
        color = Color.white;
        isCustomRangeActive = false;
        sortingLayer = "Default";
        sortingOrder = 0;
        centers = new List<Vector2>();
        radii = new List<float>();
    }
    public static CircleSpriteMask CreateNew(string stateName)
    {
        CircleSpriteMask circleSpriteMask = new()
        {
            nickname = "New Circle Sprite Mask",
            stateName = stateName,
            assignedFrames = new List<int>(),
            color = Color.white,
            isCustomRangeActive = false,
            sortingLayer = "Default",
            sortingOrder = 0,
            centers = new List<Vector2>(),
            radii = new List<float>()
        };
        return circleSpriteMask;
    }
    public static void AddNew(ControllerData controllerData, CircleSpriteMask stateDataToAdd)
    {
        if(controllerData.circleSpriteMasks == null || controllerData.circleSpriteMasks.Length == 0)
        {
            controllerData.circleSpriteMasks = new CircleSpriteMask[] { stateDataToAdd };
            return;
        }
        CircleSpriteMask[] newData = new CircleSpriteMask[controllerData.circleSpriteMasks.Length + 1];
        for (int i = 0; i < newData.Length; i++)
        {
            if(i == newData.Length - 1)
            {
                newData[i] = stateDataToAdd;
            }
            else
            {
                newData[i] = controllerData.circleSpriteMasks[i];
            }
        }
        controllerData.circleSpriteMasks = newData;
    }
    public static void RemoveAt(ControllerData controllerData, int index)
    {
        if(controllerData.circleSpriteMasks == null || controllerData.circleSpriteMasks.Length == 0)
        {
            return;
        }
        CircleSpriteMask[] newData = new CircleSpriteMask[controllerData.circleSpriteMasks.Length - 1];
        for (int i = 0; i < controllerData.circleSpriteMasks.Length; i++)
        {
            if(i < index)
            {
                newData[i] = controllerData.circleSpriteMasks[i];
            }
            else if(i > index)
            {
                newData[i - 1] = controllerData.circleSpriteMasks[i];
            }
        }
        controllerData.circleSpriteMasks = newData;
    }
}
[Serializable]
public class DirectedPoint
{
    public string nickname;
    public string stateName;
    public List<int> assignedFrames;
    public Color color;
    public List<Vector2> centers;
    public List<Vector2> upDirections;
    public List<Vector2> rightDirections;
    public DirectedPoint()
    {
        nickname = string.Empty;
        stateName = string.Empty;
        assignedFrames = new List<int>();
        color = Color.white;
        centers = new List<Vector2>();
        upDirections = new List<Vector2>();
        rightDirections = new List<Vector2>();
    }

    public static DirectedPoint CreateNew(string stateName)
    {
        DirectedPoint directedPoint = new()
        {
            nickname = "New Directed Point",
            stateName = stateName,
            assignedFrames = new List<int>(),
            color = Color.white,
            centers = new List<Vector2>(),
            upDirections = new List<Vector2>(),
            rightDirections = new List<Vector2>()
        };
        return directedPoint;
    }
    public static void AddNew(ControllerData controllerData, DirectedPoint stateDataToAdd)
    {
        if(controllerData.directedPoints == null || controllerData.directedPoints.Length == 0)
        {
            controllerData.directedPoints = new DirectedPoint[] { stateDataToAdd };
            return;
        }
        DirectedPoint[] newData = new DirectedPoint[controllerData.directedPoints.Length + 1];
        for (int i = 0; i < newData.Length; i++)
        {
            if(i == newData.Length - 1)
            {
                newData[i] = stateDataToAdd;
            }
            else
            {
                newData[i] = controllerData.directedPoints[i];
            }
        }
        controllerData.directedPoints = newData;
    }
    public static void RemoveAt(ControllerData controllerData, int index)
    {
        if(controllerData.directedPoints == null || controllerData.directedPoints.Length == 0)
        {
            return;
        }
        DirectedPoint[] newData = new DirectedPoint[controllerData.directedPoints.Length - 1];
        for (int i = 0; i < controllerData.directedPoints.Length; i++)
        {
            if(i < index)
            {
                newData[i] = controllerData.directedPoints[i];
            }
            else if(i > index)
            {
                newData[i - 1] = controllerData.directedPoints[i];
            }
        }
        controllerData.directedPoints = newData;
    }
}
public static class ResourcesUtility
{
    public static ControllerData[] LoadAllControllerDataInResources()
    {
        return Resources.LoadAll<ControllerData>("");
    }
    #if UNITY_EDITOR
    public static void CreateNewControllerData(string controllerName)
    {
        ControllerData controllerData = ScriptableObject.CreateInstance<ControllerData>();
        controllerData.controllerName = controllerName;
        string path = $"Assets/Resources/{controllerName}.asset";
        UnityEditor.AssetDatabase.CreateAsset(controllerData, path);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }
    #endif
}
public static class ControllerDataUtility
{
    public static DirectedCircleCollider[] GetDirectedCircleColliders(ControllerData controllerData, int stateHash)
    {
        List<DirectedCircleCollider> stateDatas = new();
        for (int i = 0; i < controllerData.directedCircleColliders.Length; i++)
        {
            if (Animator.StringToHash(controllerData.directedCircleColliders[i].stateName) == stateHash)
                stateDatas.Add(controllerData.directedCircleColliders[i]);
        }
        return stateDatas.ToArray();
    }
    public static DirectedCircleOverlap[] GetDirectedCircleOverlaps(ControllerData controllerData, int stateHash)
    {
        List<DirectedCircleOverlap> stateDatas = new();
        for (int i = 0; i < controllerData.directedCircleOverlaps.Length; i++)
        {
            if (Animator.StringToHash(controllerData.directedCircleOverlaps[i].stateName) == stateHash)
                stateDatas.Add(controllerData.directedCircleOverlaps[i]);
        }
        return stateDatas.ToArray();
    }
}