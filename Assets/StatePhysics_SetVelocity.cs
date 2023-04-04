using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StatePhysics_SetVelocity : StateMachineBehaviour
{
    Rigidbody2D rb;
    public Vector2 velocityToSet;
    public bool conserveXVelocity;
    public bool conserveYVelocity;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (rb == null)
            rb = animator.GetComponentInParent<Rigidbody2D>();

        if(conserveXVelocity && conserveYVelocity)
        {
            Debug.LogError("StatePhysics_SetVelocity Error: Conserve X and Y velocity true, this script will do nothing!");
        }

        Vector2 finalVelocity = velocityToSet;
        if(conserveXVelocity)
            finalVelocity = new Vector2(rb.velocity.x, finalVelocity.y);

        if (conserveYVelocity)
            finalVelocity = new Vector2(finalVelocity.x, rb.velocity.y);

        rb.velocity += -rb.velocity + finalVelocity;
    }
}
