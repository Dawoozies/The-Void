using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour, Listener_AnyAttackInput
{
    Animator animator;
    float horizontalInput => InputManager.ins.L_Input.x;
    float verticalInput => InputManager.ins.L_Input.y;
    public bool lightAttackHeld;
    public float lightCharge;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        InputManager.ins.Subscribe((Listener_AnyAttackInput)this);
    }

    void Update()
    {
        animator.SetFloat("L_InputY", verticalInput);

        if(lightAttackHeld)
        {
            lightCharge += Time.deltaTime;
        }
    }

    public void Update_AnyAttackInput(string attackType, bool anyAttackInput)
    {
        
        //This way of doing things kinda goofs if you push two attack buttons at once but meh
        if(attackType == "Light Attack")
        {
            animator.SetBool("LightAttack", anyAttackInput);
            animator.SetBool("Attack", anyAttackInput);

            lightAttackHeld = anyAttackInput;
            
            if(anyAttackInput)
            {
                //Then we just pushed the button
                //So we want to start the charge time
                lightCharge = 0f;
            }
        }
    }
}
