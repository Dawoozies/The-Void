using RuntimeObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StateHandlers.Weapon
{
    public static class Handler
    {
        const float BLUR_SPEED = 30f;
        public static void Update(RuntimeObject obj, float tickDelta)
        {
            RuntimeObjects.Weapon weapon = obj as RuntimeObjects.Weapon;
            if(weapon != null) 
            {
                if(weapon.upMagnitude >= BLUR_SPEED)
                {
                    weapon.head.animator.animator.Play($"Weapon_Head_{weapon.head.spriteType}_Blur");
                    weapon.shaft.animator.animator.Play($"Weapon_Shaft_{weapon.shaft.spriteType}_Blur");
                    weapon.pommel.animator.animator.Play($"Weapon_Pommel_{weapon.pommel.spriteType}_Blur");
                }
                else
                {
                    weapon.head.animator.animator.Play($"Weapon_Head_{weapon.head.spriteType}");
                    weapon.shaft.animator.animator.Play($"Weapon_Shaft_{weapon.shaft.spriteType}");
                    weapon.pommel.animator.animator.Play($"Weapon_Pommel_{weapon.pommel.spriteType}");
                }
            }
        }
        public static void OnStateEnter(RuntimeObject obj, int frame, int stateHash, int previousStateHash)
        {
            RuntimeObjects.Weapon weapon = obj as RuntimeObjects.Weapon;
            if (weapon != null)
            {
                weapon.head.animator.animator.Play($"Weapon_Head_{weapon.head.spriteType}");
                weapon.shaft.animator.animator.Play($"Weapon_Shaft_{weapon.shaft.spriteType}");
                weapon.pommel.animator.animator.Play($"Weapon_Pommel_{weapon.pommel.spriteType}");
            }
        }
        public static void OnFrameUpdate(RuntimeObject obj, int frame, int stateHash, int previousStateHash)
        {

        }
    }
}