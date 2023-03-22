using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteDirection : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;
    float horizontalInput => InputManager.ins.L_Input.x;
    public bool rightFacing = false;
    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponentInParent<Rigidbody2D>();
    }
    void Update()
    {
        if (animator.GetBool("TurningNotAllowed"))
            return;

        RegularDirection();
    }

    void PhysicsBasedDirection()
    {
        if (rb.velocity.x > 0)
        {
            if (rightFacing)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
        if (rb.velocity.x < 0)
        {
            if (rightFacing)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    void RegularDirection()
    {
        if (horizontalInput > 0)
        {
            if (rightFacing)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        if (horizontalInput < 0)
        {
            if (rightFacing)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
