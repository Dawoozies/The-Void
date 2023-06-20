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
            RuntimeObjects.Player player = obj as RuntimeObjects.Player;
            if(player != null)
            {
                
                if(Animator.StringToHash("Player_Idle") == player.animator.stateHash)
                {
                    //transitions at top happen least important
                    if (InputManager.ins.L_Input.x != 0f)
                        player.animator.animator.Play("Player_Run");
                    if (!player.grounded)
                        player.animator.animator.Play("Player_Fall");
                    if(InputManager.ins.JumpDown_Input || InputManager.ins.JumpDown_BufferedInput)
                        player.animator.animator.Play("Player_JumpAscent");
                    //transitions at bottom most important
                }
                if (Animator.StringToHash("Player_Run") == player.animator.stateHash)
                {
                    if (InputManager.ins.L_Input.x == 0f)
                        player.animator.animator.Play("Player_Idle");
                    if (InputManager.ins.JumpDown_Input || InputManager.ins.JumpDown_BufferedInput)
                        player.animator.animator.Play("Player_JumpAscent");

                    if (player.legs.animator.spriteRenderer.flipX && InputManager.ins.L_Input.x > 0f)
                        player.legs.animator.spriteRenderer.flipX = false;
                    if(!player.legs.animator.spriteRenderer.flipX && InputManager.ins.L_Input.x < 0f)
                        player.legs.animator.spriteRenderer.flipX = true;
                    if (player.torso.animator.spriteRenderer.flipX && InputManager.ins.L_Input.x > 0f)
                        player.torso.animator.spriteRenderer.flipX = false;
                    if (!player.torso.animator.spriteRenderer.flipX && InputManager.ins.L_Input.x < 0f)
                        player.torso.animator.spriteRenderer.flipX = true;
                }
                if(Animator.StringToHash("Player_Fall") == player.animator.stateHash)
                {
                    if (player.grounded)
                        player.animator.animator.Play("Player_Land");
                }
                if(Animator.StringToHash("Player_Land") == player.animator.stateHash)
                {
                    //Debug.Log("Is this happening?");
                    if(player.animator.trueTimeSpentInState >= player.landingLag)
                    {
                        player.animator.animator.Play("Player_Idle");
                        //Debug.LogError($"Ending Land! Realtime = {Time.realtimeSinceStartup} TrueTimeSpentInState = {player.animator.trueTimeSpentInState}");
                    }   
                }
            }
        }
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
                    player.rigidbody.rb.AddForce(-player.up * 30f);
                    if (Vector2.Dot(player.rigidbody.rb.velocity, player.up) < player.fallSpeedMax)
                        player.rigidbody.rb.velocity = Vector2.Dot(player.rigidbody.rb.velocity, player.right)*player.right + player.fallSpeedMax * player.up;
                }
                if(Animator.StringToHash("Player_JumpAscent") == player.animator.stateHash)
                {
                    player.rigidbody.rb.AddForce(player.up * 300f);
                    if(player.animator.trueTimeSpentInState < 0.2f)
                        player.rigidbody.rb.velocity = player.rigidbody.rightVelocity + player.ascentSpeedMax * player.up;
                }
                player.rigidbody.rb.velocity = player.runSpeed * InputManager.ins.L_Input.x * player.right + player.rigidbody.upVelocity;
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
                    //Debug.LogError($"Landing Started! Realtime = {Time.realtimeSinceStartup}");
                    if(player.rigidbody.upMagnitude <= 10.5f)
                    {
                        //Soft landing
                        player.legs.animator.animator.Play("PlayerLegs_LandPose2");
                        player.torso.animator.animator.Play("PlayerTorso_Default_MovementPoses1_3");
                        player.landingLag = 0.075f;
                    }
                    if(player.rigidbody.upMagnitude > 10.5f && player.rigidbody.upMagnitude <= 30.5f)
                    {
                        //Normal landing
                        player.legs.animator.animator.Play("PlayerLegs_LandPose1");
                        player.torso.animator.animator.Play("PlayerTorso_Default_LandPose1");
                        player.landingLag = 0.125f;
                    }
                    if(player.rigidbody.upMagnitude > 30.5f)
                    {
                        //Hard landing
                        player.legs.animator.animator.Play("PlayerLegs_LandPose3");
                        player.torso.animator.animator.Play("PlayerTorso_Default_BackstepPose1");
                        player.landingLag = 0.275f;
                    }
                }
                if(Animator.StringToHash("Player_JumpAscent") == player.animator.stateHash)
                {
                    player.legs.animator.animator.Play("PlayerLegs_JumpAscent");
                    player.torso.animator.animator.Play("PlayerTorso_Default_JumpAscent");
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