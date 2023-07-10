using UnityEngine;
using RuntimeObjects;
using ExtensionMethods_Bool;
using BehaviourRecord.Player;

namespace StateHandlers.Mantis
{
    public static class Handler
    {
        //Boss memory here
        public static void Update(RuntimeObject obj, float tickDelta)
        {
            RuntimeObjects.Mantis mantis = obj as RuntimeObjects.Mantis;
            if(mantis != null)
            {
                if(mantis.animator.CurrentState("Mantis_Idle"))
                {
                    if(Record.groundedTime > 5f)
                    {
                        mantis.animator.animator.Play("Mantis_AimAtPlayer");
                    } 
                }
                if(mantis.animator.CurrentState("Mantis_AimAtPlayer"))
                {
                    int pose = 1;
                    if(mantis.animator.trueTimeSpentInState > 0.14f)
                        pose = 2;
                    if (mantis.animator.trueTimeSpentInState > 0.28f)
                        pose = 3;
                    mantis.legs.animator.animator.Play($"MantisLegs_{mantis.strikeDirection}Start_Pose{pose}");
                    mantis.torso.animator.animator.Play($"MantisTorso_{mantis.strikeDirection}Start_Pose{pose}");

                    mantis.leftArm.animator.animator.Play($"MantisLeftArm_Aim_Pose{pose}");
                    if (mantis.animator.trueTimeSpentInState > 0.28f + mantis.aimTime + mantis.linearStrikeWaitTime)
                        mantis.animator.animator.Play("Mantis_LinearStrike");
                }
                if(mantis.animator.CurrentState("Mantis_LinearStrike"))
                {
                    int pose = 1;
                    if (mantis.animator.trueTimeSpentInState > 0.07f)
                        pose = 2;
                    if (mantis.animator.trueTimeSpentInState > 0.14f)
                        pose = 3;
                    mantis.legs.animator.animator.Play($"MantisLegs_{mantis.strikeDirection}Strike_Pose{pose}");
                    mantis.torso.animator.animator.Play($"MantisTorso_{mantis.strikeDirection}Strike_Pose{pose}");

                    mantis.leftArm.animator.animator.Play($"MantisLeftArm_LinearStrike_Pose{pose}");
                    if (mantis.animator.trueTimeSpentInState > 0.14f + mantis.retractTime)
                    {
                        if (mantis.useLinearStrikeSpin)
                            mantis.animator.animator.Play("Mantis_LinearStrikeSpin");
                        else
                            mantis.animator.animator.Play("Mantis_LinearRetract");
                    }
                       
                }
                if(mantis.animator.CurrentState("Mantis_LinearRetract"))
                {
                    int pose = 1;
                    if(mantis.animator.trueTimeSpentInState > 0.07f)
                        pose = 2;
                    if (mantis.animator.trueTimeSpentInState > 0.14f)
                    {
                        pose = 3;
                        mantis.leftArm.obj.right = Vector3.right;
                        if (mantis.leftArm.animator.spriteRenderer.flipY && mantis.legs.animator.spriteRenderer.flipX)
                        {
                            mantis.leftArm.animator.spriteRenderer.flipY = false;
                            mantis.leftArm.animator.spriteRenderer.flipX = true;
                        }
                            
                    }
                    if(pose <= 2)
                    {
                        mantis.legs.animator.animator.Play($"MantisLegs_{mantis.strikeDirection}Retract_Pose{pose}");
                        mantis.torso.animator.animator.Play($"MantisTorso_{mantis.strikeDirection}Retract_Pose{pose}");

                    }
                    mantis.leftArm.animator.animator.Play($"MantisLeftArm_LinearRetract_Pose{pose}");
                    if (mantis.animator.trueTimeSpentInState > 0.28f)
                        mantis.animator.animator.Play($"Mantis_Idle");
                }
                if(mantis.animator.CurrentState("Mantis_LinearStrikeSpin"))
                {
                    int pose = 1;
                    if (mantis.animator.trueTimeSpentInState > 0.07f)
                        pose = 2;
                    if (mantis.animator.trueTimeSpentInState > 0.14f)
                        pose = 3;
                    if (mantis.animator.trueTimeSpentInState > 0.18f)
                        pose = 4;
                    if (mantis.animator.trueTimeSpentInState > 0.22f)
                        pose = 5;
                    if (mantis.animator.trueTimeSpentInState > 0.26f)
                        pose = 6;
                    if (mantis.animator.trueTimeSpentInState > 0.3f)
                        pose = 7;
                    mantis.leftArm.animator.animator.Play($"MantisLeftArm_LinearStrikeSpin_Pose{pose}");
                    if (mantis.animator.trueTimeSpentInState > 0.3f + mantis.retractTime)
                        mantis.animator.animator.Play("Mantis_LinearRetract");
                }
            }
        }
        public static void OnStateEnter(RuntimeObject obj, int frame, int stateHash, int previousStateHash)
        {
            RuntimeObjects.Mantis mantis = obj as RuntimeObjects.Mantis;
            MantisLegs legs = obj as MantisLegs;
            if(mantis != null)
            {
                if(mantis.animator.CurrentState("Mantis_Idle"))
                {
                    mantis.legs.animator.animator.Play($"MantisLegs_{mantis.strikeDirection}Start_Pose1");
                    mantis.torso.animator.animator.Play($"MantisTorso_{mantis.strikeDirection}Start_Pose1");
                    mantis.leftArm.animator.animator.Play("MantisLeftArm_Idle_Pose1");
                }
                if(mantis.animator.CurrentState("Mantis_AimAtPlayer"))
                {
                    mantis.legs.animator.animator.Play($"MantisLegs_{mantis.strikeDirection}Start_Pose1");
                    mantis.torso.animator.animator.Play($"MantisTorso_{mantis.strikeDirection}Start_Pose1");
                    mantis.leftArm.animator.animator.Play("MantisLeftArm_Aim_Pose1");
                }
                if(mantis.animator.CurrentState("Mantis_LinearStrikeSpin"))
                {
                    mantis.leftArm.animator.animator.Play("MantisLeftArm_LinearStrikeSpin_Pose1");
                }
            }
        }
        public static void OnFrameUpdate(RuntimeObject obj, int frame, int stateHash, int previousStateHash)
        {
            RuntimeObjects.Mantis mantis = obj as RuntimeObjects.Mantis;
            MantisLegs legs = obj as MantisLegs;
            MantisTorso torso = obj as MantisTorso;
            MantisLeftArm leftArm = obj as MantisLeftArm;
            if(mantis != null)
            {
                if(mantis.animator.CurrentState("Mantis_AimAtPlayer"))
                {
                    //Debug.LogError("FrameByFrame");
                    RuntimeObjects.Player player = GameManager.ins.allRuntimeObjects["Player"] as RuntimeObjects.Player;
                    if (mantis.animator.trueTimeSpentInState > 0.14f && mantis.animator.trueTimeSpentInState < 0.28f + mantis.aimTime)
                    {
                        if (mantis.leftArm.animator.spriteRenderer.flipX)
                            mantis.leftArm.animator.spriteRenderer.flipX = false;
                        mantis.leftArm.obj.right = -(player.obj.position - mantis.leftArm.obj.position).normalized;
                        mantis.leftArm.animator.spriteRenderer.flipY = (player.obj.position.x > mantis.leftArm.obj.position.x);
                        mantis.legs.animator.spriteRenderer.flipX = (player.obj.position.x > mantis.obj.position.x);
                        mantis.torso.animator.spriteRenderer.flipX = mantis.legs.animator.spriteRenderer.flipX;
                    }
                }
            }
            if(legs != null)
            {
                DirectedPoint torsoAnchor = legs.directedPoints.GetDirectedPoint("MantisTorso_Anchor");
                if(torsoAnchor != null)
                {
                    
                    (GameManager.ins.allRuntimeObjects["Mantis"] as RuntimeObjects.Mantis).torso.obj.position = legs.RelativePos(torsoAnchor.centers[0]);
                }
            }
            if(torso != null)
            {
                DirectedPoint leftArmAnchor = torso.directedPoints.GetDirectedPoint("MantisLeftArm_Anchor");
                if(leftArmAnchor != null)
                {
                    (GameManager.ins.allRuntimeObjects["Mantis"] as RuntimeObjects.Mantis).leftArm.obj.position = torso.RelativePos(leftArmAnchor.centers[0]);
                }
            }
        }
        public static void OnJumpPerformed(bool jumpInput)
        {
            RuntimeObjects.Player player = GameManager.ins.allRuntimeObjects["Player"] as RuntimeObjects.Player;
            RuntimeObjects.Mantis mantis = GameManager.ins.allRuntimeObjects["Mantis"] as RuntimeObjects.Mantis;
            if(mantis.animator.CurrentState("Mantis_Idle"))
            {
                if(jumpInput)
                    mantis.animator.animator.Play("Mantis_AimAtPlayer");
            }
        }
    }
}