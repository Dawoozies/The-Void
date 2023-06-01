using GameManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Interactions
{
    public static class Interact
    {
        public static void Interactions(RuntimeSceneObject obj, Component_Overlap_Data componentData, RuntimeSceneObject hitObj, Collider2D hitCollider, ref List<(RuntimeSceneObject, Component_Overlap_Data, RuntimeSceneObject, Collider2D)> interactionBuffer)
        {
            if(obj.ID == RuntimeIdentifier.Player && componentData.nickname == "RecallArea" && hitObj.ID == RuntimeIdentifier.Weapon)
            {
                Player player = obj as Player;
                Weapon weapon = hitObj as Weapon;
                if (player != null && weapon != null)
                {
                    //if(!player.equipped.Contains(halberd))
                    //   player.equipped.Add(halberd);
                    if (!player.orbitingWeapons.Contains(weapon))
                    {
                        player.orbitingWeapons.Add(weapon);
                        weapon.animator.Play("ORBIT");
                        player.animator.Play("CATCH_HALBERD");
                    }
                }
            }
            bool hitHangedFrame =
                hitObj.ID == RuntimeIdentifier.HangedFrame
                || hitObj.ID == RuntimeIdentifier.HangedFrameRightHand
                || hitObj.ID == RuntimeIdentifier.HangedFrameLeftHand;
            if(obj.ID == RuntimeIdentifier.Weapon && componentData.nickname == "EmbeddingArea" && hitHangedFrame)
            {
                Weapon weapon = obj as Weapon;
                if (weapon != null)
                {
                    weapon.animator.SetBool("Embedded", true);
                    weapon.rb.transform.parent = hitCollider.transform;
                    weapon.spriteRenderer.sortingOrder = hitObj.spriteRenderer.sortingOrder + 1;
                }
            }
            if(obj.ID == RuntimeIdentifier.HangedFrameRightHand && componentData.nickname == "RopePoint" && hitObj.ID == RuntimeIdentifier.HangedFrameRightHand)
            {
                HangedFrame hangedFrame = GameManagement.GameManagement.ins.hangedFrame;
                hangedFrame.lineRenderer.SetPosition(0, obj.LocalPosFromTransform(componentData.circles[0].center));
            }
            if (obj.ID == RuntimeIdentifier.HangedFrameLeftHand && componentData.nickname == "RopePoint" && hitObj.ID == RuntimeIdentifier.HangedFrameLeftHand)
            {
                HangedFrame hangedFrame = GameManagement.GameManagement.ins.hangedFrame;
                hangedFrame.lineRenderer.SetPosition(1, obj.LocalPosFromTransform(componentData.circles[0].center));
            }
            interactionBuffer.Remove((obj, componentData, hitObj, hitCollider));
        }
    }
}