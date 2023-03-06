using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class FrameCollisionData
{
    public int frame;
    public List<Circle> circles;

    public FrameCollisionData (int frame, List<Circle> circles)
    {
        this.frame = frame;
        this.circles = circles;
    }

    public FrameCollisionData (int frame)
    {
        this.frame = frame;
        circles = new List<Circle>();
    }
}

[System.Serializable]
public class Circle
{
    public Vector2 center = Vector2.zero;
    public float radius = 1f;

    public Circle (Vector2 center, float radius)
    {
        this.center = center;
        this.radius = radius;
    }

    public Circle()
    {
        this.center = Vector2.zero;
        this.radius = 1f;
    }
}
