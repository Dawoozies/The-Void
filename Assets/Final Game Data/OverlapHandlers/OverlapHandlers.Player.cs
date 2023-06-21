using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeObjects;
namespace OverlapHandlers.Player
{
    public static class OnRuntimeObjectOverlap
    {
        //RuntimeOverlap: obj.id --> hitObj.id
        public static void Handle(string dataName, RuntimeObject obj, RuntimeObject hitObj)
        {

        }
    }
    public static class OnNonRuntimeObjectOverlap
    {
        //NonRuntimeOverlap: obj.id --> LayerMask.LayerToName(hitCollider.gameObject.layer)
        public static void Handle(string dataName, RuntimeObject obj, Collider2D hitCollider)
        {
            if(dataName == "Groundbox")
            {
                RuntimeObjects.Player player = GameManager.ins.allRuntimeObjects["Player"] as RuntimeObjects.Player;
                if(player != null)
                {
                    if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        player.grounded = true;
                        player.jumpsLeft = player.maxJumps;
                    }
                }
            }
        }
    }
    public static class OnNullResult
    {
        public static void Handle(string dataName, RuntimeObject obj)
        {
            if (dataName == "Groundbox")
            {
                RuntimeObjects.Player player = GameManager.ins.allRuntimeObjects["Player"] as RuntimeObjects.Player;
                if (player != null)
                {
                    player.grounded = false;
                }
            }
        }
    }
}