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
    public class Area
    {
        public Vector3 center;
        public float width;
        public float height;
        public Area()
        {
            center = Vector3.zero;
            width = 0f;
            height = 0f;
        }
        public void CopyToNew(Area copy)
        {
            copy.center = center;
            copy.width = width;
            copy.height = height;
        }
    }
    [Serializable]
    public class Point
    {
        public Vector2 point;
        public Point()
        {
            point = Vector2.zero;
        }
        public void CopyToNew(Point copy)
        {
            copy.point = point;
        }
    }
}