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
                    }
                }
            }
            if(dataName == "AirRollLandBox")
            {
                RuntimeObjects.Player player = GameManager.ins.allRuntimeObjects["Player"] as RuntimeObjects.Player;
                if(player != null)
                {
                    if(player.animator.CurrentState("Player_AirRollDescent"))
                    {
                        if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                        {
                            //Debug.LogError("Air roll land area hit ground!");
                            player.animator.animator.Play("Player_AirRollLand");
                        }
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