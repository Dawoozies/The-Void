using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeObjects;
using ExtensionMethods_Bool;
using RuntimeContainers;
using System;
namespace StateHandlers.Player
{
    public enum PlayerAction
    {
        Jump = 0,
        JumpInAir = 1,
        WeaponThrow = 2,
        WeaponMelee = 3,
        LeftStickMovement = 4,
        Fall = 5,
    }

    public static class Handler
    {
        const float FALL_SPEED_MAX = -30f;
        const float RUN_SPEED = 15f;
        const float JUMP_VELOCITY_ADD_TIME = 0.175f;
        const float HIT_STUN_MAX = 0.125f;
        const float CATCH_TIME = 0.15f;
        static readonly string[] CAN_JUMP = {
            "Player_Idle",
            "Player_Run",
            "Player_AirRollLand",
            "Player_Slide",
        };
        static readonly string[] CAN_JUMP_IN_AIR = {
            "Player_JumpAscentSlow",
            "Player_Fall",
            "Player_DoubleJumpAscent",
            "Player_DoubleJumpAscentSlow",
            "Player_AirRollDescent",
            "Player_TwoHandedStab",
        };
        static readonly string[] CAN_THROW = { 
            "Player_Idle", 
            "Player_Run",
            "Player_JumpAscent",
            "Player_JumpAscentSlow",
            "Player_Fall",
            "Player_Land",
            "Player_AirRollDescent",
            "Player_AirRollLand",
            "Player_DoubleJumpStart",
            "Player_DoubleJumpAscent",
            "Player_DoubleJumpAscentSlow",
            "Player_Slide",
        };
        static readonly string[] CAN_MELEE = {
            "Player_Idle",
            "Player_Run",
            "Player_JumpAscent",
            "Player_JumpAscentSlow",
            "Player_Fall",
            "Player_Land",
            "Player_AirRollDescent",
            "Player_AirRollLand",
            "Player_DoubleJumpStart",
            "Player_DoubleJumpAscent",
            "Player_DoubleJumpAscentSlow",
            "Player_Slide",
        };
        static readonly string[] CAN_LEFT_STICK_MOVEMENT = {
            "Player_Idle",
            "Player_Run",
            "Player_JumpAscent",
            "Player_JumpAscentSlow",
            "Player_Fall",
            "Player_AirRollLand",
            "Player_TwoHandedStabReady",
            "Player_TwoHandedStab",
        };
        static readonly string[] CAN_FALL = {
            "Player_Idle",
            "Player_Fall",
            "Player_TwoHandedStab",
        };
        static bool CanDoAction(RuntimeAnimator runtimeAnimator, PlayerAction action)
        {
            switch (action)
            {
                case PlayerAction.Jump:
                    return RuntimeAnimator.CheckStates(runtimeAnimator, CAN_JUMP);
                case PlayerAction.JumpInAir:
                    return RuntimeAnimator.CheckStates(runtimeAnimator, CAN_JUMP_IN_AIR);
                case PlayerAction.WeaponThrow:
                    return RuntimeAnimator.CheckStates(runtimeAnimator, CAN_THROW);
                case PlayerAction.WeaponMelee:
                    return RuntimeAnimator.CheckStates(runtimeAnimator, CAN_MELEE);
                case PlayerAction.LeftStickMovement:
                    return RuntimeAnimator.CheckStates(runtimeAnimator, CAN_LEFT_STICK_MOVEMENT);
                case PlayerAction.Fall:
                    return RuntimeAnimator.CheckStates(runtimeAnimator, CAN_FALL);
            }
            return false;
        }
        //By the time we get here all runtime data has been set and updated
        public static Action onAttackBlur;
        public static Action<int> onJumpsLeftChanged;
        public static void Update(RuntimeObject obj, float tickDelta)
        {
            RuntimeObjects.Player player = obj as RuntimeObjects.Player;
            if(player != null)
            {
                
                if(player.animator.CurrentState("Player_Idle"))
                {
                    if (InputManager.ins.L_Input.x != 0f)
                        player.animator.animator.Play("Player_Run");
                    if (!player.grounded)
                        player.animator.animator.Play("Player_Fall");
                    //transitions at bottom most important
                }
                if(player.animator.CurrentState("Player_Run"))
                {
                    if (InputManager.ins.L_Input.x == 0f)
                        player.animator.animator.Play("Player_Idle");
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
                        if(player.animator.trueTimeSpentInState > 0.17f)
                            player.animator.animator.Play("Player_JumpAscentSlow");
                    }
                }
                if(player.animator.CurrentState("Player_DoubleJumpStart"))
                {
                    if(player.animator.trueTimeSpentInState >= player.doubleJumpStartTime)
                        player.animator.animator.Play("Player_DoubleJumpAscent");
                }
                if(player.animator.CurrentState("Player_DoubleJumpAscent"))
                {
                    if (player.animator.trueTimeSpentInState > player.doubleJumpVelocityAddTime)
                    {
                        player.animator.animator.Play("Player_DoubleJumpAscentSlow");
                    }
                }
                if(player.animator.CurrentState("Player_AirRollStart"))
                {
                    if (player.animator.trueTimeSpentInState >= player.airRollStartTime)
                        player.animator.animator.Play("Player_AirRollDescent");
                }
                if(player.animator.CurrentState("Player_AirRollLand"))
                {
                    //Debug.LogError("Air roll land first update");
                    if(player.animator.trueTimeSpentInState > player.airRollLandBufferTime)
                    {
                        //if we go above the buffer time then player will default out into standing up
                        //Debug.LogError("Input Buffer Time Ended");
                        //Late land
                        player.animator.animator.Play("Player_Land");
                    }
                    else
                    {
                        if(InputManager.ins.L_Input.y < 0)
                        {
                            //Then we are pointing down
                            //Then we check if our y input is larger in magnitude than our x input
                            //Early land
                            if(Mathf.Abs(InputManager.ins.L_Input.y) > Mathf.Abs(InputManager.ins.L_Input.x))
                            {
                                //Debug.LogError("Want to hard land");
                                player.animator.animator.Play("Player_Land");
                            }
                        }
                        if(InputManager.ins.L_Input.x != 0f)
                        {
                            if(Mathf.Abs(InputManager.ins.L_Input.x) > Mathf.Abs(InputManager.ins.L_Input.y))
                            {
                                //Debug.LogError("Want to slide along ground");
                                player.animator.animator.Play("Player_Slide");
                            }
                        }
                    }
                }
                if(player.animator.CurrentState("Player_Slide"))
                {
                    if (player.legs.animator.spriteRenderer.flipX && player.rigidbody.rb.velocity.x > 0f)
                        player.legs.animator.spriteRenderer.flipX = false;
                    if (!player.legs.animator.spriteRenderer.flipX && player.rigidbody.rb.velocity.x < 0f)
                        player.legs.animator.spriteRenderer.flipX = true;
                    if (player.torso.animator.spriteRenderer.flipX && player.rigidbody.rb.velocity.x > 0f)
                        player.torso.animator.spriteRenderer.flipX = false; 
                    if (!player.torso.animator.spriteRenderer.flipX && player.rigidbody.rb.velocity.x < 0f)
                        player.torso.animator.spriteRenderer.flipX = true;
                    if (player.animator.trueTimeSpentInState > player.slideMaxTime)
                    {
                        player.animator.animator.Play("Player_Land");
                    }
                }
                if(player.animator.CurrentState("Player_Damaged"))
                {
                    if (player.animator.trueTimeSpentInState > HIT_STUN_MAX)
                        player.animator.animator.Play("Player_Idle");
                }
                if(player.animator.CurrentState("Player_CatchWeapon"))
                {
                    if (player.animator.trueTimeSpentInState > CATCH_TIME)
                    {
                        player.torso.animator.animator.Play("PlayerTorso_Throw_Pose4");
                    }
                    if (player.animator.trueTimeSpentInState > CATCH_TIME + 0.25f)
                        player.animator.animator.Play("Player_Idle");
                }
                if(player.animator.CurrentState("Player_ThrowWeapon"))
                {
                    if(player.animator.trueTimeSpentInState > 0.12f)
                    {
                        player.torso.animator.animator.Play("PlayerTorso_Throw_Pose4");
                    }
                    if(player.animator.trueTimeSpentInState > 0.22f)
                    {
                        player.animator.animator.Play("Player_Idle");
                    }
                }
                if(player.animator.CurrentState("Player_TwoHandedStabReady"))
                {
                    int pose = 1;
                    if (player.animator.trueTimeSpentInState > 0.06f)
                        pose = 2;
                    player.torso.animator.animator.Play($"PlayerTorso_TwoHanded_Stab_Ready_Pose{pose}");
                    if (player.animator.trueTimeSpentInState > 0.14f)
                        player.animator.animator.Play("Player_TwoHandedStab");
                }
                if(player.animator.CurrentState("Player_TwoHandedStab"))
                {
                    if (player.animator.trueTimeSpentInState > 0.11f)
                        player.torso.animator.animator.Play("PlayerTorso_TwoHanded_Stab_Ready_Pose3");
                    if (player.animator.trueTimeSpentInState > 0.24f)
                        player.animator.animator.Play("Player_Idle");
                }
                bool spriteFlipping =
                    player.animator.CurrentState("Player_Run")
                    || player.animator.CurrentState("Player_Slide");
                if(spriteFlipping)
                {
                    if (player.legs.animator.spriteRenderer.flipX && InputManager.ins.L_Input.x > 0f)
                        player.legs.animator.spriteRenderer.flipX = false;
                    if (!player.legs.animator.spriteRenderer.flipX && InputManager.ins.L_Input.x < 0f)
                        player.legs.animator.spriteRenderer.flipX = true;
                    if (player.torso.animator.spriteRenderer.flipX && InputManager.ins.L_Input.x > 0f)
                        player.torso.animator.spriteRenderer.flipX = false;
                    if (!player.torso.animator.spriteRenderer.flipX && InputManager.ins.L_Input.x < 0f)
                        player.torso.animator.spriteRenderer.flipX = true;
                }
            }
        }
        //By the time we get here all runtime data has been set and updated
        public static void PhysicsUpdate(RuntimeObject obj, float tickDelta)
        {
            RuntimeObjects.Player player = obj as RuntimeObjects.Player;
            if(player != null)
            {
                //bool noLMove =
                //    player.animator.CurrentState("Player_DoubleJumpStart")
                //    || player.animator.CurrentState("Player_DoubleJumpAscent")
                //    || player.animator.CurrentState("Player_DoubleJumpAscentSlow")
                //    || player.animator.CurrentState("Player_AirRollStart")
                //    || player.animator.CurrentState("Player_AirRollDescent")
                //    || player.animator.CurrentState("Player_Slide")
                //    || player.animator.CurrentState("Player_Land");
                //if(!noLMove)
                //    player.rigidbody.rb.velocity = player.runSpeed * InputManager.ins.L_Input.x * player.right + player.upVelocity;
                //bool canLeftStickMovement = CanDoAction(StateAction.LeftStickMovement);
                bool LeftStickMovement = CanDoAction(player.animator, PlayerAction.LeftStickMovement);
                bool Fall = CanDoAction(player.animator, PlayerAction.Fall);
                if(LeftStickMovement)
                {
                    player.rigidbody.rb.velocity = RUN_SPEED * InputManager.ins.L_Input.x * player.right + player.upVelocity;
                }
                if(Fall)
                {
                    player.rigidbody.rb.velocity += -player.up * 110f * tickDelta;
                    if (player.upSpeed < FALL_SPEED_MAX)
                        player.rigidbody.rb.velocity = player.rightVelocity + FALL_SPEED_MAX * player.up;
                }
                //if (player.animator.CurrentState("Player_Fall"))
                //{
                //    player.rigidbody.rb.velocity += -player.up * 110f * tickDelta;
                //    if (player.upSpeed < player.fallSpeedMax)
                //        player.rigidbody.rb.velocity = player.rightVelocity + player.fallSpeedMax * player.up;
                //}
                if(player.animator.CurrentState("Player_JumpAscent"))
                {
                    if(player.animator.trueTimeSpentInState < JUMP_VELOCITY_ADD_TIME)
                    {
                        //jump velocity add to max
                        player.rigidbody.rb.velocity += player.up * 5f;
                        if(player.upSpeed > player.ascentSpeedMax)
                            player.rigidbody.rb.velocity = player.rightVelocity + player.ascentSpeedMax * player.up;
                    }
                    if(player.animator.trueTimeSpentInState > JUMP_VELOCITY_ADD_TIME)
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
                        player.rigidbody.rb.velocity = (RUN_SPEED + player.jumpApexRightSpeed) * InputManager.ins.L_Input.x * player.right + player.upVelocity;
                    }
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

                        case AirJumpType.DoubleJump:
                            player.rigidbody.rb.velocity += -player.up * 100f * tickDelta;
                            if(player.upSpeed < 12f)
                            {
                                player.legs.animator.animator.Play("PlayerLegs_JumpAscentSlowPose2");
                                player.torso.animator.animator.Play("PlayerTorso_Default_JumpAscentSlowPose2");
                            }
                            if (player.upSpeed < -4f)
                                player.animator.animator.Play("Player_Fall");
                            break;
                        case AirJumpType.FrontFlip:
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
                        case AirJumpType.BackFlip:
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
                        case AirJumpType.FrontSpin:
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
                        case AirJumpType.BackSpin:
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
                if(player.animator.CurrentState("Player_AirRollDescent"))
                {
                    player.rigidbody.rb.velocity += -Vector2.up * 90f * tickDelta;
                    //player.rigidbody.rb.velocity += Vector2.right * 50f * tickDelta * InputManager.ins.L_Input.x;
                    //Debug.LogError($"Player Up Velocity = {player.upSpeed} || Player True Up Velocity = {player.rigidbody.rb.velocity.y}");
                    //Since we are changing the up direction at every step the up velocity is always positive
                    if(player.rigidbody.rb.velocity.y < player.airRollDescentSpeedMax)
                    {
                        player.rigidbody.rb.velocity = player.rigidbody.rb.velocity.x * Vector2.right + player.airRollDescentSpeedMax * Vector2.up;
                    }
                    //Debug.LogError($"Player Y Velocity = {player.rigidbody.rb.velocity.y}");
                    if (player.rigidbody.rb.velocity.y > player.airRollBraceDescentSpeed)
                    {
                        player.legs.animator.animator.Play("PlayerLegs_RollPose2");
                        player.torso.animator.animator.Play("PlayerTorso_Default_RollPose2"); 
                    }
                    else
                    {
                        player.legs.animator.animator.Play("PlayerLegs_RollPose4");
                        player.torso.animator.animator.Play("PlayerTorso_Default_RollPose4");
                    }
                }
                if(player.animator.CurrentState("Player_Slide"))
                {
                    if(!player.grounded)
                    {
                        //Debug.LogError("Sliding and not grounded");
                        player.rigidbody.rb.velocity += -Vector2.up * 90f * tickDelta;
                    }
                    if(Mathf.Abs(player.rigidbody.rb.velocity.x) < player.slideMinSpeed)
                    {
                        player.animator.animator.Play("Player_Land");
                    }
                }
                //if(player.animator.CurrentState("Player_TwoHandedStabReady") || player.animator.CurrentState("Player_TwoHandedStab"))
                //{
                //    if(!player.grounded)
                //    {
                //        player.rigidbody.rb.velocity += -player.up * 110f * tickDelta;
                //        if (player.upSpeed < player.fallSpeedMax)
                //            player.rigidbody.rb.velocity = player.rightVelocity + player.fallSpeedMax * player.up;
                //    }
                //}
            }
        }
        public static void OnStateEnter(RuntimeObject obj, int frame, int stateHash, int previousStateHash)
        {
            RuntimeObjects.Player player = obj as RuntimeObjects.Player;
            if (player != null)
            {
                if(player.animator.CurrentState("Player_Initialized"))
                {
                    //Debug.LogError("Player Animator Initialized");
                    RuntimePlayerWeapon.GetWeapon(GameManager.ins.SpawnWeapon(WeaponHeadSpriteType.Spear, WeaponShaftSpriteType.Long, WeaponPommelSpriteType.Default));
                    RuntimePlayerWeapon.GetWeapon(GameManager.ins.SpawnWeapon(WeaponHeadSpriteType.Spear, WeaponShaftSpriteType.Long, WeaponPommelSpriteType.Default));
                }
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
                    if (player.upMagnitude <= 10.5f)
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
                    onJumpsLeftChanged?.Invoke(player.jumpsLeft);
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
                    player.obj.up = new Vector3(InputManager.ins.L_Input.x, InputManager.ins.L_Input.y, 0f);
                    player.jumpsLeft--;
                    onJumpsLeftChanged?.Invoke(player.jumpsLeft);
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
                        case AirJumpType.DoubleJump:
                            player.legs.animator.animator.Play("PlayerLegs_JumpAscentSlowPose1");
                            player.torso.animator.animator.Play("PlayerTorso_Default_JumpAscentSlowPose1");
                            break;
                        case AirJumpType.FrontFlip:
                            player.legs.animator.animator.Play("PlayerLegs_FrontflipPose1");
                            player.torso.animator.animator.Play("PlayerTorso_Default_FrontflipPose1");
                            break;
                        case AirJumpType.BackFlip:
                            player.legs.animator.animator.Play("PlayerLegs_BackflipPose1");
                            player.torso.animator.animator.Play("PlayerTorso_Default_BackflipPose1");
                            break;
                        case AirJumpType.FrontSpin:
                            player.legs.animator.animator.Play("PlayerLegs_FrontspinPose1");
                            player.torso.animator.animator.Play("PlayerTorso_Default_FrontspinPose1");
                            break;
                        case AirJumpType.BackSpin:
                            player.legs.animator.animator.Play("PlayerLegs_BackspinPose1");
                            player.torso.animator.animator.Play("PlayerTorso_Default_BackspinPose1");
                            break;
                    }
                }
                if(player.animator.CurrentState("Player_AirRollStart"))
                {
                    player.legs.animator.animator.Play("PlayerLegs_DoubleJumpStartPose");
                    player.torso.animator.animator.Play("PlayerTorso_Default_DoubleJumpStartPose");
                    player.obj.up = InputManager.ins.L_Input;
                }
                if(player.animator.CurrentState("Player_AirRollDescent"))
                {
                    if(Mathf.Approximately(InputManager.ins.L_Input.y, 0f))
                    {
                        player.rigidbody.rb.velocity = Vector2.Dot(player.rigidbody.rb.velocity, Vector2.right) * Vector2.right + InputManager.ins.L_Input.x * Vector2.right * 20f;
                        //Debug.LogError("Directly across");
                    }
                    else if(Mathf.Approximately(InputManager.ins.L_Input.x, 0f))
                    {
                        player.rigidbody.rb.velocity = Vector2.Dot(player.rigidbody.rb.velocity, Vector2.up) * Vector2.up + InputManager.ins.L_Input.y * Vector2.up * 15f;
                        //Debug.LogError("Directly down");
                    }
                    else
                    {
                        //Debug.LogError($"In direction {InputManager.ins.L_Input}");
                        player.rigidbody.rb.velocity = InputManager.ins.L_Input * 20f;
                    }
                        
                    player.legs.animator.animator.Play("PlayerLegs_DoubleJumpAscentPose");
                    player.torso.animator.animator.Play("PlayerTorso_Default_DoubleJumpAscentPose");
                    player.rigidbody.rbObj.position = player.RelativePos(player.doubleJumpShift);
                }
                if(player.animator.CurrentState("Player_AirRollLand"))
                {
                    player.legs.animator.animator.Play("PlayerLegs_RollPose4");
                    player.torso.animator.animator.Play("PlayerTorso_Default_RollPose4");
                    player.obj.up = Vector2.up;
                }
                if(player.animator.CurrentState("Player_Slide"))
                {
                    player.legs.animator.animator.Play("PlayerLegs_SlidePose1");
                    player.torso.animator.animator.Play("PlayerTorso_Default_SlidePose1");
                }
                if(player.animator.CurrentState("Player_Damaged"))
                {
                    player.legs.animator.animator.Play("PlayerLegs_Damaged_Pose1");
                    player.torso.animator.animator.Play("PlayerTorso_Damaged_Pose1");
                }
                if(player.animator.CurrentState("Player_CatchWeapon"))
                {
                    player.torso.animator.animator.Play("PlayerTorso_Throw_Pose3");
                    //Debug.LogError("Catch Weapon Playing");
                }
                if(player.animator.CurrentState("Player_ThrowWeapon"))
                {
                    player.torso.animator.animator.Play("PlayerTorso_Throw_Pose3");
                }
                if(player.animator.CurrentState("Player_TwoHandedStabReady"))
                {
                    player.obj.up = Vector2.up;
                    player.torso.animator.animator.Play("PlayerTorso_TwoHanded_Stab_Ready_Pose1");
                }
                if(player.animator.CurrentState("Player_TwoHandedStab"))
                {
                    player.torso.animator.animator.Play("PlayerTorso_TwoHanded_Stab_Pose1_Blur");
                    onAttackBlur?.Invoke();
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
                if(player.animator.CurrentState("Player_AirRollDescent"))
                {
                    if(player.rigidbody.rb.velocity.y <= player.airRollBraceDescentSpeed)
                    {
                        //Debug.LogError("Rotation should be happenin");
                        float dir = player.legs.animator.spriteRenderer.flipX.DefinedValue(-1, 1);
                        player.obj.up = new Vector2(Mathf.Cos(dir * 12f * player.animator.trueTimeSpentInState), Mathf.Sin(dir * 12f * player.animator.trueTimeSpentInState));
                    }
                    else
                    {
                        player.obj.up = player.rigidbody.rb.velocity;
                    }
                }
            }
        }
        public static void OnJumpPerformed(bool jumpInput)
        {
            RuntimeObjects.Player player = GameManager.ins.FindByID("Player") as RuntimeObjects.Player;
            if (player != null)
            {
                bool Jump = CanDoAction(player.animator, PlayerAction.Jump);
                bool JumpInAir = CanDoAction(player.animator, PlayerAction.JumpInAir);
                if (!jumpInput)
                    return;
                if (Jump)
                {
                    player.animator.animator.Play("Player_JumpAscent");
                }
                if(JumpInAir)
                {
                    AirJumpType jumpType = player.ComputeAirJumpType();
                    if (jumpType == AirJumpType.NoJumpsLeft)
                        return;
                    if (jumpType == AirJumpType.AirRoll)
                    {
                        player.animator.animator.Play("Player_AirRollStart");
                    }
                    else
                    {
                        player.animator.animator.Play("Player_DoubleJumpStart");
                    }
                }
            }
        }
        public static void OnDamageProcessed(List<PlayerDamageContainer> damageToProcess)
        {
            RuntimeObjects.Player player = GameManager.ins.FindByID("Player") as RuntimeObjects.Player;
            player.rigidbody.rb.velocity = Vector2.zero;
            player.obj.up = Vector2.up;
            if (!player.animator.CurrentState("Player_Damaged"))
                player.animator.animator.Play("Player_Damaged");
        }
        public static void OnGetWeapon(RuntimeObjects.Weapon weapon)
        {
            RuntimeObjects.Player player = GameManager.ins.FindByID("Player") as RuntimeObjects.Player;
            player.animator.animator.Play("Player_CatchWeapon");

            PlayerTorso torso = GameManager.ins.FindByID("PlayerTorso") as PlayerTorso;
            weapon.SetOwner(torso);
        }
        public static void OnWeaponThrow(RuntimeObjects.Weapon weapon)
        {
            RuntimeObjects.Player player = GameManager.ins.FindByID("Player") as RuntimeObjects.Player;
            bool WeaponThrow = CanDoAction(player.animator, PlayerAction.WeaponThrow);
            if (WeaponThrow)
            {
                player.animator.animator.Play("Player_ThrowWeapon");
                weapon.Throw(InputManager.ins.R_Input * 45f);
            }
        }
        public static void OnWeaponMelee(RuntimeObjects.Weapon weapon)
        {
            RuntimeObjects.Player player = GameManager.ins.FindByID("Player") as RuntimeObjects.Player;
            bool WeaponMelee = CanDoAction(player.animator, PlayerAction.WeaponMelee);
            if(WeaponMelee)
            {
                player.animator.animator.Play("Player_TwoHandedStabReady");
            }
        }
    }
}