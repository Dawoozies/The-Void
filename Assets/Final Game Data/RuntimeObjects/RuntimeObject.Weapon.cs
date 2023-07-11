using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateHandlers.Weapon;
namespace RuntimeObjects
{
    public class Weapon : RuntimeObject
    {
        public WeaponHead head;
        public WeaponShaft shaft;
        public WeaponPommel pommel;
        public RuntimeObject owner;
        public Weapon(string id) : base(id)
        {
            RuntimeAnimator.CreateAndAttach(this, GameManager.ins.allControllers["Weapon"]);
            RuntimeRigidbody.CreateAndAttach(this);
            head = new WeaponHead();
            //GameManager.ins.allRuntimeObjects.Add("WeaponHead", head);
            RuntimeAnimator.CreateAndAttach(head, GameManager.ins.allControllers["WeaponHead"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(head);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(head);
            RuntimeDirectedPoints.CreateAndAttach(head);
            shaft = new WeaponShaft();
            //GameManager.ins.allRuntimeObjects.Add("WeaponShaft", shaft);
            RuntimeAnimator.CreateAndAttach(shaft, GameManager.ins.allControllers["WeaponShaft"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(shaft);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(shaft);
            RuntimeDirectedPoints.CreateAndAttach(shaft);
            pommel = new WeaponPommel();
            //GameManager.ins.allRuntimeObjects.Add("WeaponPommel", pommel);
            RuntimeAnimator.CreateAndAttach(pommel, GameManager.ins.allControllers["WeaponPommel"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(pommel);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(pommel);
            RuntimeDirectedPoints.CreateAndAttach(pommel);

            head.obj.SetParent(obj);
            head.obj.localPosition = new Vector3(0, 1.35f, 0);
            shaft.obj.SetParent(obj);
            pommel.obj.SetParent(obj);
            pommel.obj.localPosition = new Vector3(0, -1.35f, 0);

            managedUpdate += RuntimeAnimator.Update;
            animator.onStateEnter += Handler.OnStateEnter;
            animator.onFrameUpdate += Handler.OnFrameUpdate;


            head.animator.spriteRenderer.sortingOrder = 12;
            shaft.animator.spriteRenderer.sortingOrder = 13;
            pommel.animator.spriteRenderer.sortingOrder = 12;

            animator.spriteRenderer.color = Color.clear;
        }
        public void SetOwner(RuntimeObject owner)
        {
            this.owner = owner;
            owner.managedUpdate += AnchorUpdate;
        }
        void AnchorUpdate(RuntimeObject obj, float tickDelta)
        {
            if (owner == null)
                return;
            if (!owner.objStructure.HasFlag(RuntimeObjectStructure.DirectedPoint))
                return;
            DirectedPoint anchor = owner.directedPoints.GetDirectedPoint("Weapon_Anchor");
            if(anchor != null)
            {
                //Debug.LogError("This running?");
                rigidbody.rbObj.position = owner.RelativePos(anchor.centers[0]);
                rigidbody.rbObj.up = owner.RelativeDir(anchor.upDirections[0]);
                //obj.rigidbody.rbObj.position = owner.RelativePos(anchor.centers[0]);
                //obj.rigidbody.rbObj.up = owner.RelativeDir(anchor.upDirections[0]);
            }
        }
    }
    public class WeaponHead : RuntimeObject
    {
        public WeaponHead() : base("WeaponHead")
        {
            managedStart += ManagedStart;
        }
        public void ManagedStart()
        {
            managedUpdate += RuntimeAnimator.Update;
            animator.onStateEnter += Handler.OnStateEnter;
            animator.onFrameUpdate += Handler.OnFrameUpdate;
        }
    }
    public class WeaponShaft : RuntimeObject
    {
        public WeaponShaft() : base("WeaponShaft")
        {
            managedStart += ManagedStart;
        }
        public void ManagedStart()
        {
            managedUpdate += RuntimeAnimator.Update;
            animator.onStateEnter += Handler.OnStateEnter;
            animator.onFrameUpdate += Handler.OnFrameUpdate;
        }
    }
    public class WeaponPommel : RuntimeObject
    {
        public WeaponPommel() : base("WeaponPommel")
        {
            managedStart += ManagedStart;
        }
        public void ManagedStart()
        {
            managedUpdate += RuntimeAnimator.Update;
            animator.onStateEnter += Handler.OnStateEnter;
            animator.onFrameUpdate += Handler.OnFrameUpdate;
        }
    }
}