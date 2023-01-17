using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackSystem : MonoBehaviour
{
    bool lightAttack => BasicInput.ins.InputLightAttack;
    Animator animator;
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(lightAttack)
        {
            animator.SetBool("LightAttackDown", true);
        }
        else
        {
            animator.SetBool("LightAttackDown", false);
        }
    }
}
