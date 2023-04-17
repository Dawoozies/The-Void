using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalberdUpSpin : StateMachineBehaviour
{
    Rigidbody2D rb;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (rb == null)
            rb = animator.GetComponentInParent<Rigidbody2D>();

        float attackCharge = animator.GetFloat("AttackCharge");
        bool shortCharge = (attackCharge < 0.125f);
        bool mediumCharge = (attackCharge >= 0.125f) && (attackCharge < 0.25f);
        bool longCharge = (attackCharge >= 0.25f) && (attackCharge < 0.45f);
        bool maxCharge = (attackCharge >= 0.45f);

        if(shortCharge)
        {
            Debug.Log("Short Charge with time = " + attackCharge);
            rb.velocity += new Vector2(0f, 15f);
        }
            

        if (mediumCharge)
        {
            Debug.Log("Medium Charge with time = " + attackCharge);
            rb.velocity += new Vector2(0f, 25f);
        }
            

        if (longCharge)
        {
            Debug.Log("Long Charge with time = " + attackCharge);
            rb.velocity += new Vector2(0f, 35f);
        }
            

        if (maxCharge)
        {
            Debug.Log("Max Charge with time = " + attackCharge);
            rb.velocity += new Vector2(0f, 43f);
        }
            
    }
}
