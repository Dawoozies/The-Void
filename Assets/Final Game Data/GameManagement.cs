using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using ExtensionMethods_AnimatorController;
using ExtensionMethods_Animator;
using System;
using GeometryDefinitions;
using ExtensionMethods_Bool;
using UnityEditor.Experimental.GraphView;

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
        public List<Halberd> halberds = new List<Halberd>();
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
            circleCollider2DBank.Initialize("MAIN", 100);
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

            for (int i = 0; i < 8; i++)
            {
                Halberd halberd = new Halberd();
                halberd.Initialize(RuntimeIdentifier.Halberd);
                halberd.Init_Animator(halberdController);
                halberd.Init_Rigidbody2D();
                halberd.ManagedStart();
                halberds.Add(halberd);
                halberd.rb.position = new Vector3(-10f + i* 3f, 0f, 0f);
            }
        }
        private void Update()
        {
            InputManager.ins.ManagedUpdate(Time.deltaTime);
            playerObj.ManagedUpdate(Time.deltaTime);
            overlapManager.ManagedUpdate(Time.deltaTime);
            overlapInteractions.ManagedUpdate();
            //Do all weapons after player
            foreach (Halberd halberd in halberds)
            {
                halberd.ManagedUpdate(Time.deltaTime);
            }
            ComponentDebugging.ins.ManagedUpdate();
        }
        private void FixedUpdate()
        {
            playerObj.ManagedFixedUpdate(Time.fixedDeltaTime);
            foreach (Halberd halberd in halberds)
            {
                halberd.ManagedFixedUpdate(Time.fixedDeltaTime);
            }
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
        public int stateHash_previous;
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
        public float objTickRate(float globalTickRate) => globalTickRate * localTickRateMultiplier;
        //By LocalPos its a Local --> World transform
        Vector3 LocalPosFromSpriteRenderer(Vector3 v) { return new Vector3(spriteRenderer.flipX.DefinedValue(1, -1) * v.x, spriteRenderer.flipY.DefinedValue(1, -1) * v.y, 0f); }
        public Vector3 LocalPosFromTransform(Vector3 v)
        {
            Vector3 u = LocalPosFromSpriteRenderer(v);
            return transform.position + transform.right * u.x + transform.up * u.y;
        }
        public Vector2 up => transform.up;
        public Vector2 right => transform.right;
        public float upSpeed => Vector3.Dot(rb.velocity, up);
        public float rightSpeed => Vector3.Dot(rb.velocity, right);
        public Vector2 upVelocity => upSpeed * up;
        public Vector2 rightVelocity => rightSpeed * right;
        public Vector2 dirTo(Vector3 pos) => (pos - rb.transform.position).normalized;
        public void AnimatorUpdate(float tickDelta)
        {
            animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateHash != animatorStateInfo.shortNameHash)
            {
                stateHash_previous = stateHash;
                stateHash = animatorStateInfo.shortNameHash;
                animator.SetFloat("NormalizedTime", 0f);
                time = 0f;
                animatorStateChanged?.Invoke();
                animatorFrameChanged?.Invoke();
            }
            time += objTickRate(tickDelta) * animatorStateInfo.speed;
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