using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeObjects;
using RuntimeContainers;

namespace OverlapHandlers.HangedFrame
{
    public static class OnRuntimeObjectOverlap
    {
        public static void Handle(string dataName, RuntimeObject obj, DirectedCircleColliderContainer hitContainer, Vector2 overlapUp, Vector2 overlapRight)
        {

        }
    }
    public static class OnNonRuntimeObjectOverlap
    {
        public static void Handle(string dataName, RuntimeObject obj, Collider2D hitCollider)
        {

        }
    }
    public static class OnNullResult
    {
        public static void Handle(string dataName, RuntimeObject obj)
        {

        }
    }
}