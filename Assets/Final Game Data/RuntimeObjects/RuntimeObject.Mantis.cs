using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OverlapHandler.Mantis;
namespace RuntimeObjects
{
    public class Mantis : RuntimeObject
    {
        public MantisLegs legs;
        public MantisTorso torso;
        public MantisLeftArm leftArm;
        public float aimTime = 0.26667f;
        public float linearStrikeWaitTime = 0.25f;
        public float retractTime = 0.14f;
        public bool useLinearStrikeSpin = true;
        public string strikeDirection = "Side";
        public Mantis(string id) : base(id)
        {
            managedStart += ManagedStart;
            legs = new MantisLegs();
            GameManager.ins.allRuntimeObjects.Add("MantisLegs", legs);
            RuntimeAnimator.CreateAndAttach(legs, GameManager.ins.allControllers["MantisLegs"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(legs);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(legs);
            RuntimeDirectedPoints.CreateAndAttach(legs);
            torso = new MantisTorso();
            GameManager.ins.allRuntimeObjects.Add("MantisTorso", torso);
            RuntimeAnimator.CreateAndAttach(torso, GameManager.ins.allControllers["MantisTorso"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(torso);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(torso);
            RuntimeDirectedPoints.CreateAndAttach(torso);
            leftArm = new MantisLeftArm();
            GameManager.ins.allRuntimeObjects.Add("MantisLeftArm", leftArm);
            RuntimeAnimator.CreateAndAttach(leftArm, GameManager.ins.allControllers["MantisLeftArm"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(leftArm);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(leftArm);
            RuntimeDirectedPoints.CreateAndAttach(leftArm);
        }
        public void ManagedStart()
        {
            managedUpdate += RuntimeAnimator.Update;
            managedUpdate += StateHandlers.Mantis.Handler.Update;
            animator.onStateEnter += StateHandlers.Mantis.Handler.OnStateEnter;
            animator.onFrameUpdate += StateHandlers.Mantis.Handler.OnFrameUpdate;
            animator.spriteRenderer.color = Color.clear;
        }
    }
    public class MantisLegs : RuntimeObject
    {
        public MantisLegs() : base("MantisLegs")
        {
            managedStart += ManagedStart;
        }
        public void ManagedStart()
        {
            managedUpdate += RuntimeAnimator.Update;
            animator.onStateEnter += StateHandlers.Mantis.Handler.OnStateEnter;
            animator.onFrameUpdate += StateHandlers.Mantis.Handler.OnFrameUpdate;
            obj.SetParent(GameManager.ins.allRuntimeObjects["Mantis"].animator.animator.transform);
            obj.localPosition = new Vector3(0, -11, 0);
            animator.spriteRenderer.sortingOrder = 4;
        }
    }
    public class MantisTorso : RuntimeObject
    {
        public MantisTorso() : base("MantisTorso")
        {
            managedStart += ManagedStart;
        }
        public void ManagedStart()
        {
            managedUpdate += RuntimeAnimator.Update;
            animator.onFrameUpdate += StateHandlers.Mantis.Handler.OnFrameUpdate;
            obj.SetParent(GameManager.ins.allRuntimeObjects["Mantis"].animator.animator.transform);
            animator.spriteRenderer.sortingOrder = 5;
        }
    }
    public class MantisLeftArm : RuntimeObject
    {
        public MantisLeftArm() : base("MantisLeftArm")
        {
            managedStart += ManagedStart;
        }
        public void ManagedStart()
        {
            managedUpdate += RuntimeAnimator.Update;
            managedUpdate += RuntimeDirectedCircleOverlaps.Update;
            directedCircleOverlaps.onRuntimeObjectOverlap += OnRuntimeObjectOverlap.Handle;
            obj.SetParent(GameManager.ins.allRuntimeObjects["MantisTorso"].animator.animator.transform);
            animator.spriteRenderer.sortingOrder = 6;
        }
    }
}