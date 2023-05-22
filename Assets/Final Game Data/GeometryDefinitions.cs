using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
namespace GeometryDefinitions
{
    [Serializable]
    public class Circle
    {
        public Vector3 center;
        public float radius;
        public Circle ()
        {
            center = Vector3.zero;
            radius = 0f;
        }
        public void CopyToNew(Circle copy)
        {
            copy.center = center;
            copy.radius = radius;
        }
    }
    [Serializable]
    public class Box
    {
        public Vector2 center;
        public Vector2 size;
        public float angle;
        public Box()
        {
            center = Vector2.zero;
            size = Vector2.zero;
            angle = 0f;
        }
        public void CopyToNew(Box copy)
        {
            copy.center = center;
            copy.size = size;
            copy.angle = angle;
        }
    }
    [Serializable]
    public class Area
    {
        public Vector2 pointA;
        public Vector2 pointB;
        public Area()
        {
            pointA = Vector2.zero;
            pointB = Vector2.zero;
        }
        public void CopyToNew(Area copy)
        {
            copy.pointA = pointA;
            copy.pointB = pointB;
        }
    }
}