using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGravity : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;

    AnimatorStateInfo currentAnimationState;
    float gravity = 20;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }
    void FixedUpdate()
    {
        currentAnimationState = animator.GetCurrentAnimatorStateInfo(0);

        if(currentAnimationState.IsTag("ApplyGravity"))
        {
            rb.AddForce(new Vector2(0f, -gravity));
        }
        else
        {
            Debug.Log(rb.velocity);
            if(animator.GetBool("Grounded"))
            {
                if(rb.velocity.y < 0)
                {
                    //Then previous frame application of gravity is leaving behind some velocity
                    rb.velocity = new Vector2(rb.velocity.x, 0f);
                }
            }
        }
    }
}
