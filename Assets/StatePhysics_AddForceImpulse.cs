using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePhysics_AddForceImpulse : StateMachineBehaviour
{
    Rigidbody2D rb;
    public Vector2 force;
    public bool sameDirectionAsPlayer;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (rb == null)
            rb = animator.GetComponentInParent<Rigidbody2D>();

        if (sameDirectionAsPlayer)
        {
            rb.AddForce(new Vector2(animator.transform.localScale.x * force.x, force.y), ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }
}
