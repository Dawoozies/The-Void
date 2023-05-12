using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Geometry;
using ExtensionMethods_List;
namespace AnimatorStateData
{
    [Serializable]
    public class StateData : ScriptableObject
    {
        public int stateHash;
        CircleCollider2DStateData _CircleCollider2DStateData;
        OverlapStateData _OverlapStateData;
    }
    [Serializable]
    public class CircleCollider2DStateData : ScriptableObject
    {
        public bool isTrigger;
        public int layer;
        public List<Circle> circles;
        public CircleCollider2DStateData()
        {
            isTrigger = false;
            layer = 0;
            circles = new List<Circle>();
        }
        public CircleCollider2DStateData(bool isTrigger, int layer, List<Circle> circles)
        {
            this.isTrigger = isTrigger;
            this.layer = layer;
            this.circles = circles;
        }
        public CircleCollider2DStateData Copy()
        {
            return new CircleCollider2DStateData(isTrigger, layer, circles);
        }
        public void Paste(CircleCollider2DStateData circleCollider2DStateData)
        {
            isTrigger = circleCollider2DStateData.isTrigger;
            layer = circleCollider2DStateData.layer;
            circles = circleCollider2DStateData.circles.Copy();
        }
    }
    [Serializable]
    public class OverlapStateData : ScriptableObject
    {
        public LayerMask layerMask;
        public List<Circle> circles;
        public List<Box> boxes;
        public List<Area> areas;
    }
    //We are making all the different data types also scriptable objects
    //This will help us reduce memory when dealing with many duplicate states
    //I.e. Run and RunRecall should have the same hitboxes
}