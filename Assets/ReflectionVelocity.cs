using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionVelocity : StateMachineBehaviour
{
    public float reflectionStrength;
    public LayerMask layerMask;
    Rigidbody2D rb;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        RaycastHit2D hit = Physics2D.Raycast(animator.transform.position, -animator.transform.up, 1000f, layerMask);
        if (hit.collider == null)
            return;
        rb = animator.GetComponentInParent<Rigidbody2D>();
        if (rb == null)
            return;
        rb.velocity = reflectionStrength*Vector3.Reflect(-animator.transform.up, hit.normal);
        animator.transform.up = rb.velocity;
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        RaycastHit2D hit = Physics2D.Raycast(animator.transform.position, animator.transform.up, 1f, layerMask);
        if (hit.collider == null)
            return;
        rb = animator.GetComponentInParent<Rigidbody2D>();
        if (rb == null)
            return;
        //rb.velocity = reflectionStrength * Vector3.Reflect(animator.transform.up, hit.normal);
        rb.velocity = Vector3.Reflect(Vector3.Project(rb.velocity, animator.transform.up), hit.normal);
    }
}