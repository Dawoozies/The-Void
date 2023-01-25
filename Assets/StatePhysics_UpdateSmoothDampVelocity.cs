using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePhysics_UpdateSmoothDampVelocity : StateMachineBehaviour
{
    Rigidbody2D rb;
    public Vector2 velocityTarget;
    public float smoothTime;
    Vector2 velocity;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        velocity = Vector2.zero;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (rb == null)
            rb = animator.GetComponentInParent<Rigidbody2D>();

        rb.velocity = Vector2.SmoothDamp(rb.velocity, velocityTarget, ref velocity, smoothTime);
    }
}
