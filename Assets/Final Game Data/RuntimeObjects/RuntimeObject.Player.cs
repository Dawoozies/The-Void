using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OverlapHandlers.Player;
using BehaviourRecord.Player;
using StateHandlers.Player;
namespace RuntimeObjects
{
    public class Player : RuntimeObject
    {
        public PlayerLegs legs;
        public PlayerTorso torso;
        public float fallSpeedMax = -30f;
        public float ascentSpeedMax = 30f;
        public float landingLag = 0.125f;
        public float runSpeed = 15f;
        public bool grounded;
        public float jumpVelocityAddTime = 0.175f;
        public float jumpApexRightSpeed = 2f;
        public float jumpApexTime = 0.35f;
        public int maxJumps = 4;
        public int jumpsLeft;
        public float doubleJumpStartTime = 0.15f;
        public float doubleJumpVelocityAddTime = 0.1f;
        public Vector2 doubleJumpShift = new Vector2(0.15f, 1.35f);
        public string jumpType = "Jump";
        public float airRollStartTime = 0.1f;
        public float airRollBraceDescentSpeed = -20f;
        public float airRollDescentSpeedMax = -30f;
        public float airRollLandBufferTime = 0.2f;
        public float slideMaxTime = 0.35f;
        public float slideMinSpeed = 15f; //should be equal to run speed
        public Player(string id) : base(id)
        {
            RuntimeAnimator.CreateAndAttach(this, GameManager.ins.allControllers["Player"]);
            RuntimeRigidbody.CreateAndAttach(this);
            RuntimeDirectedCircleColliders.CreateAndAttach(this);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(this);
            RuntimeDirectedPoints.CreateAndAttach(this);

            legs = new PlayerLegs();
            GameManager.ins.allRuntimeObjects.Add(legs);
            RuntimeAnimator.CreateAndAttach(legs, GameManager.ins.allControllers["PlayerLegs"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(legs);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(legs);
            RuntimeDirectedPoints.CreateAndAttach(legs);
            torso = new PlayerTorso();
            GameManager.ins.allRuntimeObjects.Add(torso);
            RuntimeAnimator.CreateAndAttach(torso, GameManager.ins.allControllers["PlayerTorso"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(torso);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(torso);
            RuntimeDirectedPoints.CreateAndAttach(torso);

            managedUpdate += RuntimePlayerDamage.Update; //Update damage for player
            managedUpdate += RuntimeAnimator.Update;
            managedUpdate += RuntimeDirectedCircleOverlaps.Update;
            managedUpdate += Handler.Update;
            managedUpdate += Record.Update;
            managedFixedUpdate += Handler.PhysicsUpdate;
            directedCircleOverlaps.onRuntimeObjectOverlap += OnRuntimeObjectOverlap.Handle;
            directedCircleOverlaps.onNonRuntimeObjectOverlap += OnNonRuntimeObjectOverlap.Handle;
            directedCircleOverlaps.onNullOverlap += OnNullResult.Handle;
            animator.onStateEnter += Handler.OnStateEnter;
            animator.onFrameUpdate += Handler.OnFrameUpdate;
            RuntimePlayerDamage.onDamageProcessed += Handler.OnDamageProcessed;
            animator.spriteRenderer.color = Color.clear;

            legs.obj.SetParent(obj);
            torso.obj.SetParent(obj);

            RuntimePlayerWeapon.onGetWeapon += Handler.OnGetWeapon;
            RuntimePlayerWeapon.onWeaponThrow += Handler.OnWeaponThrow;
            RuntimePlayerWeapon.onWeaponMelee += Handler.OnWeaponMelee;
        }
    }
    public class PlayerLegs : RuntimeObject
    {
        public PlayerLegs() : base("PlayerLegs")
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
            //obj.SetParent(GameManager.ins.allRuntimeObjects["Player"].animator.animator.transform); 
            animator.spriteRenderer.sortingOrder = 10;
        }
    }
    public class PlayerTorso : RuntimeObject
    {
        public PlayerTorso() : base("PlayerTorso")
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
            //obj.SetParent(GameManager.ins.allRuntimeObjects["Player"].animator.animator.transform);
            animator.spriteRenderer.sortingOrder = 11;
        }
    }
}