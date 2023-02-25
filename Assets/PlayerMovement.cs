using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, Listener_JumpInput, Listener_DodgeInput
{
    //float horizontalInput => BasicInput.ins.InputLHorizontal;
    float horizontalInput => InputManager.ins.R_Input.x;
    float normalizedHorizontalInput => new Vector2(horizontalInput, 0).normalized.x;
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

        //Subscribe to InputManager
        InputManager.ins.Subscribe((Listener_JumpInput)this);
        InputManager.ins.Subscribe((Listener_DodgeInput)this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(horizontalInput) > 0f)
        {
            animator.SetBool("Run", true);
        }
        else
        {
            animator.SetBool("Run", false);
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

        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("AirDodge"))
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
        if (animator.GetBool("JumpInput") && animator.GetCurrentAnimatorStateInfo(0).IsTag("JumpAscent"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        }
    }
    public void Update_JumpInput(bool jumpInput)
    {
        animator.SetBool("JumpInput", jumpInput);
    }

    public void Update_DodgeInput(bool dodgeInput)
    {
        animator.SetBool("DodgeInput", dodgeInput);
    }
}