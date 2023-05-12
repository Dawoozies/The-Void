using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWallGrab : StateMachineBehaviour
{
    Rigidbody2D rb;
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (rb == null)
            rb = GameObject.Find("Player").GetComponent<Rigidbody2D>();

        rb.velocity = new Vector2(8f*InputManager.ins.L_Input.x, Mathf.Clamp(15f*InputManager.ins.L_Input.y, -15f, 0f));
    }
}
