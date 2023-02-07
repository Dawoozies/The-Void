using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float horizontalInput => BasicInput.ins.InputLHorizontal;
    float normalizedHorizontalInput => new Vector2(horizontalInput, 0).normalized.x;
    bool jumpInput => BasicInput.ins.InputJump;
    bool rollInput => BasicInput.ins.InputRoll;
    //Horizontal Movement
    float maxSpeed = 15;
    float accForce = 100;
    float dashMultiplier = 5;

    //Vertical Movement
    float jumpVelocity = 20;
    Animator animator;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (animator == null || animator.enabled == false) {
            Debug.LogError("Animator not found for the player");
        }
        if (rb == null)
        {
            Debug.LogError("RigidBody not found for the player");
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(rollInput)
        {
            animator.SetBool("Roll", true);
        }
        else
        {
            animator.SetBool("Roll", false);
        }

        if (Mathf.Abs(horizontalInput) > 0f)
        {
            animator.SetBool("Run", true);
        }
        else
        {
            animator.SetBool("Run", false);
        }
        //Debug.Log(rb.velocity);
        if(jumpInput)
        {
            animator.SetBool("JumpInputDown", true);
        }
        else
        {
            animator.SetBool("JumpInputDown", false);
        }

        if(animator.GetCurrentAnimatorStateInfo(0).IsTag("JumpAscent"))
        {
            animator.SetFloat("JumpTime", animator.GetFloat("JumpTime") + Time.deltaTime);
        }
        else
        {
            animator.SetFloat("JumpTime", 0f);
        }
    }

    void FixedUpdate()
    {
        // Horizontal Movement
        if(animator.GetCurrentAnimatorStateInfo(0).IsTag("Roll"))
            return;

        RunMovement();
        JumpMovement();
    }

    void RunMovement()
    {
        rb.velocity = new Vector2(maxSpeed * horizontalInput, rb.velocity.y);
       
    }

    void LegacyRunMovement() {
        //----------Run with speed acceleration
        if (normalizedHorizontalInput == 1)
        {
            rb.AddForce(new Vector2((rb.velocity.x < 0 ? 1 : dashMultiplier) * accForce * Mathf.Abs(horizontalInput) * (1 - rb.velocity.x / maxSpeed), 0));
        }
        else if (normalizedHorizontalInput == -1)
        {
            rb.AddForce(new Vector2((rb.velocity.x > 0 ? 1 : dashMultiplier) * -accForce * Mathf.Abs(horizontalInput) * (1 - rb.velocity.x / -maxSpeed), 0));
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    void JumpMovement()
    {
        if (jumpInput && animator.GetCurrentAnimatorStateInfo(0).IsTag("JumpAscent"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        }
    }
}
