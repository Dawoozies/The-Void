using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGravity : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;

    float verticalInput => BasicInput.ins.InputLVertical;

    float gravity = 80;
    float fastFallGravity = 120;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }
    void FixedUpdate()
    {
        if(!animator.GetBool("Grounded"))
        {
            if(verticalInput < -0.75f)
            {
                rb.AddForce(new Vector2(0f, -fastFallGravity));
            }
            else
            {
                rb.AddForce(new Vector2(0f, -gravity));
            }
        }
        else
        {
            if (rb.velocity.y < 0)
            {
                //Then previous frame application of gravity is leaving behind some velocity
                rb.velocity = new Vector2(rb.velocity.x, 0f);
            }
        }
    }
}
