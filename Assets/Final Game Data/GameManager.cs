using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using RuntimeObjects;
using System;
using RuntimeContainers;

public class GameManager : MonoBehaviour
{

    public static GameManager ins;
    public RuntimeAnimatorController[] controllers;
    public Dictionary<string, RuntimeAnimatorController> allControllers; //Key = controller.name
    public Dictionary<string, ControllerData> allControllerData; //Key = controller.name
    public Dictionary<string, RuntimeObject> allRuntimeObjects; //Key = whatever input was on creation / Key = runtimeObject.id
    //DirectedCircleCollider Pool
    public Transform directedCircleCollider2DParent;
    public ObjectPool<DirectedCircleColliderContainer> directedCircleColliderContainerPool;
    public Dictionary<DirectedCircleColliderContainer, string> directedCircleColliderContainerLedger;
    public string TryFindDirectedCircleColliderContainerValue(CircleCollider2D collider)
    {
        if(directedCircleColliderContainerLedger == null || directedCircleColliderContainerLedger.Count == 0)
            return string.Empty;
        foreach (KeyValuePair<DirectedCircleColliderContainer, string> pair in directedCircleColliderContainerLedger)
        {
            if (pair.Key.collider == collider)
                return pair.Value;
        }
        return string.Empty;
    }
    //SpriteRenderer Pool
    public Transform spriteRendererParent;
    public ObjectPool<SpriteRenderer> spriteRendererPool;
    public Dictionary<SpriteRenderer, string> spriteRendererLedger;

    public Sprite[] sprites;
    public Dictionary<string, Sprite> allSprites;
    private void Awake()
    {
        ins = this;
        Setup_ControllerData();
        Setup_ControllerDictionary();
        Setup_DirectedCircleCollider2DPool();
        Setup_Sprites();
        Setup_SpriteRendererPool();
    }
    void Setup_ControllerDictionary()
    {
        if(controllers == null || controllers.Length == 0)
            return;
        allControllers = new Dictionary<string, RuntimeAnimatorController>();
        foreach (RuntimeAnimatorController controller in controllers)
        {
            if (allControllers.ContainsValue(controller))
                continue;
            if (allControllers.ContainsKey(controller.name))
                continue;
            Debug.Log($"allControllers Caching: {controller.name} Controller");
            allControllers.Add(controller.name, controller);
        }
    }
    void Setup_ControllerData()
    {
        ControllerData[] loadedControllerData = ResourcesUtility.LoadAllControllerDataInResources();
        allControllerData = new Dictionary<string, ControllerData>();
        int dataAssigned = 0;
        foreach (ControllerData controllerData in loadedControllerData)
        {
            if (allControllerData.ContainsValue(controllerData))
                continue;
            for (int i = 0; i < controllers.Length; i++)
            {
                if (allControllerData.ContainsKey(controllers[i].name))
                    continue;
                if (controllers[i].name == controllerData.controllerName)
                {
                    Debug.Log($"allControllerData Caching: {controllers[i].name} Controller Data");
                    allControllerData.Add(controllers[i].name, controllerData);
                    dataAssigned++;
                }
                if(dataAssigned == controllers.Length)
                {
                    Debug.Log("All controller data assigned");
                    return;
                }
            }
        }
    }
    void Setup_DirectedCircleCollider2DPool()
    {
        directedCircleCollider2DParent = new GameObject("Directed Circle Collider Container Pool").transform;
        directedCircleColliderContainerLedger = new();
        directedCircleColliderContainerPool = new ObjectPool<DirectedCircleColliderContainer>(
            () =>
            {
                DirectedCircleColliderContainer member = new DirectedCircleColliderContainer();
                member.collider = new GameObject("Directed Circle Collider 2D").AddComponent<CircleCollider2D>();
                member.collider.transform.SetParent(directedCircleCollider2DParent);
                directedCircleColliderContainerLedger.Add(member, string.Empty);
                return member;
            },
            (DirectedCircleColliderContainer member) =>
            {
                member.collider.gameObject.SetActive(true);
            },
            (DirectedCircleColliderContainer member) =>
            {
                member.collider.transform.SetParent(directedCircleCollider2DParent);
                member.collider.gameObject.SetActive(false);
                directedCircleColliderContainerLedger[member] = string.Empty;
            },
            (DirectedCircleColliderContainer member) =>
            {
                directedCircleColliderContainerLedger.Remove(member);
                Destroy(member.collider.gameObject);
            },
            false,
            100,
            500
            );
    }
    void Setup_Sprites()
    {
        if (sprites == null || sprites.Length == 0)
            return;
        allSprites = new Dictionary<string, Sprite>();
        for (int i = 0; i < sprites.Length; i++)
        {
            allSprites.Add(sprites[i].name, sprites[i]);
        }
    }
    void Setup_SpriteRendererPool()
    {
        spriteRendererParent = new GameObject("Sprite Renderer Pool").transform;
        spriteRendererLedger = new();
        spriteRendererPool = new ObjectPool<SpriteRenderer>(
            () =>
            {
                SpriteRenderer member = new GameObject("Sprite Renderer").AddComponent<SpriteRenderer>();
                member.transform.SetParent(spriteRendererParent);
                spriteRendererLedger.Add(member, string.Empty);
                return member;
            },
            (SpriteRenderer member) =>
            {
                member.gameObject.SetActive(true);
            },
            (SpriteRenderer member) =>
            {
                member.transform.SetParent(spriteRendererParent);
                member.gameObject.SetActive(false);
                spriteRendererLedger[member] = string.Empty;
            },
            (SpriteRenderer member) =>
            {
                spriteRendererLedger.Remove(member);
                Destroy(member.gameObject);
            },
            false,
            100,
            500
            );
    }
    private void Start()
    {
        allRuntimeObjects = new Dictionary<string, RuntimeObject>();
        allRuntimeObjects.Add("Player", new Player("Player"));
        RuntimeAnimator.CreateAndAttach(allRuntimeObjects["Player"], allControllers["FinalPlayer"]);
        RuntimeRigidbody.CreateAndAttach(allRuntimeObjects["Player"]);
        RuntimeDirectedCircleColliders.CreateAndAttach(allRuntimeObjects["Player"]);
        RuntimeDirectedCircleOverlaps.CreateAndAttach(allRuntimeObjects["Player"]);
        //Then call all ManagedStart methods :))
        foreach (string key in allRuntimeObjects.Keys)
        {
            allRuntimeObjects[key].managedStart?.Invoke();
        }
    }
    private void Update()
    {
        foreach (string key in allRuntimeObjects.Keys)
        {
            allRuntimeObjects[key].managedUpdate?.Invoke(allRuntimeObjects[key], Time.deltaTime);
        }
    }
    private void FixedUpdate()
    {
        foreach (string key in allRuntimeObjects.Keys)
        {
            allRuntimeObjects[key].managedFixedUpdate?.Invoke(allRuntimeObjects[key], Time.fixedDeltaTime);
        }
    }
}
