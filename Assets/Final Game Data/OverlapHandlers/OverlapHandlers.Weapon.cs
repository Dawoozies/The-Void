using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeObjects;
using StateHandlers.Weapon;
using RuntimeContainers;

namespace OverlapHandlers.Weapon
{
    public static class OnRuntimeObjectOverlap
    {
        public static void Handle(string dataName, RuntimeObject obj, DirectedCircleColliderContainer hitContainer, Vector2 overlapUp, Vector2 overlapRight)
        {
            if(dataName == "Embed")
            {
                //Debug.LogError("Running");
                WeaponHead head = obj as WeaponHead;
                if(head != null)
                {
                    //Debug.LogError($"{head.id} Embedding into {hitObj.id}");
                }
            }
            if(dataName == "DamageZone_Weapon")
            {
                WeaponHead head = obj as WeaponHead;
                if(head != null)
                {
                    //Debug.LogError($"Hit ID = {hitContainer.obj.id} Hit Collider Name = {hitContainer.colliderNickname}");
                    if (hitContainer.colliderNickname == "Hitbox_MantisFrontLegCalf" || hitContainer.colliderNickname == "Hitbox_MantisBackLegCalf")
                    {
                        RuntimeObjects.Mantis mantis = GameManager.ins.FindByID("Mantis") as RuntimeObjects.Mantis;
                        if(mantis != null ) 
                        {
                            mantis.Stagger(50);
                        }
                    }
                    if(hitContainer.colliderNickname == "Hitbox_MantisLegs")
                    {
                        RuntimeObjects.Mantis mantis = GameManager.ins.FindByID("Mantis") as RuntimeObjects.Mantis;
                        if (mantis != null)
                        {
                            mantis.Stagger(50);
                        }
                    }
                    if (hitContainer.colliderNickname == "Hitbox_MantisTorso")
                    {
                        RuntimeObjects.Mantis mantis = GameManager.ins.FindByID("Mantis") as RuntimeObjects.Mantis;
                        if (mantis != null)
                        {
                            mantis.Stagger(50);
                        }
                    }
                    if (hitContainer.colliderNickname == "Hitbox_MantisNeck")
                    {
                        RuntimeObjects.Mantis mantis = GameManager.ins.FindByID("Mantis") as RuntimeObjects.Mantis;
                        if (mantis != null)
                        {
                            mantis.Stagger(50);
                        }
                    }
                    if (hitContainer.colliderNickname == "Hitbox_MantisHead")
                    {
                        RuntimeObjects.Mantis mantis = GameManager.ins.FindByID("Mantis") as RuntimeObjects.Mantis;
                        if (mantis != null)
                        {
                            mantis.Stagger(50);
                        }
                    }
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