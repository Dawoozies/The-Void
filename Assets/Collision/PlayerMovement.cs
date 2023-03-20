using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, Listener_JumpInput, Listener_DodgeInput, Listener_JumpReleaseInput
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
    public int jumpCount;
    int maxJumpCount;
    public float jumpTime;
    float maxJumpTime;
    // Start is called before the first frame update
    void Start()
    {
        jumpCount = 1;
        maxJumpCount = 1;
        jumpTime = 0f;
        maxJumpTime = 0.35f;

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
        InputManager.ins.Subscribe((Listener_JumpReleaseInput)this);
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

        if(jumpCount < maxJumpCount)
        {
            if(jumpTime > 0)
            {
                jumpTime -= Time.deltaTime;
            }
            else
            {
                bool refreshJump = animator.GetCurrentAnimatorStateInfo(0).IsName("JumpLand");

                if (refreshJump)
                    jumpCount++;
            }
        }

        animator.SetFloat("VelocityY", rb.velocity.y);
    }

    void FixedUpdate()
    {
        // Horizontal Movement
        if(animator.GetCurrentAnimatorStateInfo(0).IsTag("Roll"))
            return;

        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("AirDodge"))
            return;

        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("GroundAttack"))
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
            //rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        }

        if(jumpTime > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        }
    }
    public void Update_JumpInput(bool jumpInput)
    {
        bool canJump =
            animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")
            || animator.GetCurrentAnimatorStateInfo(0).IsName("Run")
            || animator.GetCurrentAnimatorStateInfo(0).IsName("JumpLandStartRun");
        if (canJump && jumpCount > 0)
        {
            jumpCount--;
            jumpTime = maxJumpTime;
        }
    }

    public void Update_DodgeInput(bool dodgeInput)
    {
        animator.SetBool("DodgeInput", dodgeInput);
    }

    public void Update_JumpReleaseInput()
    {
        jumpTime = 0;
    }
}