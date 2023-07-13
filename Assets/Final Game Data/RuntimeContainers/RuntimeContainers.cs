using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeObjects;
namespace RuntimeContainers
{
    public class DirectedCircleColliderContainer
    {
        public RuntimeObject obj;
        public string colliderNickname;
        public CircleCollider2D collider;
        public Vector2 up;
        public Vector2 right;
    }
    //Maybe could make player inventory a runtime container
    //Or maybe just make an actual inventory for the player lmao
    public class PlayerDamageContainer
    {
        public string stateName; //Animation state which dealt this damage
        public float percentage; //How much percentage damage to apply
        public Vector2 up; //"Up" direction for damage
        public Vector2 right; //"Right" direction for damage
        public bool killDamage;
        public PlayerDamageContainer(string stateName, float percentage, Vector2 up, Vector2 right, bool killDamage)
        {
            this.stateName = stateName;
            this.percentage = percentage;
            this.up = up;
            this.right = right;
            this.killDamage = killDamage;
        }
    }
}
//Hitting a DirectedCircleCollider / RuntimeObject
//Hitting a non RuntimeObject