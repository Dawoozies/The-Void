using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorParameters : MonoBehaviour, Listener_AnyAttackInput, Listener_GroundboxOverlap
{
    Vector2 L_Input => InputManager.ins.L_Input;
    Vector2 R_Input => InputManager.ins.R_Input;
    Animator animator;
    Rigidbody2D rb;
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        InputManager.ins.Subscribe(this);
    }
    void Update()
    {
        animator.SetFloat("L_InputX", L_Input.x);
        animator.SetFloat("L_InputY", L_Input.y);
        animator.SetFloat("R_InputX", R_Input.x);
        animator.SetFloat("R_InputY", R_Input.y);
        animator.SetFloat("VelocityX", rb.velocity.x);
        animator.SetFloat("VelocityY", rb.velocity.y);
    }

    public void Update_AnyAttackInput(string attackType, bool anyAttackInput)
    {
        if(attackType == "Light Attack")
        {
            animator.SetBool("LightAttack", anyAttackInput);
        }
    }

    public void Update_GroundboxOverlap(bool hitResult)
    {
        animator.SetBool("Grounded", hitResult);
    }
}
