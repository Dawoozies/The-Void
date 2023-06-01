using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSpin : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    Vector3 A => pointA.position;
    Vector3 B => pointB.position;
    public LineRenderer lineRenderer;
    public Rigidbody2D rb;
    [Range(0,200)]
    public float spinSpeed;
    // Update is called once per frame
    void Update()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, A);
        lineRenderer.SetPosition(1, B);
        pointB.transform.up = (A-B).normalized;
        rb.velocity = spinSpeed*pointB.transform.right;
    }
}
