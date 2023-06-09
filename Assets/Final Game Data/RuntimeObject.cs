using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[Flags]
public enum RuntimeObjectStructure
{
    Default = 0,
    Animator = 1,
    Rigidbody = 2,
}
public class RuntimeObject
{
    public string id;
    public GameObject gameObject;
    public Transform obj;
    public float localTickRateMultiplier;
    public RuntimeObjectStructure objStructure;
    public RuntimeAnimator runtimeAnimator;
    public RuntimeRigidbody runtimeRigidbody;
    public float TickRate(float globalTickRate) => globalTickRate * localTickRateMultiplier;
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
    public int stateHash;
    public int previousStateHash;
    public float time;
    public int frame;
    public float normalizedTime;
    public static void CreateAndAttach(RuntimeObject obj, RuntimeAnimatorController controller)
    {
        RuntimeAnimator runtimeAnimator = new RuntimeAnimator();
        obj.objStructure |= RuntimeObjectStructure.Animator;
        runtimeAnimator.spriteRenderer = obj.gameObject.AddComponent<SpriteRenderer>();
        runtimeAnimator.animator = obj.gameObject.AddComponent<Animator>();
        runtimeAnimator.animator.runtimeAnimatorController = controller;
        obj.runtimeAnimator = runtimeAnimator;
    }
    public static void Update(RuntimeObject obj, float tickDelta)
    {
        RuntimeAnimator runtimeAnimator = obj.runtimeAnimator;
        if(runtimeAnimator.stateHash != runtimeAnimator.StateInfo.shortNameHash)
        {
            runtimeAnimator.previousStateHash = runtimeAnimator.stateHash;
            runtimeAnimator.stateHash = runtimeAnimator.StateInfo.shortNameHash;
            runtimeAnimator.animator.SetFloat("NormalizedTime", 0f);
            runtimeAnimator.time = 0;
            runtimeAnimator.frame = 0;
        }
        runtimeAnimator.time += obj.TickRate(tickDelta)*runtimeAnimator.StateInfo.speed;
        runtimeAnimator.normalizedTime = (float)Mathf.FloorToInt(runtimeAnimator.time) / runtimeAnimator.ClipInfo.clip.length;
        runtimeAnimator.animator.SetFloat("NormalizedTime", runtimeAnimator.normalizedTime);
        if(runtimeAnimator.frame != Mathf.FloorToInt(runtimeAnimator.time))
        {
            runtimeAnimator.frame = Mathf.FloorToInt(runtimeAnimator.time);
        }
        bool looping = runtimeAnimator.ClipInfo.clip.isLooping;
        if(runtimeAnimator.frame >= runtimeAnimator.ClipInfo.clip.length || (!looping && runtimeAnimator.frame >= runtimeAnimator.ClipInfo.clip.length + 1))
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
    }
}