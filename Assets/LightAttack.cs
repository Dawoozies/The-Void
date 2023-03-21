using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAttack : MonoBehaviour, Listener_LightAttackInput
{
    Animator animator;
    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        InputManager.ins.Subscribe((Listener_LightAttackInput)this);
    }
    public void Update_LightAttackInput(bool lightAttackInput)
    {
        if (lightAttackInput)
        {
            animator.SetTrigger("LightAttack");
            animator.SetTrigger("Attack");
        }
    }
}
