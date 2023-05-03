using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods_Bool;
public class PlayerAnimatorParameters : MonoBehaviour, Listener_AnyAttackInput, Listener_GroundboxOverlap, Listener_JumpInput
{
    Vector2 L_Input => InputManager.ins.L_Input;
    Vector2 R_Input => InputManager.ins.R_Input;
    float LeftTrigger_Input => InputManager.ins.LeftTrigger_Input;
    int R_Direction => DirectionManager.ins.R_Direction;
    bool EquippedHalberd => PlayerDataManager.ins.EquippedHalberd;
    Animator animator;
    Rigidbody2D rb;
    bool jumpInput;
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        InputManager.ins.Subscribe((Listener_AnyAttackInput)this);
        InputManager.ins.Subscribe((Listener_JumpInput)this);
    }
    void Update()
    {
        animator.SetFloat("L_InputX", L_Input.x);
        animator.SetFloat("L_InputY", L_Input.y);
        animator.SetFloat("R_InputX", R_Input.x);
        animator.SetFloat("R_InputY", R_Input.y);
        animator.SetFloat("VelocityX", rb.velocity.x);
        animator.SetFloat("VelocityY", rb.velocity.y);
        animator.SetBool("JumpInput", jumpInput);
        animator.SetFloat("LeftTrigger_Input", LeftTrigger_Input);
        animator.SetInteger("R_Direction", R_Direction);
        animator.SetBool("EquippedHalberd", EquippedHalberd);
        animator.SetFloat("AirTime", (animator.GetFloat("AirTime")+Time.deltaTime)*animator.GetBool("Grounded").DefinedValue(1f,0f));
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

    public void Update_JumpInput(bool jumpInput)
    {
        this.jumpInput = jumpInput;
    }
}
