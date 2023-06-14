using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeObjects;
namespace StateHandlers.Player
{
    public static class Handler
    {
        public static void Update(RuntimeObject obj, float tickDelta)
        {
            RuntimeObjects.Player player = obj as RuntimeObjects.Player;
            player.rigidbody.rb.velocity = 15f * InputManager.ins.L_Input.x * player.right + player.rigidbody.upVelocity;
            if (Animator.StringToHash("FALL") == player.animator.stateHash)
            {
                player.rigidbody.rb.AddForce(-player.up*20f);
                if(player.rigidbody.upSpeed < player.fallSpeedMax)
                    player.rigidbody.rb.velocity = player.rigidbody.rightVelocity + player.fallSpeedMax * player.up;
            }
            if(Animator.StringToHash("JUMP") == player.animator.stateHash)
            {
                player.rigidbody.rb.velocity += player.up * 5f;
                if(player.rigidbody.upSpeed > player.ascentSpeedMax)
                    player.rigidbody.rb.velocity = player.rigidbody.rightVelocity + player.ascentSpeedMax * player.up;
            }
            if(Animator.StringToHash("ASCENTSLOW") == player.animator.stateHash)
            {
                player.rigidbody.rb.AddForce(-player.up*20f);
                if(player.rigidbody.upSpeed < player.fallSpeedMax)
                    player.rigidbody.rb.velocity = player.rigidbody.rightVelocity + player.fallSpeedMax * player.up;
            }
        }
    }
}