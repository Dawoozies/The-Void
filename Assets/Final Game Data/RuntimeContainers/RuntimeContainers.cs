using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeObjects;
namespace RuntimeContainers
{
    public class DirectedCircleColliderContainer
    {
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
        public PlayerDamageContainer(string stateName, float percentage)
        {
            this.stateName = stateName;
            this.percentage = percentage;
        }
    }
}
//Hitting a DirectedCircleCollider / RuntimeObject
//Hitting a non RuntimeObject