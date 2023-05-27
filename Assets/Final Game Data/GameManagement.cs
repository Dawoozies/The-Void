using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using ExtensionMethods_AnimatorController;
using ExtensionMethods_Animator;
using System;
using GeometryDefinitions;
using ExtensionMethods_Bool;
namespace GameManagement
{
    public class GameManagement : MonoBehaviour
    {
        public static GameManagement ins;
        public AnimatorController[] controllers;
        public Cache<Component_CircleCollider2D> circleCollider2DCache; //Only make one instance
        public Cache<Component_Overlap> overlapCache; //Only one instance
        public AnimatorController playerController;
        public AnimatorController halberdController;
        //
        //Test managed things
        public Player playerObj;
        public Halberd halberdObj;
        //ColliderBanks
        public ComponentBank<CircleCollider2D> circleCollider2DBank;
        public OverlapManager overlapManager;
        public OverlapInteractions overlapInteractions;
        public Vector3 gravityDirection;
        private void Awake()
        {
            ins = this;
            //Here is where we want to load in all ControllerData and then Cache it
            circleCollider2DCache = new Cache<Component_CircleCollider2D>();
            circleCollider2DCache.SetupRuntimeCache(controllers);
            overlapCache = new Cache<Component_Overlap>();
            overlapCache.SetupRuntimeCache(controllers);
            circleCollider2DBank = new ComponentBank<CircleCollider2D>();
            circleCollider2DBank.Initialize("MAIN", 50);
            //Debugging stuff
            ComponentDebugging.ins.circleColliders = circleCollider2DBank.GetVaultObjects();
            //Overlap stuff
            overlapManager = new OverlapManager();
            overlapManager.Initialize();
            overlapInteractions = new OverlapInteractions();
            overlapInteractions.Initialize();
        }
        private void Start()
        {
            playerObj = new Player();
            playerObj.Initialize(RuntimeIdentifier.Player);
            playerObj.Init_Animator(playerController);
            playerObj.Init_Rigidbody2D();
            playerObj.ManagedStart();

            halberdObj = new Halberd();
            halberdObj.Initialize(RuntimeIdentifier.Halberd);
            halberdObj.Init_Animator(halberdController);
            halberdObj.Init_Rigidbody2D();
            halberdObj.ManagedStart();
        }
        private void Update()
        {
            InputManager.ins.ManagedUpdate(Time.deltaTime);
            playerObj.ManagedUpdate(Time.deltaTime);
            overlapManager.ManagedUpdate(Time.deltaTime);
            overlapInteractions.ManagedUpdate();
            //Do all weapons after player
            halberdObj.ManagedUpdate(Time.deltaTime);
            ComponentDebugging.ins.ManagedUpdate();
        }
        private void FixedUpdate()
        {
            playerObj.ManagedFixedUpdate(Time.fixedDeltaTime);
            halberdObj.ManagedFixedUpdate(Time.fixedDeltaTime);
        }
    }
    public class RuntimeSceneObject
    {
        public RuntimeIdentifier ID;
        public GameObject obj;
        public Transform transform;
        //
        public GameObject rbObj;
        public Transform rbTransform;
        public Transform rbColliderParent;
        public Rigidbody2D rb;
        //
        public Animator animator;
        public AnimatorController controller;
        public SpriteRenderer spriteRenderer;
        public int stateHash;
        public AnimatorStateInfo animatorStateInfo;
        public float localTickRateMultiplier = 1f;
        public float time;
        public float normalizedTime;
        public int frame;
        public Action animatorStateChanged;
        public Action animatorFrameChanged;
        public void Initialize(RuntimeIdentifier ID)
        {
            this.ID = ID;
            obj = new GameObject($"RuntimeID:{ID}");
            transform = obj.transform;
        }
        public void Init_Rigidbody2D()
        {
            rbObj = new GameObject($"Rigidbody2D:{ID}");
            rbTransform = rbObj.transform;
            //Set main transform to be child of rb transform
            transform.parent = rbTransform;
            rb = rbObj.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.drag = 0;
            rb.angularDrag = 0;
            rb.freezeRotation = true;
            rbColliderParent = new GameObject($"ColliderParent:{ID}").transform;
            rbColliderParent.parent = rbTransform;
        }
        public void Init_Animator(AnimatorController controller)
        {
            spriteRenderer = obj.AddComponent<SpriteRenderer>();
            animator = obj.AddComponent<Animator>();
            this.controller = controller;
            animator.runtimeAnimatorController = controller as RuntimeAnimatorController;
        }
        public float LocalTickRate(float tickRate) { return tickRate * localTickRateMultiplier; }
        //By LocalPos its a Local --> World transform
        Vector3 LocalPosFromSpriteRenderer(Vector3 v) { return new Vector3(spriteRenderer.flipX.DefinedValue(1, -1)*v.x, spriteRenderer.flipY.DefinedValue(1, -1)*v.y, 0f); }
        public Vector3 LocalPosFromTransform(Vector3 v)
        {
            Vector3 u = LocalPosFromSpriteRenderer(v);
            return transform.position + transform.right * u.x + transform.up * u.y;
        }
        public void AnimatorUpdate(float tickDelta)
        {
            animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateHash != animatorStateInfo.shortNameHash)
            {
                stateHash = animatorStateInfo.shortNameHash;
                animator.SetFloat("NormalizedTime", 0f);
                time = 0f;
                animatorStateChanged?.Invoke();
                animatorFrameChanged?.Invoke();
            }
            time += LocalTickRate(tickDelta) * animatorStateInfo.speed;
            normalizedTime = (float)Mathf.FloorToInt(time) / animator.TotalFrames();
            animator.SetFloat("NormalizedTime", normalizedTime);
            if (frame != Mathf.FloorToInt(time))
            {
                frame = Mathf.FloorToInt(time);
                animatorFrameChanged?.Invoke();
            }
            bool loop = animator.GetCurrentAnimatorClipInfo(0)[0].clip.isLooping;
            if (time > animator.TotalFrames())
            {
                if (loop)
                {
                    time = 0f;
                }
                else
                {
                    time = animator.TotalFrames();
                    frame = Mathf.FloorToInt(time);
                    animatorFrameChanged?.Invoke();
                }
            }
        }
    }
    public class Player : RuntimeSceneObject
    {
        public Component_CircleCollider2D circleCollider2DComponent;
        public Component_Overlap overlapComponent;
        const int colliderRequestAmount = 18;
        bool hasColliders = false;
        public bool grounded = true;
        bool jump => InputManager.ins.JumpDown_Input || InputManager.ins.JumpDown_BufferedInput;
        float L_Move => 15f*InputManager.ins.L_Input.x;
        float VelocityUp => Vector3.Dot(rb.velocity, transform.up);
        float VelocityRight => Vector3.Dot(rb.velocity, transform.right);
        float fallSpeedMax = -20f;
        float ascentSpeedMax = 20f;
        public MainWeaponID mainWeaponID = MainWeaponID.Null;
        public bool recalling => (mainWeaponID == MainWeaponID.Null && InputManager.ins.LeftBumper_Input);
        public bool aimingWeapon => InputManager.ins.LeftTrigger_Input > 0.1f && mainWeaponID != MainWeaponID.Null;
        public void ManagedStart()
        {
            spriteRenderer.sortingOrder = 10;
            animatorStateChanged += () =>
            {
                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "StateSwap")
                    return;
                circleCollider2DComponent = GameManagement.ins.circleCollider2DCache.LoadComponent(controller.name, stateHash);
                overlapComponent = GameManagement.ins.overlapCache.LoadComponent(controller.name, stateHash);
            };
            animatorFrameChanged += () =>
            {
                if (circleCollider2DComponent != null && circleCollider2DComponent.componentData.Length > 0)
                {
                    Component_CircleCollider2D_Data[] circleCollider2DDataAtFrame = circleCollider2DComponent.DataWithFrame(frame);
                    UpdateLedger.CircleCollider2D(GameManagement.ins.circleCollider2DBank, this, circleCollider2DDataAtFrame);
                }
                else
                {
                    UpdateLedger.NullComponent(GameManagement.ins.circleCollider2DBank, this);
                }
                if (overlapComponent != null && overlapComponent.componentData.Length > 0)
                {
                    Component_Overlap_Data[] overlapComponentDataAtFrame = overlapComponent.DataWithFrame(frame);
                    if (overlapComponentDataAtFrame != null)
                    {
                        foreach (Component_Overlap_Data componentData in overlapComponentDataAtFrame)
                        {
                            GameManagement.ins.overlapManager.OverlapApply(this, componentData);
                        }
                    }
                }
            };
            hasColliders = GameManagement.ins.circleCollider2DBank.RequestLoanForRigidbody2D(this, colliderRequestAmount);
        }
        public void ManagedUpdate(float tickDelta)
        {
            AnimatorUpdate(tickDelta);
            animator.SetBool("Grounded", grounded);
            animator.SetBool("Jump", jump);
            animator.SetBool("Run", Mathf.Abs(InputManager.ins.L_Input.x) > 0);
            animator.SetFloat("VelocityUp", VelocityUp);
            animator.SetFloat("VelocityRight", VelocityRight);
            animator.SetInteger("MainWeaponID", (int)mainWeaponID);
            animator.SetBool("Recalling", recalling);
            animator.SetFloat("LeftTrigger_Input", InputManager.ins.LeftTrigger_Input);
            animator.SetInteger("R_Direction", Direction.Compute8WayDirection());
            animator.SetBool("RightBumper_Input", InputManager.ins.RightBumper_Input);
            if(aimingWeapon)
            {
                if (spriteRenderer.flipX && InputManager.ins.R_Input.x > 0)
                    spriteRenderer.flipX = false;
                if (!spriteRenderer.flipX && InputManager.ins.R_Input.x < 0)
                    spriteRenderer.flipX = true;
            }
            else
            {
                if (spriteRenderer.flipX && InputManager.ins.L_Input.x > 0)
                    spriteRenderer.flipX = false;
                if (!spriteRenderer.flipX && InputManager.ins.L_Input.x < 0)
                    spriteRenderer.flipX = true;
            }
        }
        public void ManagedFixedUpdate(float tickDelta)
        {
            rb.velocity = L_Move*transform.right + VelocityUp * transform.up;
            if(Animator.StringToHash("FALL") == animatorStateInfo.shortNameHash)
            {
                rb.AddForce(-transform.up*20f);
                if(VelocityUp < fallSpeedMax)
                {
                    rb.velocity = VelocityRight * transform.right + fallSpeedMax * transform.up;
                    Debug.Log("Hit fallSpeedMax");
                }
            }
            if(Animator.StringToHash("JUMP") == animatorStateInfo.shortNameHash)
            {
                rb.velocity += (Vector2)transform.up*5f;
                if(VelocityUp > ascentSpeedMax)
                {
                    rb.velocity = VelocityRight * transform.right + ascentSpeedMax * transform.up;
                }
            }
            if(Animator.StringToHash("ASCENTSLOW") == animatorStateInfo.shortNameHash)
            {
                rb.AddForce(-transform.up * 80f);
                if (VelocityUp < fallSpeedMax)
                {
                    rb.velocity = VelocityRight * transform.right + fallSpeedMax * transform.up;
                }
            }
            if(Animator.StringToHash("RECALL_AIR") == animatorStateInfo.shortNameHash)
            {
                if(VelocityUp > 3)
                {
                    rb.AddForce(-transform.up * 70f);
                }
                if(VelocityUp > 0 && VelocityUp <= 3)
                {
                    //Low grav
                    rb.AddForce(-transform.up * 10f);
                }
                if(VelocityUp <= 0)
                {
                    rb.AddForce(-transform.up * 20f);
                }
                if(VelocityUp < fallSpeedMax)
                {
                    rb.velocity = VelocityRight * transform.right + fallSpeedMax * transform.up;
                }
            }
        }
    }
    public enum RuntimeIdentifier
    {
        Null = 0, //Null objects may be important as they can be instanced but be initialized later
        Player = 1,
        Projectile = 2,
        Halberd = 3,
        Spear = 4,
        TwinSpearLeft = 5,
        TwinSpearRight = 6,
    }
    public enum MainWeaponID
    {
        Null = 0,
        Halberd = 1,
        Spear = 2,
        TwinSpears = 3,
    }
}