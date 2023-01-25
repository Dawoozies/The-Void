using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePhysics_EnterSetVelocity : StateMachineBehaviour
{
    Rigidbody2D rb;
    public Vector2 velocityToSet;
    public bool sameDirectionAsPlayer;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (rb == null)
            rb = animator.GetComponentInParent<Rigidbody2D>();

        if(sameDirectionAsPlayer)
        {
            rb.velocity = new Vector2(animator.transform.localScale.x * velocityToSet.x, velocityToSet.y);
        }
        else
        {
            rb.velocity = velocityToSet;
        }
    }
}
