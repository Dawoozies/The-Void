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
        public HangedFrameSubObject rightArm;
        public HangedFrameSubObject rightHand;
        public HangedFrame(string id) : base(id)
        {
            managedStart += ManagedStart;
            head = new HangedFrameSubObject("HangedFrameHead");
            GameManager.ins.allRuntimeObjects.Add("HangedFrameHead", head);
            RuntimeAnimator.CreateAndAttach(head, GameManager.ins.allControllers["HangedFrameHead"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(head);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(head);
            RuntimeDirectedPoints.CreateAndAttach(head);
            torso = new HangedFrameSubObject("HangedFrameTorso");
            GameManager.ins.allRuntimeObjects.Add("HangedFrameTorso", torso);
            RuntimeAnimator.CreateAndAttach(torso, GameManager.ins.allControllers["HangedFrameTorso"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(torso);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(torso);
            RuntimeDirectedPoints.CreateAndAttach(torso);
            leftArm = new HangedFrameSubObject("HangedFrameLeftArm");
            GameManager.ins.allRuntimeObjects.Add("HangedFrameLeftArm", leftArm);
            RuntimeAnimator.CreateAndAttach(leftArm, GameManager.ins.allControllers["HangedFrameLeftArm"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(leftArm);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(leftArm);
            RuntimeDirectedPoints.CreateAndAttach(leftArm);
            leftHand = new HangedFrameSubObject("HangedFrameLeftHand");
            GameManager.ins.allRuntimeObjects.Add("HangedFrameLeftHand", leftHand);
            RuntimeAnimator.CreateAndAttach(leftHand, GameManager.ins.allControllers["HangedFrameLeftHand"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(leftHand);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(leftHand);
            RuntimeDirectedPoints.CreateAndAttach(leftHand);
            rightArm = new HangedFrameSubObject("HangedFrameRightArm");
            GameManager.ins.allRuntimeObjects.Add("HangedFrameRightArm", rightArm);
            RuntimeAnimator.CreateAndAttach(rightArm, GameManager.ins.allControllers["HangedFrameRightArm"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(rightArm);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(rightArm);
            RuntimeDirectedPoints.CreateAndAttach(rightArm);
            rightHand = new HangedFrameSubObject("HangedFrameRightHand");
            GameManager.ins.allRuntimeObjects.Add("HangedFrameRightHand", rightHand);
            RuntimeAnimator.CreateAndAttach(rightHand, GameManager.ins.allControllers["HangedFrameRightHand"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(rightHand);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(rightHand);
            RuntimeDirectedPoints.CreateAndAttach(rightHand);

            torso.obj.SetParent(obj);
            head.obj.SetParent(torso.obj);
            leftArm.obj.SetParent(torso.obj);
            leftHand.obj.SetParent(leftArm.obj);
            rightArm.obj.SetParent(torso.obj);
            rightHand.obj.SetParent(rightArm.obj);
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