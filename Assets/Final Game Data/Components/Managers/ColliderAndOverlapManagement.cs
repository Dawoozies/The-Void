using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using System;
using ComponentIO;
using GeometryDefinitions;
using Interactions;

namespace GameManagement
{
    //Dont have this static, it limits us in some ways that I don't understand yet
    //Just ensure only one instance is created at the start of the game
    //Have the GameManagement class act as mediator
    public interface IComponent
    {
        public (string, int) GetControllerNameAndStateHash();
    }
    public class Cache<T> where T : ScriptableObject
    {
        //Runtime cache meant for accessing during runtime
        Dictionary<(string, int), T> runtimeCache;
        public Cache()
        {
            runtimeCache = new Dictionary<(string, int), T>();
        }
        public void SetupRuntimeCache(AnimatorController[] controllers)
        {
            object[] components = Resources.LoadAll("", typeof(T));
            List<T> alreadyAssigned = new List<T>();
            foreach (AnimatorController controller in controllers)
            {
                foreach (T component in components)
                {
                    if (alreadyAssigned.Contains(component))
                        continue;
                    IComponent componentInterface = component as IComponent;
                    if (componentInterface == null)
                        continue;
                    (string, int) componentInfo = componentInterface.GetControllerNameAndStateHash();
                    if (controller.name != componentInfo.Item1)
                        continue;
                    runtimeCache.Add((controller.name, componentInfo.Item2), component);
                    Debug.Log($"Cached: <({controller.name}, {componentInfo.Item2}), {component.name}>");
                    alreadyAssigned.Add(component);
                }
            }
            Debug.Log($"Setup Runtime Cache Finished Running Cache Type = {typeof(T).Name}");
        }
        public T LoadComponent(string controllerName, int stateHash)
        {
            string debugCacheLoad = "";
            debugCacheLoad = debugCacheLoad + $"Trying to load cached data using key ({controllerName}, {stateHash})";
            if (!runtimeCache.ContainsKey((controllerName, stateHash)))
            {
                debugCacheLoad = debugCacheLoad + "\n LOAD FAILED";
                Debug.Log(debugCacheLoad);
                return null;
            }
            debugCacheLoad = debugCacheLoad + "\n LOAD SUCCESSFUL!";
            debugCacheLoad = debugCacheLoad + $" Retrieved Data: {runtimeCache[(controllerName, stateHash)].name}";
            Debug.Log(debugCacheLoad);
            return runtimeCache[(controllerName, stateHash)];
        }
    }
    //Turn this into a generic type
    //Specifically the part that should be generic is the vault/ledger structure
    public class ComponentBank<T> where T : Component
    {
        public GameObject bankObject;
        public T[] vault;
        public List<int> loanedIndices;
        public Dictionary<RuntimeSceneObject, List<int>> ledger;
        public T[] GetVaultObjects()
        {
            if (vault == null || vault.Length == 0)
                return null;
            return vault;
        }
        public void Initialize(string vaultName, int vaultSize)
        {
            bankObject = new GameObject($"BANK:{typeof(T).Name}_VAULT:{vaultName}");
            vault = new T[vaultSize];
            loanedIndices = new List<int>();
            ledger = new Dictionary<RuntimeSceneObject, List<int>>();
            for (int i = 0; i < vaultSize; i++)
            {
                GameObject vaultObject = new GameObject($"{typeof(T).Name.ToUpper()}_{i}");
                vaultObject.transform.parent = bankObject.transform;
                T component = vaultObject.AddComponent<T>();
                vault[i] = component;
                vaultObject.SetActive(false);
            }
        }
        public bool RequestLoan(RuntimeSceneObject requester, int loanSize)
        {
            if (vault == null || vault.Length == 0)
                return false;
            List<int> newLedgerEntry = new List<int>();
            bool loanListEmpty = loanedIndices.Count == 0;
            for (int i = 0; i < vault.Length; i++)
            {
                if (!loanListEmpty && loanedIndices.Contains(i))
                    continue;
                loanedIndices.Add(i);
                newLedgerEntry.Add(i);
                if (newLedgerEntry.Count == loanSize)
                    break;
            }
            if(!ledger.ContainsKey(requester))
            {
                ledger.Add(requester, newLedgerEntry);
            }
            else
            {
                ledger[requester].AddRange(newLedgerEntry);
            }
            if (newLedgerEntry.Count < loanSize)
                Debug.LogError($"RuntimeID:{requester.ID} failed to recieve full loan from {typeof(T).Name} bank!");
            return true;
        }
        public bool RequestLoanForRigidbody2D(RuntimeSceneObject requester, int loanSize)
        {
            if (requester.rbObj == null)
                return false;
            if (vault == null || vault.Length == 0)
                return false;
            List<int> newLedgerEntry = new List<int>();
            bool loanListEmpty = loanedIndices.Count == 0;
            for (int i = 0; i < vault.Length; i++)
            {
                if (!loanListEmpty && loanedIndices.Contains(i))
                    continue;
                loanedIndices.Add(i);
                newLedgerEntry.Add(i);

                Collider2D collider = vault[i] as Collider2D;
                if(collider != null)
                {
                    collider.transform.parent = requester.rbColliderParent;
                }

                if (newLedgerEntry.Count == loanSize)
                    break;
            }
            if(!ledger.ContainsKey(requester))
            {
                ledger.Add(requester, newLedgerEntry);
            }
            else
            {
                ledger[requester].AddRange(newLedgerEntry);
            }
            if (newLedgerEntry.Count < loanSize)
                Debug.LogError($"RuntimeID:{requester.ID} failed to recieve full loan from {typeof(T).Name} bank!");
            return true;
        }
    }
    public static class UpdateLedger
    {
        public static void CircleCollider2D(ComponentBank<CircleCollider2D> bank, RuntimeSceneObject obj, Component_CircleCollider2D_Data[] dataAtFrame)
        {
            if (!bank.ledger.ContainsKey(obj))
                return;
            List<int> ledgerIndices = bank.ledger[obj];
            int assignedCircles = 0;
            foreach (Component_CircleCollider2D_Data componentData in dataAtFrame)
            {
                for (int i = 0; i < componentData.circles.Count; i++)
                {
                    if (assignedCircles == ledgerIndices.Count)
                        break;
                    Circle circle = componentData.circles[i];
                    CircleCollider2D vaultObject = bank.vault[ledgerIndices[assignedCircles]];
                    vaultObject.gameObject.SetActive(true);
                    vaultObject.gameObject.layer = componentData.layer;
                    vaultObject.transform.position = obj.LocalPosFromTransform(circle.center);
                    vaultObject.radius = circle.radius;
                    vaultObject.isTrigger = componentData.isTrigger;
                    ComponentDebugging.ins.DrawCircleCollider(ledgerIndices[assignedCircles], componentData.fillColor);
                    assignedCircles++;
                }
            }
            for (int i = assignedCircles; i < ledgerIndices.Count; i++)
            {
                bank.vault[ledgerIndices[i]].gameObject.SetActive(false);
                ComponentDebugging.ins.StopDrawCircleCollider(i);
            }
        }
        public static void NullComponent(ComponentBank<CircleCollider2D> bank, RuntimeSceneObject obj)
        {
            if (!bank.ledger.ContainsKey(obj))
                return;
            List<int> ledgerIndices = bank.ledger[obj];
            for (int i = 0; i < ledgerIndices.Count; i++)
            {
                bank.vault[ledgerIndices[i]].gameObject.SetActive(false);
                ComponentDebugging.ins.StopDrawCircleCollider(ledgerIndices[i]);
            }
        }
    }
    public class OverlapManager
    {
        public List<Collider2D> overlapResults;
        List<(RuntimeSceneObject, Component_Overlap_Data, float)> overlaps;
        public void Initialize()
        {
            overlapResults = new List<Collider2D>();
            overlaps = new List<(RuntimeSceneObject, Component_Overlap_Data, float)>();
            ComponentDebugging.ins.overlapsActive = overlaps;
        }
        public void ManagedUpdate(float tickDelta)
        {
            if (overlaps == null || overlaps.Count == 0)
                return;
            for (int i = 0; i < overlaps.Count; i++)
            {
                RuntimeSceneObject obj = overlaps[i].Item1;
                Component_Overlap_Data componentData = overlaps[i].Item2;
                float timeHeld = overlaps[i].Item3;
                overlaps[i] = (obj, componentData, timeHeld + obj.objTickRate(tickDelta)*obj.animatorStateInfo.speed);
                if (overlaps[i].Item3 > componentData.holdTime)
                {
                    overlaps.RemoveAt(i);
                    continue;
                }
                //Otherwise we will do the cast now
                Overlap.Circles(obj, componentData, ref overlapResults);
                //Package circle results into interactions
                Overlap.Areas(obj, componentData, ref overlapResults);
                //Package area results into interactions
            }
        }
        public void OverlapApply(RuntimeSceneObject obj, Component_Overlap_Data componentData)
        {
            if (componentData.circles.Count > 0 || componentData.areas.Count > 0)
            {
                overlaps.Add((obj, componentData, 0f));
            }
        }
    }
    public static class Overlap
    {
        public static void Circles(RuntimeSceneObject obj, Component_Overlap_Data componentData, ref List<Collider2D> results)
        {
            if (componentData.circles == null || componentData.circles.Count == 0)
                return;
            for (int i = 0; i < componentData.circles.Count; i++)
            {
                Circle circle = componentData.circles[i];
                Vector3 worldPos = obj.LocalPosFromTransform(circle.center);
                int resultCount = Physics2D.OverlapCircle(worldPos, circle.radius, componentData.ContactFilter(), results);
                Debug.Log($"Overlap.Circles run, result count: {resultCount}");
                if (resultCount == 0)
                {
                    if (componentData.useNullResult)
                    {
                        Debug.Log($"Null Interaction: ({obj.ID}, {componentData.nickname}, null)");
                        GameManagement.ins.overlapInteractions.AddInteractionToBuffer((obj, componentData));
                    }
                    continue;
                }
                ComponentBank<CircleCollider2D> bank = GameManagement.ins.circleCollider2DBank;
                List<int> loanedIndices = bank.loanedIndices;
                for (int j = 0; j < resultCount; j++)
                {
                    CircleCollider2D collider = results[j] as CircleCollider2D;
                    if (collider == null)
                    {
                        if (StaticInteract.StaticCheck(results[j]))
                        {
                            Debug.Log($"Static Interaction: ({obj.ID}, {componentData.nickname}, {results[j].gameObject.name})");
                            GameManagement.ins.overlapInteractions.AddInteractionToBuffer((obj, componentData, results[j].gameObject));
                        }
                        continue;
                    }
                    int index = Array.IndexOf(bank.vault, collider);
                    if (!loanedIndices.Contains(index))
                        continue;
                    foreach(RuntimeSceneObject key in bank.ledger.Keys)
                    {
                        if(bank.ledger[key].Contains(index))
                        {
                            Debug.Log($"Interaction: ({obj.ID}, {componentData.nickname}, {key.ID})");
                            GameManagement.ins.overlapInteractions.AddInteractionToBuffer((obj, componentData, key));
                        }
                    }
                }
            }
        }
        public static void Areas(RuntimeSceneObject obj, Component_Overlap_Data componentData, ref List<Collider2D> results)
        {
            if (componentData.areas == null || componentData.areas.Count == 0)
                return;
            for (int i = 0; i < componentData.areas.Count; i++)
            {
                Area area = componentData.areas[i];
                Vector3 pointA = obj.LocalPosFromTransform(area.PointA());
                Vector3 pointB = obj.LocalPosFromTransform(area.PointB());
                int resultCount = Physics2D.OverlapArea(pointA, pointB, componentData.ContactFilter(), results);
                Debug.Log($"Overlap.Areas run, result count: {resultCount}");
                if(resultCount == 0)
                {
                    if(componentData.useNullResult)
                    {
                        Debug.Log($"Null Interaction: ({obj.ID}, {componentData.nickname}, null)");
                        GameManagement.ins.overlapInteractions.AddInteractionToBuffer((obj, componentData));
                    }
                    continue;
                }
                ComponentBank<CircleCollider2D> bank = GameManagement.ins.circleCollider2DBank;
                List<int> loanedIndices = bank.loanedIndices;
                for (int j = 0; j < resultCount; j++)
                {
                    CircleCollider2D collider = results[j] as CircleCollider2D;
                    if (collider == null)
                    {
                        if (StaticInteract.StaticCheck(results[j]))
                        {
                            Debug.Log($"Static Interaction: ({obj.ID}, {componentData.nickname}, {results[j].gameObject.name})");
                            GameManagement.ins.overlapInteractions.AddInteractionToBuffer((obj, componentData, results[j].gameObject));
                        }
                        continue;
                    }
                    int index = Array.IndexOf(bank.vault, collider);
                    if (!loanedIndices.Contains(index))
                        continue;
                    foreach (RuntimeSceneObject key in bank.ledger.Keys)
                    {
                        if (bank.ledger[key].Contains(index))
                        {
                            Debug.Log($"Interaction: ({obj.ID}, {componentData.nickname}, {key.ID})");
                            GameManagement.ins.overlapInteractions.AddInteractionToBuffer((obj, componentData, key));
                        }
                    }
                }
            }
        }
    }
    public class OverlapInteractions
    {
        public List<(RuntimeSceneObject, Component_Overlap_Data, RuntimeSceneObject)> interactionBuffer;
        public List<(RuntimeSceneObject, Component_Overlap_Data, GameObject)> staticInteractionBuffer;
        public List<(RuntimeSceneObject, Component_Overlap_Data)> nullInteractionBuffer;
        public void Initialize()
        {
            interactionBuffer = new List<(RuntimeSceneObject, Component_Overlap_Data, RuntimeSceneObject)>();
            staticInteractionBuffer = new List<(RuntimeSceneObject, Component_Overlap_Data, GameObject)>();
            nullInteractionBuffer = new List<(RuntimeSceneObject, Component_Overlap_Data)>();
        }
        public void AddInteractionToBuffer((RuntimeSceneObject, Component_Overlap_Data, RuntimeSceneObject) interactionInfo)
        {
            interactionBuffer.Add(interactionInfo);
        }
        public void AddInteractionToBuffer((RuntimeSceneObject, Component_Overlap_Data, GameObject) interactionInfo)
        {
            staticInteractionBuffer.Add(interactionInfo);
        }
        public void AddInteractionToBuffer((RuntimeSceneObject, Component_Overlap_Data) interactionInfo)
        {
            nullInteractionBuffer.Add(interactionInfo);
        }
        public void ManagedUpdate()
        {
            if(interactionBuffer != null && interactionBuffer.Count > 0)
            {
                for (int i = 0; i < interactionBuffer.Count; i++)
                {
                    RuntimeSceneObject obj = interactionBuffer[i].Item1;
                    Component_Overlap_Data componentData = interactionBuffer[i].Item2;
                    RuntimeSceneObject hitObj = interactionBuffer[i].Item3;
                    Interact.Interactions(obj, componentData, hitObj, ref interactionBuffer);
                }
            }
            if(staticInteractionBuffer != null && staticInteractionBuffer.Count > 0)
            {
                for (int i = 0; i < staticInteractionBuffer.Count; i++)
                {
                    RuntimeSceneObject obj = staticInteractionBuffer[i].Item1;
                    Component_Overlap_Data componentData = staticInteractionBuffer[i].Item2;
                    GameObject hitObj = staticInteractionBuffer[i].Item3;
                    StaticInteract.Interactions(obj, componentData, hitObj, ref staticInteractionBuffer);
                }
            }
            if(nullInteractionBuffer != null && nullInteractionBuffer.Count > 0)
            {
                for (int i = 0; i < nullInteractionBuffer.Count; i++)
                {
                    RuntimeSceneObject obj = nullInteractionBuffer[i].Item1;
                    Component_Overlap_Data componentData = nullInteractionBuffer[i].Item2;
                    NullInteract.Interactions(obj, componentData, ref nullInteractionBuffer);
                }
            }
        }
        //Leave this for now and do physics + input
        //We need some actual movement
    }


    //OverlapInteraction
    //OBJ 1 OVERLAP DATA -- Hits --> OBJ 2
    //(RuntimeSceneObject1, Obj1OverlapData, RuntimeSceneObject2)
    //Start interaction
    //Update interaction
    //Stop interaction
    //int results = Physics2D.OverlapCircle(position, circle.radius, componentData.ContactFilter(), overlapResults);
    //Debug.Log($"OverlapApply:({obj.ID}, {componentData.nickname}) Results = {results}");
    //A base set of static interactions (RuntimeSceneObject, ObjOverlapData, (GameObject)StaticObject)
}