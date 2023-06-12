using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using RuntimeObjects;
using System;
public class GameManager : MonoBehaviour
{

    public static GameManager ins;
    public RuntimeAnimatorController[] controllers;
    public Dictionary<string, RuntimeAnimatorController> allControllers; //Key = controller.name
    public Dictionary<string, ControllerData> allControllerData; //Key = controller.name
    public Dictionary<string, RuntimeObject> allRuntimeObjects; //Key = whatever input was on creation / Key = runtimeObject.id
    //Collider2D Pool
    public Transform circleCollider2DParent;
    public ObjectPool<CircleCollider2D> circleCollider2DPool;
    public Dictionary<CircleCollider2D, string> circleCollider2DLedger;
    //SpriteRenderer Pool
    public Transform spriteRendererParent;
    public ObjectPool<SpriteRenderer> spriteRendererPool;
    public Dictionary<SpriteRenderer, string> spriteRendererLedger;

    public Sprite[] sprites;
    public Dictionary<string, Sprite> allSprites;

    //Debugs
    public Action onDirectedCircleCollidersDebug;
    public bool showDirectedCircleColliders;
    private void Awake()
    {
        ins = this;
        Setup_ControllerData();
        Setup_ControllerDictionary();
        Setup_CircleCollider2DPool();
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
    void Setup_CircleCollider2DPool()
    {
        circleCollider2DParent = new GameObject("Circle Collider 2D Pool").transform;
        circleCollider2DLedger = new();
        circleCollider2DPool = new ObjectPool<CircleCollider2D>(
            () => 
            {
                CircleCollider2D member = new GameObject("Circle Collider 2D").AddComponent<CircleCollider2D>();
                member.transform.SetParent(circleCollider2DParent);
                circleCollider2DLedger.Add(member, string.Empty);
                return member;
            },
            (CircleCollider2D member) =>
            {
                member.gameObject.SetActive(true);
            },
            (CircleCollider2D member) =>
            {
                member.transform.SetParent(circleCollider2DParent);
                member.gameObject.SetActive(false);
                circleCollider2DLedger[member] = string.Empty;
            },
            (CircleCollider2D member) =>
            {
                circleCollider2DLedger.Remove(member);
                Destroy(member.gameObject);
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
        //allRuntimeObjects.Add("Player", new RuntimeObject("Player"));
        //RuntimeAnimator.CreateAndAttach(allRuntimeObjects["Player"], allControllers["FinalPlayer"]);
        //RuntimeRigidbody.CreateAndAttach(allRuntimeObjects["Player"]);
        allRuntimeObjects.Add("Player", new Player("Player"));
        RuntimeAnimator.CreateAndAttach(allRuntimeObjects["Player"], allControllers["FinalPlayer"]);
        RuntimeRigidbody.CreateAndAttach(allRuntimeObjects["Player"]);
        RuntimeDirectedCircleColliders.CreateAndAttach(allRuntimeObjects["Player"]);
        //Then call all ManagedStart methods :))
        foreach (string key in allRuntimeObjects.Keys)
        {
            allRuntimeObjects[key].managedStart?.Invoke();
        }
    }
    private void Update()
    {
        RuntimeAnimator.Update(allRuntimeObjects["Player"], Time.deltaTime);
        foreach (string key in allRuntimeObjects.Keys)
        {
            allRuntimeObjects[key].managedUpdate?.Invoke(Time.deltaTime);
        }
        onDirectedCircleCollidersDebug?.Invoke();
    }
}
