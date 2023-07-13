using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeObjects;
using StateHandlers.Player;
using RuntimeContainers;

namespace OverlapHandlers.Player
{
    public static class OnRuntimeObjectOverlap
    {
        //RuntimeOverlap: obj.id --> hitObj.id
        public static void Handle(string dataName, RuntimeObject obj, DirectedCircleColliderContainer hitContainer, Vector2 overlapUp, Vector2 overlapRight)
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
                RuntimeObjects.Player player = GameManager.ins.FindByID("Player") as RuntimeObjects.Player;
                if(player != null)
                {
                    if(player.animator.CurrentState("Player_DoubleJumpAscentSlow"))
                    {
                        player.animator.animator.Play("Player_AirRollLand");
                    }
                    if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        player.grounded = true;
                        player.jumpsLeft = player.maxJumps;
                        Handler.onJumpsLeftChanged?.Invoke(player.jumpsLeft);
                    }
                }
            }
            if(dataName == "AirRollLandBox")
            {
                RuntimeObjects.Player player = GameManager.ins.FindByID("Player") as RuntimeObjects.Player;
                if(player != null)
                {
                    if(player.animator.CurrentState("Player_AirRollDescent"))
                    {
                        if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                        {
                            player.animator.animator.Play("Player_AirRollLand");
                        }
                    }
                }
            }
            if(dataName == "WallSlamDetection")
            {
                RuntimeObjects.Player player = GameManager.ins.FindByID("Player") as RuntimeObjects.Player;
                //Debug.LogError($"Slammed into wall with velocity {player.rigidbody.rb.velocity}");
            }
        }
    }
    public static class OnNullResult
    {
        public static void Handle(string dataName, RuntimeObject obj)
        {
            if (dataName == "Groundbox")
            {
                RuntimeObjects.Player player = GameManager.ins.FindByID("Player") as RuntimeObjects.Player;
                if (player != null)
                {
                    player.grounded = false;
                }
            }
        }
    }
}