using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using ExtensionMethods_AnimatorController;
using ExtensionMethods_Animator;
using System;
using GeometryDefinitions;
using ExtensionMethods_Bool;
using System.Linq;

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
        public AnimatorController hangedFrameTorsoController;
        public AnimatorController hangedFrameLeftArmController;
        public AnimatorController hangedFrameRightArmController;
        public AnimatorController hangedFrameHeadController;
        //
        //Test managed things
        public Player playerObj;
        public List<Weapon> weapons = new List<Weapon>();
        public Weapon weapon;
        //Enemies
        public HangedFrame hangedFrame;
        public HangedFrame_Torso hangedFrameTorsoObj;
        public HangedFrame_LeftArm hangedFrameLeftArmObj;
        public HangedFrame_RightArm hangedFrameRightArmObj;
        public HangedFrame_Head hangedFrameHeadObj;
        public Material ropeMaterial;
        //ColliderBanks
        public ComponentBank<CircleCollider2D> circleCollider2DBank;
        public OverlapManager overlapManager;
        public OverlapInteractions overlapInteractions;
        public Vector3 gravityDirection;
        public Vector3 centerOfCamera;
        public Sprite spriteForMask;
        public Dictionary<CircleCollider2D, SpriteMask> spriteMasks = new Dictionary<CircleCollider2D, SpriteMask>();
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
            List<CircleCollider2D> colliders = circleCollider2DBank.vault.ToList();
            for (int i = 0; i < colliders.Count; i++)
            {
                GameObject maskObject = new GameObject($"{typeof(SpriteMask).Name.ToUpper()}_{i}");
                maskObject.transform.parent = colliders[i].transform;
                SpriteMask spriteMask = maskObject.AddComponent<SpriteMask>();
                spriteMask.sprite = spriteForMask;
                spriteMask.isCustomRangeActive = true;
                spriteMask.frontSortingLayerID = SortingLayer.NameToID("EmbeddedWeapon");
                spriteMask.frontSortingOrder = 11;
                spriteMask.backSortingLayerID = spriteMask.frontSortingLayerID;
                spriteMasks.Add(colliders[i], spriteMask);
            }
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
                Weapon weapon = new Weapon();
                weapon.Initialize(RuntimeIdentifier.Weapon);
                weapon.Init_Animator(halberdController);
                weapon.Init_Rigidbody2D();
                weapon.ManagedStart();
                weapons.Add(weapon);
                weapon.rb.position = new Vector3(-10f + i* 3f, 0f, 0f);
            }

            hangedFrame = new HangedFrame();
            hangedFrame.Initialize(RuntimeIdentifier.HangedFrame);
            hangedFrameTorsoObj = new HangedFrame_Torso();
            hangedFrameTorsoObj.Initialize(RuntimeIdentifier.HangedFrame);
            hangedFrameTorsoObj.Init_Animator(hangedFrameTorsoController);
            hangedFrameTorsoObj.ManagedStart();
            hangedFrameLeftArmObj = new HangedFrame_LeftArm();
            hangedFrameLeftArmObj.Initialize(RuntimeIdentifier.HangedFrameLeftHand);
            hangedFrameLeftArmObj.Init_Animator(hangedFrameLeftArmController);
            hangedFrameLeftArmObj.ManagedStart();
            hangedFrameRightArmObj = new HangedFrame_RightArm();
            hangedFrameRightArmObj.Initialize(RuntimeIdentifier.HangedFrameRightHand);
            hangedFrameRightArmObj.Init_Animator(hangedFrameRightArmController);
            hangedFrameRightArmObj.ManagedStart();
            hangedFrameHeadObj = new HangedFrame_Head();
            hangedFrameHeadObj.Initialize(RuntimeIdentifier.HangedFrame);
            hangedFrameHeadObj.Init_Animator(hangedFrameHeadController);
            hangedFrameHeadObj.ManagedStart();

            hangedFrame.lineRenderer = hangedFrame.transform.gameObject.AddComponent<LineRenderer>();
            hangedFrame.lineRenderer.material = ropeMaterial;
            hangedFrame.lineRenderer.textureMode = LineTextureMode.Tile;
            hangedFrame.lineRenderer.sortingLayerName = "EmbeddedWeapon";

            hangedFrame.transform.position = new Vector3(16, 8, 0);
            hangedFrame.torsoObj = hangedFrameTorsoObj;
            hangedFrame.leftArmObj = hangedFrameLeftArmObj;
            hangedFrame.rightArmObj = hangedFrameRightArmObj;
            hangedFrame.headObj = hangedFrameHeadObj;
            hangedFrame.ManagedStart();
        }
        private void Update()
        {
            InputManager.ins.ManagedUpdate(Time.deltaTime);
            playerObj.ManagedUpdate(Time.deltaTime);
            for (int i = 0; i < weapons.Count; i++)
            {
                weapons[i].ManagedUpdate(Time.deltaTime);
            }
            hangedFrame.ManagedUpdate(Time.deltaTime);
            overlapManager.ManagedUpdate(Time.deltaTime);
            overlapInteractions.ManagedUpdate();
            //Do all weapons after player
            ComponentDebugging.ins.ManagedUpdate();
        }
        private void FixedUpdate()
        {
            playerObj.ManagedFixedUpdate(Time.fixedDeltaTime);
            for (int i = 0; i < weapons.Count; i++)
            {
                weapons[i].ManagedFixedUpdate(Time.fixedDeltaTime);
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
        public int previousStateHash;
        public AnimatorStateInfo animatorStateInfo;
        public AnimatorClipInfo animatorClipInfo;
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
            animatorClipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];
            if (stateHash != animatorStateInfo.shortNameHash)
            {
                previousStateHash = stateHash;
                Debug.Log(previousStateHash);
                stateHash = animatorStateInfo.shortNameHash;
                animator.SetFloat("NormalizedTime", 0f);
                time = 0f;
                frame = 0;
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
            bool looping = animatorClipInfo.clip.isLooping;
            if(looping)
            {
                if(frame >= animator.TotalFrames())
                    time = 0f;
            }
            else
            {
                if(frame >= animator.TotalFrames() + 1)
                    time = animator.TotalFrames();
            }
        }
        public void RigidbodyUpdate(float tickDelta)
        {

        }
    }
    public enum RuntimeIdentifier
    {
        Null = 0, //Null objects may be important as they can be instanced but be initialized later
        Player = 1,
        Projectile = 2,
        Weapon = 3,
        HangedFrame = 4,
        HangedFrameRightHand = 5,
        HangedFrameLeftHand = 6,
        HangedFrameNoose = 7, 
    }
    public enum MainWeaponID
    {
        Null = 0,
        Halberd = 1,
        Spear = 2,
        TwinSpears = 3,
    }
}