using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateHandlers.Mantis;
using OverlapHandlers.Mantis;
using System;
using RuntimeContainers;
namespace RuntimeObjects
{
    public class Mantis : RuntimeObject
    {
        public MantisLegs legs;
        public MantisTorso torso;
        public MantisLeftArm leftArm;
        public SpriteRenderer background;
        public float aimTime = 0.26667f;
        public float linearStrikeWaitTime = 0.25f;
        public float retractTime = 0.14f;
        public bool useLinearStrikeSpin = true;
        public string strikeDirection = "Side";
        int maxStagger = 100;
        int stagger = 0;
        public Action<int, int> onStaggerValueChanged;
        public Action<int> onDamaged;
        public Action onPostureBreak;
        public int postureRecoveryAmount = 1;
        public TimedAction postureRecovery = new(timeMax: 0.75f, loop: true);
        public TimedAction damageCooldown = new(timeMax: 0.25f, loop: false);
        public Mantis(string id) : base(id)
        {
            RuntimeAnimator.CreateAndAttach(this, GameManager.ins.allControllers["Mantis"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(this);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(this);
            RuntimeDirectedPoints.CreateAndAttach(this);

            legs = new MantisLegs();
            torso = new MantisTorso();
            leftArm = new MantisLeftArm();

            background = GameManager.ins.spriteRendererPool.Get();
            background.sprite = GameManager.ins.FindSpriteByID("MantisBackground1");
            background.sortingOrder = -1;
            background.material = GameManager.ins.FindMaterialByID("NoOverrideColor");
            background.color = new Color(0.04f, 0f, 0.02f, 1f);

            managedUpdate += RuntimeAnimator.Update;
            managedUpdate += Handler.Update;
            animator.onStateEnter += Handler.OnStateEnter;
            animator.onFrameUpdate += Handler.OnFrameUpdate;
            animator.onTrueFrameUpdate += Handler.OnTrueFrameUpdate;
            animator.spriteRenderer.color = Color.clear;

            legs.obj.SetParent(obj);
            torso.obj.SetParent(obj);
            leftArm.obj.SetParent(torso.obj);

            //managedUpdate += StaggerUpdate;
            managedUpdate += postureRecovery.Update;
            managedUpdate += damageCooldown.Update;
            onStaggerValueChanged += UIManager.ins.OnStaggerValueChanged;
            postureRecovery.onTimerEnd += PostureRecovery;
            damageCooldown.onTimerEnd += OnDamageCooldownEnd;
            onPostureBreak += Handler.OnPostureBreak;
            onDamaged += OnDamaged;

        }
        public void ApplyDamage(int damage)
        {
            if(!damageCooldown.isActive)
            {
                onDamaged?.Invoke(damage);
                //right now damage just increases stagger
                //weapons will have varied damage types etc eventually
                IncreaseStagger(damage);
                damageCooldown.Activate();
            }
        }
        void IncreaseStagger(int valueToAdd)
        {
            //(previous value, new value)
            int newStaggerValue = stagger + valueToAdd;
            if(newStaggerValue >= maxStagger)
            {
                onPostureBreak?.Invoke();
                newStaggerValue = maxStagger;
            }
            onStaggerValueChanged?.Invoke(stagger, newStaggerValue); 
            stagger = newStaggerValue;
        }
        void PostureRecovery()
        {
            onStaggerValueChanged?.Invoke(stagger, stagger - postureRecoveryAmount);
            stagger -= postureRecoveryAmount;
        }
        void OnDamaged(int damage)
        {
            background.material = GameManager.ins.FindMaterialByID("BackgroundGlitch");
        }
        void OnDamageCooldownEnd()
        {
            background.material = GameManager.ins.FindMaterialByID("NoOverrideColor");
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