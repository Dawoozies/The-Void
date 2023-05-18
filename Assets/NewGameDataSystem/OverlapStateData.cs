using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Geometry;
using System;
using ExtensionMethods_List;
namespace GameData.StateData
{
    [Serializable]
    public class OverlapStateData : ScriptableObject, ScriptableObjectInitialization, StateDataCast<OverlapStateData>
    {
        public string dataId;
        public LayerMask layerMask;
        public List<Circle> circles;
        public List<Box> boxes;
        public List<Area> areas;
        public OverlapStateData GetCastedData()
        {
            return this;
        }
        public void Initialize()
        {
            layerMask = 0;
            circles = new List<Circle>();
            boxes = new List<Box>();
            areas = new List<Area>();
        }
    }
    //We are making all the different data types also scriptable objects
    //This will help us reduce memory when dealing with many duplicate states
    //I.e. Run and RunRecall should have the same hitboxes
}