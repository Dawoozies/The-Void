using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeObjects;
namespace BehaviourRecord.Player
{
    public static class Record
    {
        public static bool jumpInput = false;
        static float jumpInputBufferTime = 0;
        const float JUMP_INPUT_BUFFER_TIME = 0.2f;
        public static float groundedTime = 0f;
        public static void Update(RuntimeObject obj, float tickDelta)
        {
            //Debug.LogError($"{obj.id} Record Update");
            RuntimeObjects.Player player = obj as RuntimeObjects.Player;
            if (player == null)
                Debug.LogError("Player null tf");
            if (player.grounded)
                groundedTime += tickDelta;
            else
                groundedTime = 0f;
            Debug.Log($"Grounded Time = {groundedTime}");
            if(jumpInput)
                jumpInputBufferTime -= tickDelta;
            if (jumpInputBufferTime < 0f)
            {
                jumpInput = false;
                jumpInputBufferTime = 0f;
            }
        }
        public static void OnJumpPerformed(bool inputValue)
        {
            if(inputValue)
            {
                jumpInput = true;
                jumpInputBufferTime = JUMP_INPUT_BUFFER_TIME;
            }
        }
    }
}