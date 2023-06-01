using GameManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Interactions
{
    public static class StaticInteract
    {
        public static bool StaticCheck(Collider2D colliderToCheck)
        {
            if (colliderToCheck.gameObject.layer == LayerMask.NameToLayer("Ground"))
                return true;
            return false;
        }
        public static void Interactions(RuntimeSceneObject obj, Component_Overlap_Data componentData, GameObject hitObj, ref List<(RuntimeSceneObject, Component_Overlap_Data, GameObject)> staticInteractionBuffer)
        {
            
            if (obj.ID == RuntimeIdentifier.Player && componentData.nickname == "Groundbox" && hitObj.layer == LayerMask.NameToLayer("Ground"))
            {
                Player player = obj as Player;
                if (player != null)
                {
                    player.grounded = true;
                    //Debug.Log("grounded = true");
                }
            }
            if(obj.ID == RuntimeIdentifier.Weapon && componentData.nickname == "EmbeddingArea" && hitObj.layer == LayerMask.NameToLayer("Ground"))
            {
                Weapon weapon = obj as Weapon;
                if(weapon != null)
                    weapon.animator.SetBool("Embedded", true);
            }
            staticInteractionBuffer.Remove((obj, componentData, hitObj));
        }
    }
}