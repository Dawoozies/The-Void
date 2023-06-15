using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OverlapHandlers.HangedFrame;
namespace RuntimeObjects
{
    public class HangedFrame : RuntimeObject
    {
        public HangedFrameSubObject head;
        public HangedFrameSubObject torso;
        public HangedFrameSubObject leftArm;
        public HangedFrameSubObject leftHand;
        public HangedFrame(string id) : base(id)
        {
            managedStart += ManagedStart;
            head = new HangedFrameSubObject("HangedFrameHead");
            GameManager.ins.allRuntimeObjects.Add("HangedFrameHead", head);
            RuntimeAnimator.CreateAndAttach(head, GameManager.ins.allControllers["HangedFrameHead"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(head);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(head);
            //RuntimeDirectedPoints.CreateAndAttach(head);
            torso = new HangedFrameSubObject("HangedFrameTorso");
            GameManager.ins.allRuntimeObjects.Add("HangedFrameTorso", torso);
            RuntimeAnimator.CreateAndAttach(torso, GameManager.ins.allControllers["HangedFrameTorso"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(torso);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(torso);
            //RuntimeDirectedPoints.CreateAndAttach(torso);
            leftArm = new HangedFrameSubObject("HangedFrameLeftArm");
            GameManager.ins.allRuntimeObjects.Add("HangedFrameLeftArm", leftArm);
            RuntimeAnimator.CreateAndAttach(leftArm, GameManager.ins.allControllers["HangedFrameLeftArm"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(leftArm);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(leftArm);
            //RuntimeDirectedPoints.CreateAndAttach(leftArm);
            leftHand = new HangedFrameSubObject("HangedFrameLeftHand");
            GameManager.ins.allRuntimeObjects.Add("HangedFrameLeftHand", leftHand);
            RuntimeAnimator.CreateAndAttach(leftHand, GameManager.ins.allControllers["HangedFrameLeftHand"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(leftHand);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(leftHand);
            //RuntimeDirectedPoints.CreateAndAttach(leftHand);
        }
        public void ManagedStart()
        {
            managedUpdate += RuntimeAnimator.Update;
            managedUpdate += RuntimeDirectedCircleOverlaps.Update;
            managedUpdate += StateHandlers.HangedFrame.Handler.Update;
            directedCircleOverlaps.onRuntimeObjectOverlap += OnRuntimeObjectOverlap.Handle;
            directedCircleOverlaps.onNonRuntimeObjectOverlap += OnNonRuntimeObjectOverlap.Handle;
            directedCircleOverlaps.onNullOverlap += OnNullResult.Handle;
        }
    }
    public class HangedFrameSubObject : RuntimeObject
    {
        public HangedFrameSubObject(string id) : base(id)
        {
            managedStart += ManagedStart;
        }
        public void ManagedStart()
        {
            managedUpdate += RuntimeAnimator.Update;
            managedUpdate += RuntimeDirectedCircleOverlaps.Update;
            managedUpdate += StateHandlers.HangedFrame.Handler.Update;
            directedCircleOverlaps.onRuntimeObjectOverlap += OnRuntimeObjectOverlap.Handle;
            directedCircleOverlaps.onNonRuntimeObjectOverlap += OnNonRuntimeObjectOverlap.Handle;
            directedCircleOverlaps.onNullOverlap += OnNullResult.Handle;
        }
    }
}