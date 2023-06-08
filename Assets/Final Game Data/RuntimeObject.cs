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
    public void CreateAndAttach(ref RuntimeObject obj, RuntimeAnimatorController controller)
    {
        obj.objStructure |= RuntimeObjectStructure.Animator;
        spriteRenderer = obj.gameObject.AddComponent<SpriteRenderer>();
        animator = obj.gameObject.AddComponent<Animator>();
        animator.runtimeAnimatorController = controller;
        obj.runtimeAnimator = this;
    }
}
public class RuntimeRigidbody
{
    public Transform rbObj;
    public Rigidbody2D rb;
    public Transform rbColliderParent;
    public void CreateAndAttach(ref RuntimeObject obj)
    {
        obj.objStructure |= RuntimeObjectStructure.Rigidbody;
        rbObj = new GameObject($"Rigidbody2D:{obj.id}").transform;
        obj.obj.SetParent(rbObj);
        rbColliderParent = new GameObject($"ColliderParent:{obj.id}").transform;
        rbColliderParent.SetParent(rbObj);
        rb = rbObj.gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.drag = 0;
        rb.angularDrag = 0;
        rb.freezeRotation = true;
    }
}