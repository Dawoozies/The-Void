using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour, Listener_AnyAttackInput
{
    Animator animator;
    float verticalInput => InputManager.ins.L_Input.y;
    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        InputManager.ins.Subscribe((Listener_AnyAttackInput)this);
    }

    void Update()
    {
        animator.SetFloat("L_InputY", verticalInput);
    }

    public void Update_AnyAttackInput(string attackType, bool anyAttackInput)
    {
        
        //This way of doing things kinda goofs if you push two attack buttons at once but meh
        if(attackType == "Light Attack")
        {
            animator.SetBool("LightAttack", anyAttackInput);
            animator.SetBool("Attack", anyAttackInput);
        }
    }
}
