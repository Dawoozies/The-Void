using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using RuntimeObjects;
using RuntimeContainers;
public class GameManager : MonoBehaviour
{

    public static GameManager ins;
    public RuntimeAnimatorController[] controllers;
    public Dictionary<string, RuntimeAnimatorController> allControllers; //Key = controller.name
    public Dictionary<string, ControllerData> allControllerData; //Key = controller.name
    //public Dictionary<string, RuntimeObject> allRuntimeObjects; //Key = whatever input was on creation / Key = runtimeObject.id
    public List<RuntimeObject> allRuntimeObjects;
    public List<RuntimeObject> allRuntimeWeapons;
    public RuntimeObject FindByID(string id)
    {
        foreach (RuntimeObject obj in allRuntimeObjects) 
        {
            if(obj.id == id) 
            {
                return obj;
            }
        }
        return null;
    }
    //DirectedCircleCollider Pool
    public Transform directedCircleCollider2DParent;
    public ObjectPool<DirectedCircleColliderContainer> directedCircleColliderContainerPool;
    public Dictionary<DirectedCircleColliderContainer, string> directedCircleColliderContainerLedger;
    public DirectedCircleColliderContainer TryFindDirectedCircleColliderContainerValue(CircleCollider2D collider)
    {
        if(directedCircleColliderContainerLedger == null || directedCircleColliderContainerLedger.Count == 0)
            return null;
        foreach (KeyValuePair<DirectedCircleColliderContainer, string> pair in directedCircleColliderContainerLedger)
        {
            if (pair.Key.collider == collider)
            {
                return pair.Key;
            }
        }
        return null;
    }
    //SpriteRenderer Pool
    public Transform spriteRendererParent;
    public ObjectPool<SpriteRenderer> spriteRendererPool;
    public Dictionary<SpriteRenderer, string> spriteRendererLedger;

    public List<Sprite> sprites;
    public Sprite FindSpriteByID(string id)
    {
        foreach (Sprite sprite in sprites)
        {
            if (sprite.name == id)
            {
                return sprite;
            }
        }
        return null;
    }
    //Player Weapon Pool
    public ObjectPool<Weapon> runtimeWeaponPool;
    //Effect Pool
    //Make this for on hit effects specifically requiring animators etc.
    //this will be good as some effects could have overlaps or even colliders
    public List<Material> allMaterials;
    public Material FindMaterialByID(string id)
    {
        foreach (Material material in allMaterials)
        {
            if(material.name == id)
            {
                return material;
            }
        }
        return null;
    }
    private void Awake()
    {
        ins = this;
        Setup_ControllerData();
        Setup_ControllerDictionary();
        Setup_DirectedCircleCollider2DPool();
        Setup_SpriteRendererPool();
        Setup_RuntimeWeaponPool();
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
    void Setup_RuntimeWeaponPool()
    {
        runtimeWeaponPool = new ObjectPool<Weapon>(
            () =>
            {
                //Creates a generalised weapon
                //This will not be in allRuntimeObjects
                Weapon member = new Weapon("Weapon", WeaponHeadSpriteType.Spear, WeaponShaftSpriteType.Long, WeaponPommelSpriteType.Default);
                allRuntimeWeapons.Add(member);

                //member.rigidbody.rb.transform.SetParent(runtimeWeaponParent);
                return member;
            },
            (Weapon member) =>
            {
                member.rigidbody.rb.transform.gameObject.SetActive(true);
            },
            (Weapon member) =>
            {
                //member.rigidbody.rb.transform.SetParent(runtimeWeaponParent);
                member.rigidbody.rb.transform.gameObject.SetActive(false);
            },
            (Weapon member) =>
            {
                allRuntimeWeapons.Remove(member);
                Destroy(member.rigidbody.rb.transform.gameObject);
            },
            false,
            50,
            200
            );
    }
    void RuntimeObjectCreate_Player()
    {
        Player player = new("Player");
        allRuntimeObjects.Add(player);

    }
    void RuntimeObjectCreate_HangedFrame()
    {
        //Main HangedFrame Object
        HangedFrame hangedFrame = new("HangedFrame");
        allRuntimeObjects.Add(hangedFrame);
        RuntimeAnimator.CreateAndAttach(hangedFrame, allControllers["HangedFrame"]);
        RuntimeDirectedCircleColliders.CreateAndAttach(hangedFrame);
        RuntimeDirectedCircleOverlaps.CreateAndAttach(hangedFrame);
        RuntimeDirectedPoints.CreateAndAttach(hangedFrame);
    }
    void RuntimeObjectCreate_Mantis()
    {
        Mantis mantis = new("Mantis");
        allRuntimeObjects.Add(mantis);
    }
    void RuntimeObjectCreate_Hallway()
    {
        Hallway hallway = new();
        allRuntimeObjects.Add(hallway);
    }
    public Weapon SpawnWeapon(WeaponHeadSpriteType headSpriteType, WeaponShaftSpriteType shaftSpriteType, WeaponPommelSpriteType pommelSpriteType)
    {
        Weapon newWeapon = runtimeWeaponPool.Get();
        newWeapon.head.SetSpriteType(headSpriteType);
        newWeapon.shaft.SetSpriteType(shaftSpriteType);
        newWeapon.pommel.SetSpriteType(pommelSpriteType);
        return newWeapon;
    }
    private void Start()
    {
        allRuntimeObjects = new();
        allRuntimeWeapons = new();
        RuntimeObjectCreate_Player();
        RuntimeObjectCreate_Hallway();
        //RuntimeObjectCreate_HangedFrame();
        //RuntimeObjectCreate_Mantis();
        //Then call all ManagedStart methods :))
        foreach (RuntimeObject item in allRuntimeObjects)
        {
            item.managedStart?.Invoke();
        }
    }
    private void Update()
    {
        InputManager.ins.ManagedUpdate(Time.deltaTime);
        foreach (RuntimeObject item in allRuntimeObjects)
        {
            item.managedUpdate?.Invoke(item, Time.deltaTime);
        }
        foreach(RuntimeObject item in allRuntimeWeapons)
        {
            item.managedUpdate?.Invoke(item, Time.deltaTime);
        }
    }
    private void FixedUpdate()
    {
        foreach (RuntimeObject item in allRuntimeObjects)
        {
            item.managedFixedUpdate?.Invoke(item, Time.fixedDeltaTime);
        }
        foreach (RuntimeObject item in allRuntimeWeapons)
        {
            item.managedFixedUpdate?.Invoke(item, Time.fixedDeltaTime);
        }
    }
}
