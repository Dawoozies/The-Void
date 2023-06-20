using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OverlapHandlers.Player;
namespace RuntimeObjects
{
    public class Player : RuntimeObject
    {
        public PlayerLegs legs;
        public PlayerTorso torso;
        public float fallSpeedMax = -30f;
        public float ascentSpeedMax = 20f;
        public float landingLag = 0.125f;
        public float runSpeed = 15f;
        public bool grounded;
        public Player(string id) : base(id)
        {
            managedStart += ManagedStart;
            legs = new PlayerLegs("PlayerLegs");
            GameManager.ins.allRuntimeObjects.Add("PlayerLegs", legs);
            RuntimeAnimator.CreateAndAttach(legs, GameManager.ins.allControllers["PlayerLegs"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(legs);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(legs);
            RuntimeDirectedPoints.CreateAndAttach(legs);
            torso = new PlayerTorso("PlayerTorso");
            GameManager.ins.allRuntimeObjects.Add("PlayerTorso", torso);
            RuntimeAnimator.CreateAndAttach(torso, GameManager.ins.allControllers["PlayerTorso"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(torso);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(torso);
            RuntimeDirectedPoints.CreateAndAttach(torso);
            //legs.obj.SetParent(animator.animator.transform);
            //torso.obj.SetParent(animator.animator.transform);
        }
        public void ManagedStart()
        {
            managedUpdate += RuntimeAnimator.Update;
            managedUpdate += RuntimeRigidbody.Update;
            managedUpdate += RuntimeDirectedCircleOverlaps.Update;
            //managedUpdate += (RuntimeObject obj, float tickDelta) =>
            //{
            //    animator.animator.SetBool("Jump", InputManager.ins.JumpDown_Input || InputManager.ins.JumpDown_BufferedInput);
            //    animator.animator.SetBool("Run", InputManager.ins.L_Input.x != 0f);
            //    animator.animator.SetFloat("VelocityUp", rigidbody.upSpeed);
            //    animator.animator.SetBool("Recalling", InputManager.ins.LeftBumper_Input);
            //    animator.animator.SetFloat("LeftTrigger_Input", InputManager.ins.LeftTrigger_Input);
            //    animator.animator.SetInteger("R_Direction", Direction.Compute8WayDirection());
            //    animator.animator.SetBool("RightBumper_Input", InputManager.ins.RightBumper_Input);
            //};
            managedUpdate += StateHandlers.Player.Handler.Update;
            managedFixedUpdate += StateHandlers.Player.Handler.PhysicsUpdate;
            directedCircleOverlaps.onRuntimeObjectOverlap += OnRuntimeObjectOverlap.Handle;
            directedCircleOverlaps.onNonRuntimeObjectOverlap += OnNonRuntimeObjectOverlap.Handle;
            directedCircleOverlaps.onNullOverlap += OnNullResult.Handle;
            animator.onStateEnter += StateHandlers.Player.Handler.OnStateEnter;
            animator.onFrameUpdate += StateHandlers.Player.Handler.OnFrameUpdate;

            animator.spriteRenderer.color = Color.clear;
        }
    }
    public class PlayerLegs : RuntimeObject
    {
        public PlayerLegs(string id) : base(id)
        {
            managedStart += ManagedStart;
        }
        public void ManagedStart()
        {
            managedUpdate += RuntimeAnimator.Update;
            managedUpdate += RuntimeDirectedCircleOverlaps.Update;
            directedCircleOverlaps.onRuntimeObjectOverlap += OnRuntimeObjectOverlap.Handle;
            directedCircleOverlaps.onNonRuntimeObjectOverlap += OnNonRuntimeObjectOverlap.Handle;
            directedCircleOverlaps.onNullOverlap += OnNullResult.Handle;
            obj.SetParent(GameManager.ins.allRuntimeObjects["Player"].animator.animator.transform);
            animator.spriteRenderer.sortingOrder = 4;
        }
    }
    public class PlayerTorso : RuntimeObject
    {
        public PlayerTorso(string id) : base(id)
        {
            managedStart += ManagedStart;
        }
        public void ManagedStart()
        {
            managedUpdate += RuntimeAnimator.Update;
            managedUpdate += RuntimeDirectedCircleOverlaps.Update;
            directedCircleOverlaps.onRuntimeObjectOverlap += OnRuntimeObjectOverlap.Handle;
            directedCircleOverlaps.onNonRuntimeObjectOverlap += OnNonRuntimeObjectOverlap.Handle;
            directedCircleOverlaps.onNullOverlap += OnNullResult.Handle;
            obj.SetParent(GameManager.ins.allRuntimeObjects["Player"].animator.animator.transform);
            animator.spriteRenderer.sortingOrder = 5;
        }
    }
}