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
        public Weapon(string id) : base(id)
        {
            managedStart += ManagedStart;
            head = new WeaponHead();
            GameManager.ins.allRuntimeObjects.Add("WeaponHead", head);
            RuntimeAnimator.CreateAndAttach(head, GameManager.ins.allControllers["WeaponHead"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(head);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(head);
            RuntimeDirectedPoints.CreateAndAttach(head);
            shaft = new WeaponShaft();
            GameManager.ins.allRuntimeObjects.Add("WeaponShaft", shaft);
            RuntimeAnimator.CreateAndAttach(shaft, GameManager.ins.allControllers["WeaponShaft"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(shaft);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(shaft);
            RuntimeDirectedPoints.CreateAndAttach(shaft);
            pommel = new WeaponPommel();
            GameManager.ins.allRuntimeObjects.Add("WeaponPommel", pommel);
            RuntimeAnimator.CreateAndAttach(pommel, GameManager.ins.allControllers["WeaponPommel"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(pommel);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(pommel);
            RuntimeDirectedPoints.CreateAndAttach(pommel);
        }
        public void ManagedStart()
        {
            managedUpdate += RuntimeAnimator.Update;
            animator.onStateEnter += Handler.OnStateEnter;
            animator.onFrameUpdate += Handler.OnFrameUpdate;
            animator.spriteRenderer.color = Color.clear;
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
            obj.SetParent(GameManager.ins.allRuntimeObjects["Weapon"].animator.animator.transform);
            obj.localPosition = new Vector3(0, 0.95f, 0);
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
            obj.SetParent(GameManager.ins.allRuntimeObjects["Weapon"].animator.animator.transform);
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
            obj.SetParent(GameManager.ins.allRuntimeObjects["Weapon"].animator.animator.transform);
            obj.localPosition = new Vector3(0, -0.95f, 0);
        }
    }
}