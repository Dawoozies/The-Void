using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornetFirstPass : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;
    public int intervals = 10;
    public int intervalLength = 10;
    public List<Vector3> pathSegments = new List<Vector3>();
    public Vector3 initialVelocity;
    public bool applyVelocity;
    public bool calculatePath;
    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if(Mathf.Abs(BasicInput.ins.InputLHorizontal) > 0f)
        {
            animator.SetBool("Run", true);
        }
        else
        {
            animator.SetBool("Run", false);
        }
    }
    void FixedUpdate()
    {
        if(calculatePath)
        {
            //rb.velocity = initialVelocity;
            pathSegments = new List<Vector3>();

            for (int i = 0; i < intervals; i++)
            {
                float currentTime = i * intervalLength * Time.fixedDeltaTime;
                float xPos = transform.position.x + initialVelocity.x * currentTime;
                float yPos = transform.position.y + initialVelocity.y * currentTime + 0.5f * Physics2D.gravity.y * Mathf.Pow(currentTime, 2);
                Vector3 newPoint = new Vector3(xPos, yPos);
                pathSegments.Add(newPoint);
            }

            calculatePath = false;
        }

        if(applyVelocity)
        {
            rb.velocity = initialVelocity;

            applyVelocity = false;
        }
    }

    private void OnDrawGizmos()
    {
        if(rb != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, rb.velocity + (Vector2)transform.position);

            //So there are at least two points in pathSegments
            if(pathSegments.Count > 1)
            {
                for (int i = 0; i < pathSegments.Count - 1; i++)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(pathSegments[i], pathSegments[i + 1]);
                }
            }
        }
    }
}
