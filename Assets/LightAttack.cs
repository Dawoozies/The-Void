using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAttack : MonoBehaviour
{
    string controllerType => BasicInput.ins.ControllerType;
    Animator animator;
    AnimatorStateInfo stateInfo;
    int buttonPresses;
    public bool buttonDown;
    public bool buttonUp;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }
    void Update()
    {
        if (Input.GetButtonDown($"LightAttack{controllerType}"))
            Debug.Log("It's not going to work");


        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if(Input.GetButton($"LightAttack{controllerType}") && !buttonDown)
        {
            Debug.Log("Button Down");
            buttonDown = true;
            buttonUp = false;

            animator.SetTrigger("LightAttack");
        }

        if(!Input.GetButton($"LightAttack{controllerType}") && buttonDown && !buttonUp)
        {
            Debug.Log("Button Up");
            buttonDown = false;
            buttonUp = true;
        }
    }
}
