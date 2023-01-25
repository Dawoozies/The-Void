using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteDirection : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;
    public bool rightFacing = false;
    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponentInParent<Rigidbody2D>();
    }
    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Roll"))
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
        //This only works with sprite sheets that have default left facing characters
        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Turn"))
        {
            if (BasicInput.ins.InputLDirection > 0)
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
            if (BasicInput.ins.InputLDirection < 0)
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
        else
        {
            if (BasicInput.ins.InputLDirection > 0)
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
            if (BasicInput.ins.InputLDirection < 0)
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
        }
    }
}
