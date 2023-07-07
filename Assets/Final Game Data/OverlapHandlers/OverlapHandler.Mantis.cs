using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeObjects;
namespace OverlapHandler.Mantis
{
    public static class OnRuntimeObjectOverlap
    {
        public static void Handle(string dataName, RuntimeObject obj, RuntimeObject hitObj)
        {
            if(dataName == "DamageZone")
            {
                PlayerLegs playerLegs = hitObj as PlayerLegs;
                PlayerTorso playerTorso = hitObj as PlayerTorso;
                if(playerLegs != null || playerTorso != null) 
                {
                    MantisLeftArm leftArm = obj as MantisLeftArm;

                    if (leftArm != null)
                        Handle_MantisLeftArm_Damage(leftArm);
                }
            }
        }
        static void Handle_MantisLeftArm_Damage(MantisLeftArm leftArm)
        {
            if (leftArm.animator.CurrentState("MantisLeftArm_LinearStrike_Pose1"))
                RuntimePlayerDamage.ApplyDamage("MantisLeftArm_LinearStrike_Pose1", 2f);
            if (leftArm.animator.CurrentState("MantisLeftArm_LinearStrike_Pose2"))
                RuntimePlayerDamage.ApplyDamage("MantisLeftArm_LinearStrike_Pose2", 3f);
        }
    }
}