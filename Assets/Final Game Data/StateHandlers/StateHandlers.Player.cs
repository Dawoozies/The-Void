using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeObjects;
using ExtensionMethods_Bool;

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
                if(Animator.StringToHash("Player_JumpAscent") == player.animator.stateHash)
                {
                    if(!InputManager.ins.JumpDown_Input)
                    {
                        player.animator.animator.Play("Player_JumpAscentSlow");
                    }
                }
            }
        }
        public static void PhysicsUpdate(RuntimeObject obj, float tickDelta)
        {
            RuntimeObjects.Player player = obj as RuntimeObjects.Player;
            if(player != null)
            {
                player.rigidbody.rb.velocity = player.runSpeed * InputManager.ins.L_Input.x * player.right + player.upVelocity;
                if (Animator.StringToHash("Player_Fall") == player.animator.stateHash)
                {
                    player.rigidbody.rb.AddForce(-player.up * 30f);
                    if (Vector2.Dot(player.rigidbody.rb.velocity, player.up) < player.fallSpeedMax)
                        player.rigidbody.rb.velocity = Vector2.Dot(player.rigidbody.rb.velocity, player.right)*player.right + player.fallSpeedMax * player.up;
                }
                if(Animator.StringToHash("Player_JumpAscent") == player.animator.stateHash)
                {
                    if(player.animator.trueTimeSpentInState < player.jumpVelocityAddTime)
                    {
                        //jump velocity add to max
                        player.rigidbody.rb.velocity += player.up * 5f;
                        if(player.upSpeed > player.ascentSpeedMax)
                            player.rigidbody.rb.velocity = player.rightVelocity + player.ascentSpeedMax * player.up;
                    }
                    if(player.animator.trueTimeSpentInState > player.jumpVelocityAddTime)
                        player.animator.animator.Play("Player_JumpAscentSlow");
                }
                if(Animator.StringToHash("Player_JumpAscentSlow") == player.animator.stateHash)
                {
                    //jump velocity slow to zero
                    //within the apex time we have, we have to hit 0 velocity
                    //float noJumpInputFactor = InputManager.ins.JumpDown_Input.DefinedValue(300f, 0f);
                    float noJumpInputFactor = InputManager.ins.JumpDown_Input.DefinedValue(300f, 0f);
                    if (player.upSpeed > 0f)
                        player.rigidbody.rb.velocity += -player.up * (100f + noJumpInputFactor) * tickDelta;
                    if(player.upSpeed < 10f)
                    {
                        player.legs.animator.animator.Play("PlayerLegs_JumpAscentSlowPose2");
                        player.torso.animator.animator.Play("PlayerTorso_Default_JumpAscentSlowPose2");
                    }
                    if (player.upSpeed < 0f)
                        player.rigidbody.rb.velocity = player.rightVelocity;
                    if(player.upSpeed == 0f)
                    {
                        player.rigidbody.rb.velocity = (player.runSpeed + player.jumpApexRightSpeed) * InputManager.ins.L_Input.x * player.right + player.upVelocity;
                    }
                    //Debug.LogError($"Velocity Up = {player.upVelocity}");
                    if (player.animator.trueTimeSpentInState > player.jumpApexTime)
                    {
                        player.animator.animator.Play("Player_Fall");
                    }
                    if (!InputManager.ins.JumpDown_Input && player.upSpeed == 0f)
                    {
                        player.rigidbody.rb.AddForce(-player.up * 30f);
                        player.animator.animator.Play("Player_Fall");
                    }
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
                    //Debug.LogError($"Landing Started! Realtime = {Time.realtimeSinceStartup}");
                    if(player.upMagnitude <= 10.5f)
                    {
                        //Soft landing
                        player.legs.animator.animator.Play("PlayerLegs_LandPose2");
                        player.torso.animator.animator.Play("PlayerTorso_Default_MovementPoses1_3");
                        player.landingLag = 0.075f;
                    }
                    if(player.upMagnitude > 10.5f && player.upMagnitude <= 30.5f)
                    {
                        //Normal landing
                        player.legs.animator.animator.Play("PlayerLegs_LandPose1");
                        player.torso.animator.animator.Play("PlayerTorso_Default_LandPose1");
                        player.landingLag = 0.125f;
                    }
                    if(player.upMagnitude > 30.5f)
                    {
                        //Hard landing
                        player.legs.animator.animator.Play("PlayerLegs_LandPose3");
                        player.torso.animator.animator.Play("PlayerTorso_Default_BackstepPose1");
                        player.landingLag = 0.275f;
                    }
                }
                if(Animator.StringToHash("Player_JumpAscent") == player.animator.stateHash)
                {
                    if (player.grounded)
                        player.grounded = false;
                    player.legs.animator.animator.Play("PlayerLegs_JumpAscent");
                    player.torso.animator.animator.Play("PlayerTorso_Default_JumpAscent");
                }
                if(Animator.StringToHash("Player_JumpAscentSlow") == player.animator.stateHash)
                {
                    player.legs.animator.animator.Play("PlayerLegs_JumpAscentSlowPose1");
                    //If weapon held do play one with weapon held
                    player.torso.animator.animator.Play("PlayerTorso_Default_JumpAscentSlowPose1");
                }
                if(Animator.StringToHash("Player_DoubleJumpStart") == player.animator.stateHash)
                {
                    player.rigidbody.rb.velocity = Vector2.zero;
                    Debug.LogError("Double jump onStateEnter");
                    player.legs.animator.animator.Play("PlayerLegs_DoubleJumpStartPose");
                    player.torso.animator.animator.Play("PlayerTorso_Default_DoubleJumpStartPose");
                    player.obj.up = InputManager.ins.L_Input;
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