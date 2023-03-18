using System.Collections.Generic;
using UnityEngine;
namespace DamageSystem
{
    public static class DamageUtilities
    {
        //Computes how much time in seconds is left in the animation state
        //We only want another instance of damage being allowed if another animation state is causing damage
        public static float DamageRecoveryTime(HurtboxOverlap overlap)
        {
            return overlap.stateLength - (overlap.stateNormalizedTime * overlap.clip.length) / overlap.stateSpeed;
        }
    }
}