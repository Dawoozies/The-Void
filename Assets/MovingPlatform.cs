using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LinearAlgebra;
public class MovingPlatform : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float distanceThreshold;
    ParametrisedLine line = new ParametrisedLine();
    public float speed;
    public float offset;
    private void Start()
    {
        line.pathStart = startPoint.position;
        line.pathEnd = endPoint.position;
        line.parameter = offset;
    }
    void Update()
    {
        float endPointsDistance = Vector3.Distance(startPoint.position, endPoint.position);
        line.parameter += speed * (Time.deltaTime / endPointsDistance);
        transform.position = line.point;
        float distanceToEndPoint = Vector3.Distance(transform.position, endPoint.position);
        if (distanceToEndPoint <= distanceThreshold)
            line.parameter = 0f;
    }
}
