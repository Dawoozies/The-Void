using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OverlapHandlers.Player;
namespace RuntimeObjects
{
    public class Player : RuntimeObject
    {
        public float fallSpeedMax = -20f;
        public float ascentSpeedMax = 20f;
        public Player(string id) : base(id)
        {
            managedStart += ManagedStart;
        }
        public void ManagedStart()
        {
            managedUpdate += RuntimeAnimator.Update;
            managedUpdate += RuntimeRigidbody.Update;
            managedUpdate += RuntimeDirectedCircleOverlaps.Update;
            managedUpdate += (RuntimeObject obj, float tickDelta) =>
            {

                animator.animator.SetBool("Jump", InputManager.ins.JumpDown_Input || InputManager.ins.JumpDown_BufferedInput);
                animator.animator.SetBool("Run", InputManager.ins.L_Input.x != 0f);
                animator.animator.SetFloat("VelocityUp", rigidbody.upSpeed);
                animator.animator.SetBool("Recalling", InputManager.ins.LeftBumper_Input);
                animator.animator.SetFloat("LeftTrigger_Input", InputManager.ins.LeftTrigger_Input);
                animator.animator.SetInteger("R_Direction", Direction.Compute8WayDirection());
                animator.animator.SetBool("RightBumper_Input", InputManager.ins.RightBumper_Input);
            };
            managedUpdate += StateHandlers.Player.Handler.Update;
            directedCircleOverlaps.onRuntimeObjectOverlap += OnRuntimeObjectOverlap.Handle;
            directedCircleOverlaps.onNonRuntimeObjectOverlap += OnNonRuntimeObjectOverlap.Handle;
            directedCircleOverlaps.onNullOverlap += OnNullResult.Handle;
        }
    }
}