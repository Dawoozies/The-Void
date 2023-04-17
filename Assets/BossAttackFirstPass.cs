using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackFirstPass : MonoBehaviour
{
    Animator animator;
    public int attackCount;
    AnimatorStateInfo lastStateInfo;
    public int attackChoice;
    public bool forceUseAttack;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(forceUseAttack && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            animator.SetInteger("ATKChoice", attackChoice);
            animator.SetTrigger("ATK");

            return;
        }    


        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            animator.SetInteger("ATKChoice", Random.Range(0, attackCount));
            animator.SetTrigger("ATK");
        }
    }
}
