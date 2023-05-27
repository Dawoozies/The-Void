using GameManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Interactions
{
    public static class Interact
    {
        public static void Interactions(RuntimeSceneObject obj, Component_Overlap_Data componentData, RuntimeSceneObject hitObj, ref List<(RuntimeSceneObject, Component_Overlap_Data, RuntimeSceneObject)> interactionBuffer)
        {
            if(obj.ID == RuntimeIdentifier.Player && componentData.nickname == "RecallArea" && hitObj.ID == RuntimeIdentifier.Halberd)
            {
                Player player = obj as Player;
                Halberd halberd = hitObj as Halberd;
                if(player != null && halberd != null)
                {
                    player.mainWeaponID = MainWeaponID.Halberd;
                    halberd.inPlayerInventory = true;
                }
            }
            interactionBuffer.Remove((obj, componentData, hitObj));
        }
    }
}