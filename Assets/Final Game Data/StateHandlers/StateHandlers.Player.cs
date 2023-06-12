using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeObjects;
namespace StateHandlers.Player
{
    public static class StateOnEnter
    {

    }
    public static class StateOnFrameUpdate
    {
        public static void Handle(RuntimeObject obj, int frame, int stateHash, int previousStateHash)
        {
            if(Animator.StringToHash("FALL") == stateHash)
            {
                Debug.Log("Player FALL state");
            }
        }
    }
}