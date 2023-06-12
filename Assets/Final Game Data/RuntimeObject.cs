using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using ExtensionMethods_Bool;
namespace RuntimeObjects
{
    [Flags]
    public enum RuntimeObjectStructure
    {
        Default = 0,
        Animator = 1,
        Rigidbody = 2,
        DirectedCircleCollider = 4,
        DirectedCircleOverlap = 8,
        CircleSpriteMask = 16,
        DirectedPoint = 32,
    }
    public class RuntimeObject
    {
        public string id;
        public GameObject gameObject;
        public Transform obj;
        public float localTickRateMultiplier;
        public Action managedStart;
        public Action<float> managedUpdate;
        public RuntimeObjectStructure objStructure;
        public RuntimeAnimator animator;
        public RuntimeRigidbody rigidbody;
        public RuntimeDirectedCircleColliders directedCircleColliders;
        public RuntimeDirectedCircleOverlaps directeCircleOverlaps;
        public float TickRate(float globalTickRate) => globalTickRate * localTickRateMultiplier;
        public Vector2 up => obj.up;
        public Vector2 right => obj.right;
        public Vector2 RelativePos(Vector2 v)
        {
            return (Vector2)obj.position + animator.spriteRenderer.flipX.DefinedValue(1, -1) * v.x * right
                + animator.spriteRenderer.flipY.DefinedValue(1, -1) * v.y * up;
        }
        public RuntimeObject(string id)
        {
            this.id = id;
            gameObject = new GameObject($"RuntimeObject:{id}");
            obj = gameObject.transform;
            localTickRateMultiplier = 1f;
        }
    }
    public class RuntimeAnimator
    {
        public Animator animator;
        public SpriteRenderer spriteRenderer;
        private AnimatorStateInfo StateInfo => animator.GetCurrentAnimatorStateInfo(0);
        private AnimatorClipInfo ClipInfo => animator.GetCurrentAnimatorClipInfo(0)[0];
        public string controllerName => animator.runtimeAnimatorController.name;
        public int stateHash;
        public int previousStateHash;
        public float time;
        public int frame;
        public float normalizedTime;
        public Action<RuntimeObject, int> onStateEnter;
        public Action<RuntimeObject, int> onFrameUpdate;
        public Action<RuntimeObject, ControllerData, int> onStateEnterData;
        public Action<RuntimeObject, ControllerData, int> onFrameUpdateData;
        public static void CreateAndAttach(RuntimeObject obj, RuntimeAnimatorController controller)
        {
            RuntimeAnimator runtimeAnimator = new RuntimeAnimator();
            obj.objStructure |= RuntimeObjectStructure.Animator;
            runtimeAnimator.spriteRenderer = obj.gameObject.AddComponent<SpriteRenderer>();
            runtimeAnimator.animator = obj.gameObject.AddComponent<Animator>();
            runtimeAnimator.animator.runtimeAnimatorController = controller;
            obj.animator = runtimeAnimator;
        }
        public static void Update(RuntimeObject obj, float tickDelta)
        {
            RuntimeAnimator runtimeAnimator = obj.animator;
            if (runtimeAnimator.stateHash != runtimeAnimator.StateInfo.shortNameHash)
            {
                //If there is some collision issue in the future please check to see
                //If it is an abuse of state swapping causing it
                if (runtimeAnimator.ClipInfo.clip.name == "StateSwap")
                    return;
                Debug.Log($"Swapping to {runtimeAnimator.ClipInfo.clip.name}");
                runtimeAnimator.previousStateHash = runtimeAnimator.stateHash;
                runtimeAnimator.stateHash = runtimeAnimator.StateInfo.shortNameHash;
                runtimeAnimator.animator.SetFloat("NormalizedTime", 0f);
                runtimeAnimator.time = 0;
                runtimeAnimator.frame = 0;
                runtimeAnimator.onStateEnterData?.Invoke(obj, GameManager.ins.allControllerData[runtimeAnimator.controllerName], runtimeAnimator.stateHash);
                runtimeAnimator.onFrameUpdateData?.Invoke(obj, GameManager.ins.allControllerData[runtimeAnimator.controllerName], runtimeAnimator.frame);
                runtimeAnimator.onStateEnter?.Invoke(obj, runtimeAnimator.stateHash);
                runtimeAnimator.onFrameUpdate?.Invoke(obj, runtimeAnimator.frame);
            }
            runtimeAnimator.time += obj.TickRate(tickDelta) * runtimeAnimator.StateInfo.speed;
            runtimeAnimator.normalizedTime = (float)Mathf.FloorToInt(runtimeAnimator.time) / runtimeAnimator.ClipInfo.clip.length;
            runtimeAnimator.animator.SetFloat("NormalizedTime", runtimeAnimator.normalizedTime);
            if (runtimeAnimator.frame != Mathf.FloorToInt(runtimeAnimator.time))
            {
                runtimeAnimator.frame = Mathf.FloorToInt(runtimeAnimator.time);
                runtimeAnimator.onFrameUpdateData?.Invoke(obj, GameManager.ins.allControllerData[runtimeAnimator.controllerName], runtimeAnimator.frame);
                runtimeAnimator.onFrameUpdate?.Invoke(obj, runtimeAnimator.frame);
            }
            bool looping = runtimeAnimator.ClipInfo.clip.isLooping;
            if (runtimeAnimator.frame >= runtimeAnimator.ClipInfo.clip.length || (!looping && runtimeAnimator.frame >= runtimeAnimator.ClipInfo.clip.length + 1))
            {
                runtimeAnimator.time = looping ? 0 : runtimeAnimator.ClipInfo.clip.length;
            }
        }
    }
    public class RuntimeRigidbody
    {
        public Transform rbObj;
        public Rigidbody2D rb;
        public Transform rbColliderParent;
        Vector2 upVelocity;
        float upMagnitude;
        Vector2 previousUpVelocity;
        float previousUpMagnitude;
        Vector2 rightVelocity;
        float rightMagnitude;
        Vector2 previousRightVelocity;
        float previousRightMagnitude;
        //important info, previousVelocity, velocity
        //Dealing with directed velocity
        //do vel build up and then update
        public System.Action<RuntimeObject, Vector2, Vector2, float, float> onVelocityUpdate;
        public static void CreateAndAttach(RuntimeObject obj)
        {
            RuntimeRigidbody runtimeRigidbody = new RuntimeRigidbody();
            obj.objStructure |= RuntimeObjectStructure.Rigidbody;
            runtimeRigidbody.rbObj = new GameObject($"Rigidbody2D:{obj.id}").transform;
            obj.obj.SetParent(runtimeRigidbody.rbObj);
            runtimeRigidbody.rbColliderParent = new GameObject($"ColliderParent:{obj.id}").transform;
            runtimeRigidbody.rbColliderParent.SetParent(runtimeRigidbody.rbObj);
            runtimeRigidbody.rb = runtimeRigidbody.rbObj.gameObject.AddComponent<Rigidbody2D>();
            runtimeRigidbody.rb.gravityScale = 0;
            runtimeRigidbody.rb.drag = 0;
            runtimeRigidbody.rb.angularDrag = 0;
            runtimeRigidbody.rb.freezeRotation = true;
            obj.rigidbody = runtimeRigidbody;
        }
        public static void Update(RuntimeObject obj, float tickDelta)
        {
            RuntimeRigidbody runtimeRigidbody = obj.rigidbody;
            //Grab previous velocities
            runtimeRigidbody.previousUpMagnitude = runtimeRigidbody.upMagnitude;
            runtimeRigidbody.previousUpVelocity = runtimeRigidbody.upVelocity;
            runtimeRigidbody.previousRightMagnitude = runtimeRigidbody.rightMagnitude;
            runtimeRigidbody.previousRightVelocity = runtimeRigidbody.rightVelocity;
            //Calculate current velocities
            runtimeRigidbody.upMagnitude = Vector2.Dot(runtimeRigidbody.rb.velocity, obj.up);
            runtimeRigidbody.upVelocity = runtimeRigidbody.upMagnitude * obj.up;
            runtimeRigidbody.upMagnitude = Mathf.Abs(runtimeRigidbody.upMagnitude);
            runtimeRigidbody.rightMagnitude = Vector2.Dot(runtimeRigidbody.rb.velocity, obj.right);
            runtimeRigidbody.rightVelocity = runtimeRigidbody.rightMagnitude * obj.right;
            runtimeRigidbody.rightMagnitude = Mathf.Abs(runtimeRigidbody.rightMagnitude);
            //Update
            runtimeRigidbody.onVelocityUpdate?.Invoke(obj, runtimeRigidbody.upVelocity, runtimeRigidbody.rightVelocity, runtimeRigidbody.upMagnitude, runtimeRigidbody.rightMagnitude);
        }
    }
    public class RuntimeDirectedCircleColliders
    {
        RuntimeAnimator animator;
        Transform colliderParent;
        DirectedCircleCollider[] directedCircleColliders;
        private List<CircleCollider2D> existingColliders;
        public static void CreateAndAttach(RuntimeObject obj)
        {
            if (!obj.objStructure.HasFlag(RuntimeObjectStructure.Animator))
                return;
            RuntimeDirectedCircleColliders runtimeDirectedCircleColliders = new();
            obj.objStructure |= RuntimeObjectStructure.DirectedCircleCollider;
            runtimeDirectedCircleColliders.animator = obj.animator;
            if(obj.objStructure.HasFlag(RuntimeObjectStructure.Rigidbody))
            {
                runtimeDirectedCircleColliders.colliderParent = obj.rigidbody.rbColliderParent;
            }
            else
            {
                runtimeDirectedCircleColliders.colliderParent = new GameObject($"ColliderParent:{obj.id}").transform;
            }
            runtimeDirectedCircleColliders.existingColliders = new();
            obj.directedCircleColliders = runtimeDirectedCircleColliders;
            obj.animator.onStateEnterData += runtimeDirectedCircleColliders.OnStateEnterData;
            obj.animator.onFrameUpdateData += runtimeDirectedCircleColliders.OnFrameUpdateData;
            GameManager.ins.onDirectedCircleCollidersDebug += runtimeDirectedCircleColliders.OnDebugUpdate;
        }
        public void OnStateEnterData(RuntimeObject obj, ControllerData controllerData, int stateHash)
        {
            directedCircleColliders = ControllerDataUtility.GetDirectedCircleColliders(controllerData, stateHash);

            //FRAME WILL ALWAYS BE 0 HERE
            //Get existing circle collider 2d in the collider parent
            int collidersRequired = 0;
            for (int _dataIndex = 0; _dataIndex < directedCircleColliders.Length; _dataIndex++)
            {
                //We are only concerned with data in this state that applies to the start of a state i.e. the 0th frame
                if (!directedCircleColliders[_dataIndex].assignedFrames.Contains(0))
                    continue;
                for (int _centerIndex = 0; _centerIndex < directedCircleColliders[_dataIndex].centers.Count; _centerIndex++)
                {
                    //We dont want to use the size of existingCollider as it will be changed during these loops
                    if(_centerIndex >= colliderParent.childCount)
                    {
                        //then we need to get a new circle collider from the pool
                        CircleCollider2D poolMember = GameManager.ins.circleCollider2DPool.Get();
                        poolMember.transform.SetParent(colliderParent);
                        //This line important for overlaps/interactions
                        GameManager.ins.circleCollider2DLedger[poolMember] = obj.id;
                        existingColliders.Add(poolMember);
                    }
                    //Right before we increase collidersRequired we should actually do the collider property set
                    existingColliders[collidersRequired].transform.position = obj.RelativePos(directedCircleColliders[_dataIndex].centers[_centerIndex]);
                    existingColliders[collidersRequired].radius = directedCircleColliders[_dataIndex].radii[_centerIndex];
                    existingColliders[collidersRequired].isTrigger = directedCircleColliders[_dataIndex].isTrigger;
                    existingColliders[collidersRequired].gameObject.layer = directedCircleColliders[_dataIndex].collisionLayer;
                    collidersRequired++;
                }
            }
            //[ ABOVE ] Deals with acquiring new pool objects and setting them up
            //[ BELOW ] Deals with releasing unused pool objects
            if(colliderParent.childCount - collidersRequired > 0)
            {
                //Then we have excess unused colliders which we should return to the pool
                while (colliderParent.childCount > collidersRequired)
                {
                    GameManager.ins.circleCollider2DPool.Release(existingColliders[existingColliders.Count - 1]);
                    existingColliders.RemoveAt(existingColliders.Count - 1);
                    if(existingColliders.Count == 0)
                    {
                        break;
                    }
                }
            }
        }
        public void OnFrameUpdateData(RuntimeObject obj, ControllerData controllerData, int frame)
        {
            int collidersRequired = 0;
            for (int _dataIndex = 0; _dataIndex < directedCircleColliders.Length; _dataIndex++)
            {
                if (!directedCircleColliders[_dataIndex].assignedFrames.Contains(frame))
                    continue;
                for (int _centerIndex = 0; _centerIndex < directedCircleColliders[_dataIndex].centers.Count; _centerIndex++)
                {
                    if(_centerIndex >= colliderParent.childCount)
                    {
                        CircleCollider2D poolMember = GameManager.ins.circleCollider2DPool.Get();
                        poolMember.transform.SetParent(colliderParent);
                        GameManager.ins.circleCollider2DLedger[poolMember] = obj.id;
                        existingColliders.Add(poolMember);
                    }
                    existingColliders[collidersRequired].transform.position = obj.RelativePos(directedCircleColliders[_dataIndex].centers[_centerIndex]);
                    existingColliders[collidersRequired].radius = directedCircleColliders[_dataIndex].radii[_centerIndex];
                    existingColliders[collidersRequired].isTrigger = directedCircleColliders[_dataIndex].isTrigger;
                    existingColliders[collidersRequired].gameObject.layer = directedCircleColliders[_dataIndex].collisionLayer;
                    collidersRequired++;
                }
            }
            if (colliderParent.childCount - collidersRequired > 0)
            {
                //Then we have excess unused colliders which we should return to the pool
                while (colliderParent.childCount > collidersRequired)
                {
                    GameManager.ins.circleCollider2DPool.Release(existingColliders[existingColliders.Count - 1]);
                    existingColliders.RemoveAt(existingColliders.Count - 1);
                    if (existingColliders.Count == 0)
                    {
                        break;
                    }
                }
            }
        }
        public void OnDebugUpdate()
        {

        }
    }
    public class RuntimeDirectedCircleOverlaps
    {
        DirectedCircleOverlap[] directedCircleOverlaps;
        public static void CreateAndAttach(RuntimeObject obj)
        {
            if (!obj.objStructure.HasFlag(RuntimeObjectStructure.Animator))
                return;
            RuntimeDirectedCircleOverlaps runtimeDirectedCirclOverlaps = new();
            obj.objStructure |= RuntimeObjectStructure.DirectedCircleOverlap;
            obj.animator.onStateEnterData += runtimeDirectedCirclOverlaps.OnStateEnterData;
        }
        public void OnStateEnterData(RuntimeObject obj, ControllerData controllerData, int stateHash)
        {
            directedCircleOverlaps = ControllerDataUtility.GetDirectedCircleOverlaps(controllerData, stateHash);
        }
        public void OnFrameUpdateData(RuntimeObject obj, ControllerData controllerData, int frame)
        {
            
        }
    }
}