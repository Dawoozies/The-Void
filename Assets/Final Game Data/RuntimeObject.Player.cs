using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateHandlers.Player;
using OverlapHandlers.Player;
namespace RuntimeObjects
{
    public class Player : RuntimeObject
    {
        public float fallSpeedMax = 20f;
        public Player(string id) : base(id)
        {
            managedStart += ManagedStart;
            managedUpdate += RuntimeAnimator.Update;
            managedUpdate += RuntimeRigidbody.Update;
            //managedFixedUpdate += RuntimeRigidbody.Update;
        }
        public void ManagedStart()
        {
            animator.onStateEnter += StateOnEnter.Handle;
            animator.onFrameUpdate += StateOnFrameUpdate.Handle;
            directedCircleOverlaps.onRuntimeObjectOverlap += OnRuntimeObjectOverlap.Handle;
            directedCircleOverlaps.onNonRuntimeObjectOverlap += OnNonRuntimeObjectOverlap.Handle;
        }
    }
}