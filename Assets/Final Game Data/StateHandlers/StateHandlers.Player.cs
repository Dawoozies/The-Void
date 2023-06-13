using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeObjects;
namespace StateHandlers.Player
{
    public static class StateOnEnter
    {
        public static void Handle(RuntimeObject obj, int frame, int stateHash, int previousStateHash)
        {

        }
    }
    public static class StateOnFrameUpdate
    {
        public static void Handle(RuntimeObject obj, int frame, int stateHash, int previousStateHash)
        {
            RuntimeObjects.Player player = obj as RuntimeObjects.Player;
            //Kept this here as a warning for putting physics / input related things in OnFrameUpdate
            player.rigidbody.rb.velocity = 15f * InputManager.ins.L_Input.x * player.right + player.rigidbody.upVelocity;
            if (Animator.StringToHash("FALL") == stateHash)
            {
                player.rigidbody.rb.AddForce(-player.up*20f);
                if(player.rigidbody.upMagnitude < -player.fallSpeedMax)
                    player.rigidbody.rb.velocity = player.rigidbody.rightVelocity - player.fallSpeedMax * player.up;
            }
        }
    }
}