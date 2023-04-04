using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour, Listener_AnyAttackInput
{
    Animator animator;
    float horizontalInput => InputManager.ins.L_Input.x;
    float verticalInput => InputManager.ins.L_Input.y;
    bool lightAttackHeld;
    float lightCharge;

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

        animator.SetFloat("AttackCharge", lightCharge);

        bool atLowThreshold = (Mathf.Abs(horizontalInput) <= 0.35f);
        bool upDirection = false;
        bool neutralDirection = false;
        bool downDirection = false;
        if(atLowThreshold)
        {
            //Directional input should stay at max sensitivity
            if(verticalInput > 0.25f)
            {
                upDirection = true;
            }
            if(verticalInput < -0.25f)
            {
                downDirection = true;
            }
        }
        else
        {
            //0.95f comes from specific line gradient
            if(verticalInput > Mathf.Abs(0.95f*horizontalInput))
            {
                upDirection = true;
            }
            if(verticalInput < -Mathf.Abs(0.95f*horizontalInput))
            {
                downDirection = true;
            }
        }

        if(!upDirection && !downDirection)
        {
            neutralDirection = true;
        }

        animator.SetBool("ATKDirection = Up", upDirection);
        animator.SetBool("ATKDirection = Neutral", neutralDirection);
        animator.SetBool("ATKDirection = Down", downDirection);
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
