using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalScaleToVelocityDirection : StateMachineBehaviour
{
    Rigidbody2D rb;
    public Vector2 directionMultiplier;
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (rb == null)
            rb = animator.GetComponentInParent<Rigidbody2D>();
        float xScale = 0;
        float yScale = 0;
        if(rb.velocity.x != 0)
            xScale = Mathf.Sign(rb.velocity.x) * directionMultiplier.x;
        if(rb.velocity.y != 0)
            yScale = Mathf.Sign(rb.velocity.y) * directionMultiplier.y;
        if(xScale == 0)
            xScale = animator.transform.localScale.x;
        if (yScale == 0)
            yScale = animator.transform.localScale.y;
        animator.transform.localScale = new Vector3(xScale,yScale,animator.transform.localScale.z);
    }
}
