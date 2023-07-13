using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateHandlers.Mantis;
using OverlapHandlers.Mantis;
using Unity.VisualScripting;
using UnityEngine.WSA;

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
        public int stagger = 0;
        int staggerReduction = 1;
        float staggerReduceMaxTime = 0.5f;
        float staggerReduceTime = 0;
        float staggerCooldown = 0;
        public Mantis(string id) : base(id)
        {
            RuntimeAnimator.CreateAndAttach(this, GameManager.ins.allControllers["Mantis"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(this);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(this);
            RuntimeDirectedPoints.CreateAndAttach(this);

            legs = new MantisLegs();
            //GameManager.ins.allRuntimeObjects.Add(legs);

            torso = new MantisTorso();
            //GameManager.ins.allRuntimeObjects.Add(torso);

            leftArm = new MantisLeftArm();
            //GameManager.ins.allRuntimeObjects.Add(leftArm);

            managedUpdate += RuntimeAnimator.Update;
            managedUpdate += Handler.Update;
            animator.onStateEnter += Handler.OnStateEnter;
            animator.onFrameUpdate += Handler.OnFrameUpdate;
            animator.spriteRenderer.color = Color.clear;

            legs.obj.SetParent(obj);
            torso.obj.SetParent(obj);
            leftArm.obj.SetParent(torso.obj);

            managedUpdate += StaggerUpdate;
        }
        void StaggerUpdate(RuntimeObject obj, float tickDelta)
        {
            if (stagger > 0)
                staggerReduceTime -= tickDelta;
            if(staggerReduceTime < 0)
            {
                stagger -= staggerReduction;
                if(stagger <= 0)
                {
                    stagger = 0;
                    staggerReduceTime = 0;
                }
                else
                {
                    staggerReduceTime = staggerReduceMaxTime;
                }
            }
            if (staggerCooldown > 0)
                staggerCooldown -= tickDelta;
            UIManager.ins.OnStaggerValueChanged(stagger);
        }
        public void Stagger(int staggerAddAmount)
        {
            if(staggerCooldown <= 0)
            {
                stagger += staggerAddAmount;
                staggerCooldown = 0.25f;

                if(stagger >= 100)
                {
                    stagger = 100;
                }    
            }
            UIManager.ins.OnStaggerValueChanged(stagger);
        }
    }
    public class MantisLegs : RuntimeObject
    {
        public MantisLegs() : base("MantisLegs")
        {
            GameManager.ins.allRuntimeObjects.Add(this);
            RuntimeAnimator.CreateAndAttach(this, GameManager.ins.allControllers["MantisLegs"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(this);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(this);
            RuntimeDirectedPoints.CreateAndAttach(this);

            managedUpdate += RuntimeAnimator.Update;
            animator.onStateEnter += Handler.OnStateEnter;
            animator.onFrameUpdate += Handler.OnFrameUpdate;
            obj.localPosition = new Vector3(0, -11, 0);
            animator.spriteRenderer.sortingOrder = 4;
        }
    }
    public class MantisTorso : RuntimeObject
    {
        public MantisTorso() : base("MantisTorso")
        {
            GameManager.ins.allRuntimeObjects.Add(this);
            RuntimeAnimator.CreateAndAttach(this, GameManager.ins.allControllers["MantisTorso"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(this);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(this);
            RuntimeDirectedPoints.CreateAndAttach(this);

            managedUpdate += RuntimeAnimator.Update;
            animator.onStateEnter += Handler.OnStateEnter;
            animator.onFrameUpdate += Handler.OnFrameUpdate;
            animator.spriteRenderer.sortingOrder = 5;
        }
    }
    public class MantisLeftArm : RuntimeObject
    {
        public MantisLeftArm() : base("MantisLeftArm")
        {
            GameManager.ins.allRuntimeObjects.Add(this);
            RuntimeAnimator.CreateAndAttach(this, GameManager.ins.allControllers["MantisLeftArm"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(this);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(this);
            RuntimeDirectedPoints.CreateAndAttach(this);

            managedUpdate += RuntimeAnimator.Update;
            managedUpdate += RuntimeDirectedCircleOverlaps.Update;
            directedCircleOverlaps.onRuntimeObjectOverlap += OnRuntimeObjectOverlap.Handle;
            animator.spriteRenderer.sortingOrder = 6;
        }
    }
}