using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeObjects;
using StateHandlers.Weapon;
namespace OverlapHandlers.Weapon
{
    public static class OnRuntimeObjectOverlap
    {
        public static void Handle(string dataName, RuntimeObject obj, RuntimeObject hitObj, Vector2 overlapUp, Vector2 overlapRight)
        {
            if(dataName == "Embed")
            {
                Debug.LogError("Running");
                WeaponHead head = obj as WeaponHead;
                if(head != null)
                {
                    //Debug.LogError($"{head.id} Embedding into {hitObj.id}");
                }
            }
        }
    }
    public static class OnNonRuntimeObjectOverlap
    {
        public static void Handle(string dataName, RuntimeObject obj, Collider2D hitCollider)
        {
            if(dataName == "Embed")
            {
                WeaponHead head = obj as WeaponHead;
                if(head != null)
                {
                    if(hitCollider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        //Debug.LogError($"{head.id} Embedding into {hitCollider.gameObject.name}");
                    }
                }
            }
        }
    }
}