using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LinearAlgebra;
public class MathTest : MonoBehaviour
{
    public Transform start;
    public Transform end;
    [Range(0f, 1f)]
    public float parameter;
    public Vector3 directionVector;
    public Vector3 point;

    ParametrisedLine line = new ParametrisedLine();
    private void Update()
    {
        line.pathStart = start.position;
        line.pathEnd = end.position;
        line.parameter = parameter;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(line.pathStart, line.pathEnd);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(line.point, 0.025f);
    }
}
