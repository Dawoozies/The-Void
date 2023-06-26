using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeObjects;
using ExtensionMethods_Bool;
using UnityEngine.InputSystem;

namespace StateHandlers.Player
{
    public static class Handler
    {
        //By the time we get here all runtime data has been set and updated
        public static void Update(RuntimeObject obj, float tickDelta)
        {
            RuntimeObjects.Player player = obj as RuntimeObjects.Player;
            if(player != null)
            {
                
                if(player.animator.CurrentState("Player_Idle"))
                {
                    //transitions at top happen least important
                    if (player.jumpsLeft > 0 && InputManager.ins.JumpDown_BufferedInput)
                        player.animator.animator.Play("Player_JumpAscent");
                    if (InputManager.ins.L_Input.x != 0f)
                        player.animator.animator.Play("Player_Run");
                    if (!player.grounded)
                        player.animator.animator.Play("Player_Fall");
                    //transitions at bottom most important
                }
                if (player.animator.CurrentState("Player_Run"))
                {
                    if (player.jumpsLeft > 0 && InputManager.ins.JumpDown_BufferedInput)
                        player.animator.animator.Play("Player_JumpAscent");
                    if (InputManager.ins.L_Input.x == 0f)
                        player.animator.animator.Play("Player_Idle");

                    if (player.legs.animator.spriteRenderer.flipX && InputManager.ins.L_Input.x > 0f)
                        player.legs.animator.spriteRenderer.flipX = false;
                    if(!player.legs.animator.spriteRenderer.flipX && InputManager.ins.L_Input.x < 0f)
                        player.legs.animator.spriteRenderer.flipX = true;
                    if (player.torso.animator.spriteRenderer.flipX && InputManager.ins.L_Input.x > 0f)
                        player.torso.animator.spriteRenderer.flipX = false;
                    if (!player.torso.animator.spriteRenderer.flipX && InputManager.ins.L_Input.x < 0f)
                        player.torso.animator.spriteRenderer.flipX = true;
                }
                if(player.animator.CurrentState("Player_Fall"))
                {
                    if (player.grounded)
                        player.animator.animator.Play("Player_Land");
                }
                if(player.animator.CurrentState("Player_Land"))
                {
                    //Debug.Log("Is this happening?");
                    if(player.animator.trueTimeSpentInState >= player.landingLag)
                    {
                        player.animator.animator.Play("Player_Idle");
                        //Debug.LogError($"Ending Land! Realtime = {Time.realtimeSinceStartup} TrueTimeSpentInState = {player.animator.trueTimeSpentInState}");
                    }   
                }
                if(player.animator.CurrentState("Player_JumpAscent"))
                {
                    if(!InputManager.ins.JumpDown_Input)
                    {
                        player.animator.animator.Play("Player_JumpAscentSlow");
                    }
                }
                if(player.animator.CurrentState("Player_DoubleJumpStart"))
                {
                    if(player.animator.trueTimeSpentInState >= player.doubleJumpStartTime)
                    {
                        player.animator.animator.Play("Player_DoubleJumpAscent");
                    }
                }
                if(player.animator.CurrentState("Player_DoubleJumpAscent"))
                {
                    if (player.animator.trueTimeSpentInState > player.doubleJumpVelocityAddTime)
                    {
                        player.animator.animator.Play("Player_DoubleJumpAscentSlow");
                    }
                }
            }
        }
        //By the time we get here all runtime data has been set and updated
        public static void PhysicsUpdate(RuntimeObject obj, float tickDelta)
        {
            RuntimeObjects.Player player = obj as RuntimeObjects.Player;
            if(player != null)
            {
                bool noLMove =
                    player.animator.CurrentState("Player_DoubleJumpStart")
                    || player.animator.CurrentState("Player_DoubleJumpAscent")
                    || player.animator.CurrentState("Player_DoubleJumpAscentSlow")
                    || player.animator.CurrentState("Player_Backflip");
                if(!noLMove)
                    player.rigidbody.rb.velocity = player.runSpeed * InputManager.ins.L_Input.x * player.right + player.upVelocity;
                if (player.animator.CurrentState("Player_Fall"))
                {
                    player.rigidbody.rb.velocity += -player.up * 110f * tickDelta;
                    if (player.upSpeed < player.fallSpeedMax)
                        player.rigidbody.rb.velocity = player.rightVelocity + player.fallSpeedMax * player.up;
                }
                if(player.animator.CurrentState("Player_JumpAscent"))
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
                if(player.animator.CurrentState("Player_JumpAscentSlow"))
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
                        player.rigidbody.rb.velocity += -player.up * 120f * tickDelta;
                        player.animator.animator.Play("Player_Fall");
                    }
                }
                if(player.animator.CurrentState("Player_DoubleJumpAscent"))
                {
                    //Debug.LogError("Double jump ascent physics update happening now");
                    //DirectedPoint upOverride = player.legs.directedPoints.atFrame["VelocityUpDirectionOverride"];
                    if(player.animator.trueTimeSpentInState < player.doubleJumpVelocityAddTime)
                    {
                        player.rigidbody.rb.velocity += player.up * 10f;
                        if(player.upSpeed > player.ascentSpeedMax)
                            player.rigidbody.rb.velocity = player.ascentSpeedMax * player.up;
                    }
                }
                if(player.animator.CurrentState("Player_DoubleJumpAscentSlow"))
                {
                    int pose = 1;
                    if (player.upSpeed > 15)
                        pose = 1;
                    if (10 < player.upSpeed && player.upSpeed <= 15)
                        pose = 2;
                    if (5 < player.upSpeed && player.upSpeed <= 10)
                        pose = 3;
                    if (0 < player.upSpeed && player.upSpeed <= 5)
                        pose = 4;
                    if (-5 < player.upSpeed && player.upSpeed <= 0)
                        pose = 5;
                    if (-10 < player.upSpeed && player.upSpeed <= -5)
                        pose = 6;
                    if (player.upSpeed <= -10)
                        pose = 7;
                    switch (player.jumpType)
                    {

                        case "Double Jump":
                            player.rigidbody.rb.velocity += -player.up * 100f * tickDelta;
                            if(player.upSpeed < 12f)
                            {
                                player.legs.animator.animator.Play("PlayerLegs_JumpAscentSlowPose2");
                                player.torso.animator.animator.Play("PlayerTorso_Default_JumpAscentSlowPose2");
                            }
                            if (player.upSpeed < -4f)
                                player.animator.animator.Play("Player_Fall");
                            break;
                        case "Front Flip":
                            player.rigidbody.rb.velocity += -player.up * 75f * tickDelta;
                            if(pose == 6 || pose == 7)
                            {
                                player.legs.animator.animator.Play("PlayerLegs_BackflipPose6");
                                player.torso.animator.animator.Play("PlayerTorso_Default_BackflipPose6");
                            }
                            else
                            {
                                player.legs.animator.animator.Play($"PlayerLegs_FrontflipPose{pose}");
                                player.torso.animator.animator.Play($"PlayerTorso_Default_FrontflipPose{pose}");
                            }
                            if (player.upSpeed < -24)
                                player.animator.animator.Play("Player_Fall");
                            break;
                        case "Back Flip":
                            player.rigidbody.rb.velocity += -player.up * 75f * tickDelta;
                            if(pose == 7)
                            {
                                player.legs.animator.animator.Play($"PlayerLegs_BackflipPose6");
                                player.torso.animator.animator.Play($"PlayerTorso_Default_BackflipPose6");
                            }
                            else
                            {
                                player.legs.animator.animator.Play($"PlayerLegs_BackflipPose{pose}");
                                player.torso.animator.animator.Play($"PlayerTorso_Default_BackflipPose{pose}");
                            }
                            if (player.upSpeed < -24)
                                player.animator.animator.Play("Player_Fall");
                            break;
                        case "Front Spin":
                            player.rigidbody.rb.velocity += -player.up * 55f * tickDelta;
                            if(pose == 1 || pose == 2 || pose == 3)
                            {
                                player.legs.animator.animator.Play($"PlayerLegs_FrontspinPose{pose}");
                                player.torso.animator.animator.Play($"PlayerTorso_Default_FrontspinPose{pose}");
                            }
                            if (pose == 4 || pose == 5 || pose == 6)
                            {
                                player.legs.animator.animator.Play($"PlayerLegs_FrontflipPose{pose - 1}");
                                player.torso.animator.animator.Play($"PlayerTorso_Default_FrontflipPose{pose - 1}");
                            }
                            if(pose == 7)
                            {
                                player.legs.animator.animator.Play($"PlayerLegs_BackflipPose6");
                                player.torso.animator.animator.Play($"PlayerTorso_Default_BackflipPose6");
                            }
                            if (player.upSpeed < -24)
                                player.animator.animator.Play("Player_Fall");
                            break;
                        case "Back Spin":
                            player.rigidbody.rb.velocity += -player.up * 55f * tickDelta;
                            if (pose == 1 || pose == 2 || pose == 3)
                            {
                                player.legs.animator.animator.Play($"PlayerLegs_BackspinPose{pose}");
                                player.torso.animator.animator.Play($"PlayerTorso_Default_BackspinPose{pose}");
                            }
                            else
                            {
                                player.legs.animator.animator.Play($"PlayerLegs_BackflipPose{pose - 1}");
                                player.torso.animator.animator.Play($"PlayerTorso_Default_BackflipPose{pose - 1}");
                            }
                            if (player.upSpeed < -24)
                                player.animator.animator.Play("Player_Fall");
                            break;
                    }
                }
                if(player.animator.CurrentState("Player_Backflip"))
                {
                    player.rigidbody.rb.velocity += -player.up * 75f * tickDelta;
                    //Debug.LogError($"Up Speed = {player.upSpeed}");
                    int pose = 1;
                    if (player.upSpeed > 12)
                        pose = 1;
                    if (6 < player.upSpeed && player.upSpeed <= 12)
                        pose = 2;
                    if(0 < player.upSpeed && player.upSpeed <= 6)
                        pose = 3;
                    if(-12 < player.upSpeed && player.upSpeed <= 0)
                        pose = 4;
                    if(-18 < player.upSpeed && player.upSpeed <= -12)
                        pose = 5;
                    if(player.upSpeed <= -18)
                        pose = 6;
                    player.legs.animator.animator.Play($"PlayerLegs_BackflipPose{pose}");
                    player.torso.animator.animator.Play($"PlayerTorso_Default_BackflipPose{pose}");
                    if (player.upSpeed < -24)
                        player.animator.animator.Play("Player_Fall");
                }
            }
        }
        public static void OnStateEnter(RuntimeObject obj, int frame, int stateHash, int previousStateHash)
        {
            RuntimeObjects.Player player = obj as RuntimeObjects.Player;
            if (player != null)
            {
                if (player.animator.CurrentState("Player_Idle"))
                {
                    player.legs.animator.animator.Play("PlayerLegs_Idle");
                    player.torso.animator.animator.Play("PlayerTorso_Idle");
                }
                if(player.animator.CurrentState("Player_Run"))
                {
                    player.legs.animator.animator.Play("PlayerLegs_Run");
                    //If weapon held then do PlayerTorso_WeaponHeld_Run
                    player.torso.animator.animator.Play("PlayerTorso_Default_Run");
                }
                if(player.animator.CurrentState("Player_Fall"))
                {
                    player.legs.animator.animator.Play("PlayerLegs_Fall");
                    player.torso.animator.animator.Play("PlayerTorso_Default_Fall");
                }
                if(player.animator.CurrentState("Player_Land"))
                {
                    //Debug.LogError($"Landing Started! Realtime = {Time.realtimeSinceStartup}");
                    player.jumpsLeft = player.maxJumps;
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
                if(player.animator.CurrentState("Player_JumpAscent"))
                {
                    if (player.grounded)
                        player.grounded = false;
                    player.legs.animator.animator.Play("PlayerLegs_JumpAscent");
                    player.torso.animator.animator.Play("PlayerTorso_Default_JumpAscent");
                    player.jumpsLeft--;
                }
                if(player.animator.CurrentState("Player_JumpAscentSlow"))
                {
                    //the up direction may not always be the default up
                    //but for prototype purposes it is
                    player.legs.animator.animator.Play("PlayerLegs_JumpAscentSlowPose1");
                    //If weapon held do play one with weapon held
                    player.torso.animator.animator.Play("PlayerTorso_Default_JumpAscentSlowPose1");
                }
                if(player.animator.CurrentState("Player_DoubleJumpStart"))
                {
                    //player.rigidbody.rb.velocity = Vector2.zero;
                    //Debug.LogError("Double jump onStateEnter");
                    player.legs.animator.animator.Play("PlayerLegs_DoubleJumpStartPose");
                    player.torso.animator.animator.Play("PlayerTorso_Default_DoubleJumpStartPose");
                    player.obj.up = new Vector3(InputManager.ins.L_Input.x, Mathf.Abs(InputManager.ins.L_Input.y), 0f);
                    player.jumpsLeft--;
                }
                if(player.animator.CurrentState("Player_DoubleJumpAscent"))
                {
                    player.rigidbody.rb.velocity = player.rightVelocity;
                    //Debug.LogError("Double jump ascent onStateEnter");
                    player.legs.animator.animator.Play("PlayerLegs_DoubleJumpAscentPose");
                    player.torso.animator.animator.Play("PlayerTorso_Default_DoubleJumpAscentPose");
                    player.rigidbody.rbObj.position = player.RelativePos(player.doubleJumpShift);
                }
                if(player.animator.CurrentState("Player_DoubleJumpAscentSlow"))
                {
                    player.obj.up = Vector2.up;
                    switch (player.jumpType)
                    {
                        case "Double Jump":
                            player.legs.animator.animator.Play("PlayerLegs_JumpAscentSlowPose1");
                            player.torso.animator.animator.Play("PlayerTorso_Default_JumpAscentSlowPose1");
                            break;
                        case "Front Flip":
                            player.legs.animator.animator.Play("PlayerLegs_FrontflipPose1");
                            player.torso.animator.animator.Play("PlayerTorso_Default_FrontflipPose1");
                            break;
                        case "Back Flip":
                            player.legs.animator.animator.Play("PlayerLegs_BackflipPose1");
                            player.torso.animator.animator.Play("PlayerTorso_Default_BackflipPose1");
                            break;
                        case "Front Spin":
                            player.legs.animator.animator.Play("PlayerLegs_FrontspinPose1");
                            player.torso.animator.animator.Play("PlayerTorso_Default_FrontspinPose1");
                            break;
                        case "Back Spin":
                            player.legs.animator.animator.Play("PlayerLegs_BackspinPose1");
                            player.torso.animator.animator.Play("PlayerTorso_Default_BackspinPose1");
                            break;
                    }
                }
            }
        }
        public static void OnFrameUpdate(RuntimeObject obj, int frame, int stateHash, int previousStateHash)
        {
            RuntimeObjects.Player player = obj as RuntimeObjects.Player;
            if(player != null)
            {
                if (player.animator.CurrentState("Player_Idle"))
                {
                    player.torso.obj.position = player.legs.obj.position;
                }
                if (player.animator.CurrentState("Player_Run"))
                {
                    player.torso.obj.position = player.legs.obj.position;
                }
            }
        }
        public static void OnJumpPerformed(bool jumpInput)
        {
            RuntimeObjects.Player player = GameManager.ins.allRuntimeObjects["Player"] as RuntimeObjects.Player;
            if (player != null)
            {

                bool canJump =
                    player.animator.CurrentState("Player_Idle")
                    || player.animator.CurrentState("Player_Run");
                if (canJump)
                {
                    if (player.jumpsLeft > 0 && jumpInput)
                    {
                        player.animator.animator.Play("Player_JumpAscent");
                    }
                }
                bool canJumpInAir =
                player.animator.CurrentState("Player_JumpAscentSlow")
                || player.animator.CurrentState("Player_Fall")
                || player.animator.CurrentState("Player_DoubleJumpAscent")
                || player.animator.CurrentState("Player_DoubleJumpAscentSlow");
                if (canJumpInAir && jumpInput)
                {
                    if(player.jumpsLeft > 0)
                    {
                        if (player.jumpsLeft == 3)
                        {
                            player.jumpType = "Double Jump";
                        }
                        if (player.jumpsLeft == 2)
                        {
                            if (Mathf.Sign(InputManager.ins.L_Input.x) == Mathf.Sign(player.legs.animator.spriteRenderer.flipX.DefinedValue(1, -1)))
                                player.jumpType = "Front Flip";
                            else
                                player.jumpType = "Back Flip";
                        }
                        if (player.jumpsLeft == 1)
                        {
                            if (Mathf.Sign(InputManager.ins.L_Input.x) == Mathf.Sign(player.legs.animator.spriteRenderer.flipX.DefinedValue(1, -1)))
                                player.jumpType = "Front Spin";
                            else
                                player.jumpType = "Back Spin";
                        }
                        if (player.jumpsLeft > 0)
                            player.animator.animator.Play("Player_DoubleJumpStart");
                    }
                }
            }
        }
    }
}