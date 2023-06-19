using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeObjects;
namespace StateHandlers.Player
{
    public static class Handler
    {
        public static void Update(RuntimeObject obj, float tickDelta)
        {
            //RuntimeObjects.Player player = obj as RuntimeObjects.Player;
            //player.rigidbody.rb.velocity = 15f * InputManager.ins.L_Input.x * player.right + player.rigidbody.upVelocity;
            //if (Animator.StringToHash("FALL") == player.animator.stateHash)
            //{
            //    player.rigidbody.rb.AddForce(-player.up*20f);
            //    if(player.rigidbody.upSpeed < player.fallSpeedMax)
            //        player.rigidbody.rb.velocity = player.rigidbody.rightVelocity + player.fallSpeedMax * player.up;
            //}
            //if(Animator.StringToHash("JUMP") == player.animator.stateHash)
            //{
            //    player.rigidbody.rb.velocity += player.up * 5f;
            //    if(player.rigidbody.upSpeed > player.ascentSpeedMax)
            //        player.rigidbody.rb.velocity = player.rigidbody.rightVelocity + player.ascentSpeedMax * player.up;
            //}
            //if(Animator.StringToHash("ASCENTSLOW") == player.animator.stateHash)
            //{
            //    player.rigidbody.rb.AddForce(-player.up*20f);
            //    if(player.rigidbody.upSpeed < player.fallSpeedMax)
            //        player.rigidbody.rb.velocity = player.rigidbody.rightVelocity + player.fallSpeedMax * player.up;
            //}
            RuntimeObjects.Player player = obj as RuntimeObjects.Player;
            if(player != null)
            {
                if (Animator.StringToHash("Player_Null") == player.animator.stateHash)
                {
                    //player.animator.animator.Play("Player_Idle");
                    player.animator.animator.Play("Player_Run");
                }
            }
        }
        public static void OnStateEnter(RuntimeObject obj, int frame, int stateHash, int previousStateHash)
        {
            RuntimeObjects.Player player = obj as RuntimeObjects.Player;
            if (player != null)
            {
                if (Animator.StringToHash("Player_Idle") == player.animator.stateHash)
                {
                    player.legs.animator.animator.Play("PlayerLegs_Idle");
                    player.torso.animator.animator.Play("PlayerTorso_Idle");
                }
                if(Animator.StringToHash("Player_Run") == player.animator.stateHash)
                {
                    player.legs.animator.animator.Play("PlayerLegs_Run");
                    player.torso.animator.animator.Play("PlayerTorso_WeaponHeld_Run");
                }
            }
        }
        public static void OnFrameUpdate(RuntimeObject obj, int frame, int stateHash, int previousStateHash)
        {
            RuntimeObjects.Player player = obj as RuntimeObjects.Player;
            if(player != null)
            {
                if (Animator.StringToHash("Player_Idle") == player.animator.stateHash)
                {
                    player.torso.obj.position = player.legs.obj.position;
                }
                if (Animator.StringToHash("Player_Run") == player.animator.stateHash)
                {
                    player.torso.obj.position = player.legs.obj.position;
                }
            }
        }
    }
}