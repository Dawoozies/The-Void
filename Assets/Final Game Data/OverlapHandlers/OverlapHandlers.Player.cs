using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeObjects;
namespace OverlapHandlers.Player
{
    public static class OnRuntimeObjectOverlap
    {
        //RuntimeOverlap: obj.id --> hitObj.id
        public static void Handle(RuntimeObject obj, RuntimeObject hitObj)
        {

        }
    }
    public static class OnNonRuntimeObjectOverlap
    {
        //NonRuntimeOverlap: obj.id --> LayerMask.LayerToName(hitCollider.gameObject.layer)
        public static void Handle(RuntimeObject obj, Collider2D hitCollider)
        {
            RuntimeObjects.Player player = obj as RuntimeObjects.Player;
            if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                player.animator.animator.SetBool("Grounded", true);
            }
        }
    }
}