using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateHandlers.Mantis;
using OverlapHandlers.Mantis;

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
        float staggerReduceMaxTime = 0.75f;
        float staggerReduceTime = 0;
        float staggerCooldown = 0;
        public Mantis(string id) : base(id)
        {
            RuntimeAnimator.CreateAndAttach(this, GameManager.ins.allControllers["Mantis"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(this);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(this);
            RuntimeDirectedPoints.CreateAndAttach(this);

            legs = new MantisLegs();
            torso = new MantisTorso();
            leftArm = new MantisLeftArm();
            Material noOverrideColorMaterial = GameManager.ins.FindMaterialByID("NoOverrideColor");
            Material overrideColorMaterial = GameManager.ins.FindMaterialByID("OverrideColor");
            if(noOverrideColorMaterial != null && overrideColorMaterial != null)
            {
                legs.defaultMaterial = noOverrideColorMaterial;
                legs.overrideMaterial = overrideColorMaterial;
                torso.defaultMaterial = noOverrideColorMaterial;
                torso.overrideMaterial = overrideColorMaterial;
                leftArm.defaultMaterial = noOverrideColorMaterial;
                leftArm.overrideMaterial = overrideColorMaterial;
            }

            managedUpdate += RuntimeAnimator.Update;
            managedUpdate += Handler.Update;
            animator.onStateEnter += Handler.OnStateEnter;
            animator.onFrameUpdate += Handler.OnFrameUpdate;
            animator.onTrueFrameUpdate += Handler.OnTrueFrameUpdate;
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
            if(animator.CurrentState("Mantis_StunForward"))
                return;
            if (staggerCooldown <= 0)
            {
                stagger += staggerAddAmount;
                staggerCooldown = 0.25f;

                if(stagger >= 100)
                {
                    //reset stagger and then do knockdown
                    stagger = 0;
                    animator.animator.Play("Mantis_StunForward");
                    UIManager.ins.OnStaggerValueChanged(stagger);
                    return;
                }    
            }
            UIManager.ins.OnStaggerValueChanged(stagger);
        }
    }
    public class MantisLegs : RuntimeObject
    {
        public Material defaultMaterial;
        public Material overrideMaterial;
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
        public Material defaultMaterial;
        public Material overrideMaterial;
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
        public Material defaultMaterial;
        public Material overrideMaterial;
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