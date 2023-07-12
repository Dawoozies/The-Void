using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateHandlers.Weapon;
using OverlapHandlers.Weapon;

namespace RuntimeObjects
{
    public enum WeaponHeadSpriteType
    {
        Spear = 0,
        Halberd = 1,
    }
    public enum WeaponShaftSpriteType
    {
        Long = 0,
    }
    public enum WeaponPommelSpriteType
    {
        Default = 0,
    }
    public class Weapon : RuntimeObject
    {
        public WeaponHead head;
        public WeaponShaft shaft;
        public WeaponPommel pommel;
        public RuntimeObject owner;
        public bool useBlur;
        const float BLUR_BUFFER_TIME = 0.14f;
        float blurBufferTime;
        int anchorSlot = 0;
        public Weapon(string id, WeaponHeadSpriteType headSpriteType, WeaponShaftSpriteType shaftSpriteType, WeaponPommelSpriteType pommelSpriteType) : base(id)
        {
            RuntimeAnimator.CreateAndAttach(this, GameManager.ins.allControllers["Weapon"]);
            RuntimeRigidbody.CreateAndAttach(this);
            head = new WeaponHead();
            //GameManager.ins.allRuntimeObjects.Add("WeaponHead", head);

            shaft = new WeaponShaft();
            //GameManager.ins.allRuntimeObjects.Add("WeaponShaft", shaft);

            pommel = new WeaponPommel();
            //GameManager.ins.allRuntimeObjects.Add("WeaponPommel", pommel);

            head.obj.SetParent(obj);
            head.obj.localPosition = new Vector3(0, 1.35f, 0);
            shaft.obj.SetParent(obj);
            pommel.obj.SetParent(obj);
            pommel.obj.localPosition = new Vector3(0, -1.35f, 0);

            managedUpdate += RuntimeAnimator.Update;
            managedUpdate += Handler.Update;
            managedUpdate += BlurBuffer;
            managedUpdate += Update_SubObjects;
            animator.onStateEnter += Handler.OnStateEnter;
            animator.onFrameUpdate += Handler.OnFrameUpdate;


            head.animator.spriteRenderer.sortingOrder = 12;
            shaft.animator.spriteRenderer.sortingOrder = 13;
            pommel.animator.spriteRenderer.sortingOrder = 12;

            animator.spriteRenderer.color = Color.clear;

            RuntimePlayerWeapon.onWeaponsListChanged += OnWeaponsListChanged;
        }
        public void SetOwner(RuntimeObject owner)
        {
            this.owner = owner;
            owner.managedUpdate += AnchorUpdate;

            PlayerTorso playerTorso = owner as PlayerTorso;
            if(playerTorso != null)
            {
                StateHandlers.Player.Handler.onAttackBlur += OnTorsoBlur;
            }
        }
        public void RemoveOwner()
        {
            owner.managedUpdate -= AnchorUpdate;
            owner = null;

            PlayerTorso playerTorso = owner as PlayerTorso;
            if (playerTorso != null)
            {
                StateHandlers.Player.Handler.onAttackBlur -= OnTorsoBlur;
            }
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
                if (owner.animator.spriteRenderer.flipX)
                {
                    head.animator.spriteRenderer.flipX = true;
                }
                else
                {
                    head.animator.spriteRenderer.flipX = false;
                }
            }
        }
        public void Throw(Vector2 velocity)
        {
            RemoveOwner();
            rigidbody.rb.velocity = velocity;
            rigidbody.rbObj.up = velocity;
            if (velocity.x < 0)
                head.animator.spriteRenderer.flipX = true;
            else
                head.animator.spriteRenderer.flipX = false;
        }
        void OnTorsoBlur()
        {
            useBlur = true;
            blurBufferTime = BLUR_BUFFER_TIME;
        }
        void BlurBuffer(RuntimeObject obj, float tickDelta)
        {
            if (useBlur)
                blurBufferTime -= tickDelta;
            if(blurBufferTime < 0f)
            {
                useBlur = false;
                blurBufferTime = 0f;
            }
        }
        void Update_SubObjects(RuntimeObject obj, float tickDelta)
        {
            head.managedUpdate?.Invoke(head, tickDelta);
            shaft.managedUpdate?.Invoke(shaft, tickDelta);
            pommel.managedUpdate?.Invoke(pommel, tickDelta);
        }
        void OnWeaponsListChanged(List<Weapon> playerWeaponsList)
        {
            if(playerWeaponsList.Contains(this))
            {
                int weaponIndex = playerWeaponsList.IndexOf(this);
                if(weaponIndex >= 1)
                {
                    Hide();
                }
                else
                {
                    //Then weapon can be held in hand
                    anchorSlot = weaponIndex;
                    Show();
                }
            }
        }
        void Hide()
        {
            head.animator.spriteRenderer.color = Color.clear;
            shaft.animator.spriteRenderer.color = Color.clear;
            pommel.animator.spriteRenderer.color = Color.clear;
        }
        void Show()
        {
            head.animator.spriteRenderer.color = Color.white;
            shaft.animator.spriteRenderer.color = Color.white;
            pommel.animator.spriteRenderer.color = Color.white;
        }
    }
    public class WeaponHead : RuntimeObject
    {
        public WeaponHeadSpriteType spriteType = WeaponHeadSpriteType.Spear;
        public WeaponHead() : base("WeaponHead")
        {
            RuntimeAnimator.CreateAndAttach(this, GameManager.ins.allControllers["WeaponHead"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(this);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(this);
            RuntimeDirectedPoints.CreateAndAttach(this);

            managedUpdate += RuntimeAnimator.Update;
            animator.onStateEnter += Handler.OnStateEnter;
            animator.onFrameUpdate += Handler.OnFrameUpdate;

            managedUpdate += RuntimeDirectedCircleOverlaps.Update;
            directedCircleOverlaps.onRuntimeObjectOverlap += OnRuntimeObjectOverlap.Handle;
            directedCircleOverlaps.onNonRuntimeObjectOverlap += OnNonRuntimeObjectOverlap.Handle;
        }
        public void SetSpriteType(WeaponHeadSpriteType type)
        {
            spriteType = type;
        }
    }
    public class WeaponShaft : RuntimeObject
    {
        public WeaponShaftSpriteType spriteType = WeaponShaftSpriteType.Long;
        public WeaponShaft() : base("WeaponShaft")
        {
            RuntimeAnimator.CreateAndAttach(this, GameManager.ins.allControllers["WeaponShaft"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(this);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(this);
            RuntimeDirectedPoints.CreateAndAttach(this);

            managedUpdate += RuntimeAnimator.Update;
            animator.onStateEnter += Handler.OnStateEnter;
            animator.onFrameUpdate += Handler.OnFrameUpdate;
        }
        public void SetSpriteType(WeaponShaftSpriteType type)
        {
            spriteType = type;
        }
    }
    public class WeaponPommel : RuntimeObject
    {
        public WeaponPommelSpriteType spriteType = WeaponPommelSpriteType.Default;
        public WeaponPommel() : base("WeaponPommel")
        {
            RuntimeAnimator.CreateAndAttach(this, GameManager.ins.allControllers["WeaponPommel"]);
            RuntimeDirectedCircleColliders.CreateAndAttach(this);
            RuntimeDirectedCircleOverlaps.CreateAndAttach(this);
            RuntimeDirectedPoints.CreateAndAttach(this);

            managedUpdate += RuntimeAnimator.Update;
            animator.onStateEnter += Handler.OnStateEnter;
            animator.onFrameUpdate += Handler.OnFrameUpdate;
        }
        public void SetSpriteType(WeaponPommelSpriteType type)
        {
            spriteType = type;
        }
    }
}