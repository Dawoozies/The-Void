using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Geometry;
using System;
using ExtensionMethods_List;
using UnityEditor;
namespace GameData.StateData
{
    [Serializable]
    public class CircleCollider2DStateData : ScriptableObject, ScriptableObjectInitialization, StateDataCast<CircleCollider2DStateData>
    {
        public string dataId;
        public bool isTrigger;
        public int layer;
        public List<Circle> circles;
        public void Paste(CircleCollider2DStateData circleCollider2DStateData)
        {
            isTrigger = circleCollider2DStateData.isTrigger;
            layer = circleCollider2DStateData.layer;
            circles = circleCollider2DStateData.circles.Copy();
        }
        public CircleCollider2DStateData GetCastedData()
        {
            return this;
        }

        public void Initialize()
        {
            isTrigger = false;
            layer = 0;
            circles = new List<Circle>();
        }
    }
}