using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeObjects;
namespace StateHandlers.Player
{
    public static class Handler
    {
        public static void PhysicsUpdate(RuntimeObject obj, float tickDelta)
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
                if (Animator.StringToHash("Player_Fall") == player.animator.stateHash)
                {
                    player.rigidbody.rb.AddForce(-player.up * 20f);
                    if (player.rigidbody.upSpeed < player.fallSpeedMax)
                        player.rigidbody.rb.velocity = player.rigidbody.rightVelocity + player.fallSpeedMax * player.up;
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
                    //If weapon held then do PlayerTorso_WeaponHeld_Run
                    player.torso.animator.animator.Play("PlayerTorso_Default_Run");
                }
                if(Animator.StringToHash("Player_Fall") == player.animator.stateHash)
                {
                    player.legs.animator.animator.Play("PlayerLegs_Fall");
                    player.torso.animator.animator.Play("PlayerTorso_Default_Fall");
                }
                if(Animator.StringToHash("Player_Land") == player.animator.stateHash)
                {
                    if(player.rigidbody.upMagnitude <= 10.5f)
                    {
                        //Soft landing
                        player.legs.animator.animator.Play("PlayerLegs_LandPose2");
                        player.torso.animator.animator.Play("PlayerTorso_Default_MovementPoses1_3");
                    }
                    if(player.rigidbody.upMagnitude > 10.5f && player.rigidbody.upMagnitude <= 20.5f)
                    {
                        //Normal landing
                        player.legs.animator.animator.Play("PlayerLegs_LandPose1");
                        player.torso.animator.animator.Play("PlayerTorso_Default_LandPose1");
                    }
                    if(player.rigidbody.upMagnitude > 20.5f)
                    {
                        //Hard landing
                        player.legs.animator.animator.Play("PlayerLegs_LandPose3");
                        player.torso.animator.animator.Play("PlayerTorso_Default_BackstepPose1");
                    }
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