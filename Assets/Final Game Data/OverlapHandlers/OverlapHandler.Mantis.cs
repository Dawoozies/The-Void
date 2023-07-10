using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeObjects;
namespace OverlapHandler.Mantis
{
    public static class OnRuntimeObjectOverlap
    {
        public static void Handle(string dataName, RuntimeObject obj, RuntimeObject hitObj, Vector2 overlapUp, Vector2 overlapRight)
        {
            if(dataName == "DamageZone")
            {
                PlayerLegs playerLegs = hitObj as PlayerLegs;
                PlayerTorso playerTorso = hitObj as PlayerTorso;
                if(playerLegs != null || playerTorso != null) 
                {
                    MantisLeftArm leftArm = obj as MantisLeftArm;

                    if (leftArm != null)
                        Handle_MantisLeftArm_Damage(leftArm, overlapUp, overlapRight);
                }
            }
        }
        static void Handle_MantisLeftArm_Damage(MantisLeftArm leftArm, Vector2 overlapUp, Vector2 overlapRight)
        {
            if (leftArm.animator.CurrentState("MantisLeftArm_LinearStrike_Pose1"))
                RuntimePlayerDamage.ApplyDamage("MantisLeftArm_LinearStrike_Pose1", 2f, overlapUp, overlapRight, false);
            if (leftArm.animator.CurrentState("MantisLeftArm_LinearStrike_Pose2"))
                RuntimePlayerDamage.ApplyDamage("MantisLeftArm_LinearStrike_Pose2", 3f, overlapUp, overlapRight, false);
            if (leftArm.animator.CurrentState("MantisLeftArm_LinearStrikeSpin_Pose3"))
                RuntimePlayerDamage.ApplyDamage("MantisLeftArm_LinearStrikeSpin_Pose3", 2f, overlapUp, overlapRight, false);
            if (leftArm.animator.CurrentState("MantisLeftArm_LinearStrikeSpin_Pose4"))
                RuntimePlayerDamage.ApplyDamage("MantisLeftArm_LinearStrikeSpin_Pose4", 3f, overlapUp, overlapRight, false);
            if (leftArm.animator.CurrentState("MantisLeftArm_LinearStrikeSpin_Pose5"))
                RuntimePlayerDamage.ApplyDamage("MantisLeftArm_LinearStrikeSpin_Pose5", 4f, overlapUp, overlapRight, false);
            if (leftArm.animator.CurrentState("MantisLeftArm_LinearStrikeSpin_Pose6"))
                RuntimePlayerDamage.ApplyDamage("MantisLeftArm_LinearStrikeSpin_Pose6", 3f, overlapUp, overlapRight, false);
            if (leftArm.animator.CurrentState("MantisLeftArm_LinearStrikeSpin_Pose7"))
                RuntimePlayerDamage.ApplyDamage("MantisLeftArm_LinearStrikeSpin_Pose7", 2f, overlapUp, overlapRight, false);
        }
    }
}