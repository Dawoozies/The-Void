using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
}
//Hitting a DirectedCircleCollider / RuntimeObject
//Hitting a non RuntimeObject