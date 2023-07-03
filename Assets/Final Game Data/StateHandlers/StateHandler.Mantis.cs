using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeObjects;
using ExtensionMethods_Bool;
namespace StateHandlers.Mantis
{
    public static class Handler
    {
        public static void OnStateEnter(RuntimeObject obj, int frame, int stateHash, int previousStateHash)
        {
            RuntimeObjects.Mantis mantis = obj as RuntimeObjects.Mantis;
            if(mantis != null)
            {
                if(mantis.animator.CurrentState("Mantis_Idle"))
                {
                    mantis.legs.animator.animator.Play("MantisLegs_WindUp1_Pose1");
                    mantis.torso.animator.animator.Play("MantisTorso_WindUp1_Pose1");
                    mantis.leftArm.animator.animator.Play("MantisLeftArm_Idle_Pose1");
                }
            }
        }
        public static void OnFrameUpdate(RuntimeObject obj, int frame, int stateHash, int previousStateHash)
        {
            RuntimeObjects.Mantis mantis = GameManager.ins.allRuntimeObjects["Mantis"] as RuntimeObjects.Mantis;
            MantisLegs legs = obj as MantisLegs;
            MantisTorso torso = obj as MantisTorso;
            if(legs != null)
            {
                DirectedPoint torsoAnchor = legs.directedPoints.GetDirectedPoint("MantisTorso_Anchor");
                if(torsoAnchor != null)
                {
                    mantis.torso.obj.position = legs.RelativePos(torsoAnchor.centers[0]);
                }
            }
            if(torso != null)
            {
                DirectedPoint leftArmAnchor = torso.directedPoints.GetDirectedPoint("MantisLeftArm_Anchor");
                if(leftArmAnchor != null)
                {
                    mantis.leftArm.obj.position = torso.RelativePos(leftArmAnchor.centers[0]);
                }
            }
        }
    }
}