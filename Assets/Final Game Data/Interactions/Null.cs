using GameManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Interactions
{
    public static class NullInteract
    {
        public static void Interactions(RuntimeSceneObject obj, Component_Overlap_Data componentData, ref List<(RuntimeSceneObject, Component_Overlap_Data)> nullInteractionBuffer)
        {
            if (obj.ID == RuntimeIdentifier.Player && componentData.nickname == "Groundbox")
            {
                Player player = obj as Player;
                if (player != null)
                {
                    player.grounded = false;
                    //Debug.Log("grounded = false");
                }
            }
            nullInteractionBuffer.Remove((obj, componentData));
        }
    }
}