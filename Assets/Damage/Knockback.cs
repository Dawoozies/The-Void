using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ExtensionMethods_LayerMask;
using LinearAlgebra;
using ExtensionMethods_Animator;
public class Knockback : MonoBehaviour, Listener_ColliderOverlap
{
    Rigidbody2D rb;
    public LayerMask layerMask;
    public float knockbackIntensity;
    public Vector2 shift;

    Animator animator;
    Queue<HurtboxOverlap> queuedOverlaps;
    int currentFrame;
    int lastFrame;
    public bool showFrameData;
    public void Update_ColliderOverlap(List<HurtboxOverlap> overlaps)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        //This will check if we are trying to process more than one set of collisions on the same frame
        currentFrame = animator.CurrentFrame();
        bool stopProcessingCollisions = (currentFrame == lastFrame);

        lastFrame = animator.CurrentFrame();
        if (stopProcessingCollisions)
            return;

        if (showFrameData)
        {
            Debug.Log("Damage being dealt on frame: " + animator.CurrentFrame());
            Debug.Log("Knockback being applied");
        }

        queuedOverlaps = new Queue<HurtboxOverlap>();

        foreach (HurtboxOverlap overlap in overlaps)
        {
            if(layerMask.Contains(overlap.collider.gameObject.layer))
            {
                queuedOverlaps.Enqueue(overlap);
            }
        }

        while(queuedOverlaps.Count > 0)
        {
            ParametrisedLine newVelocity = new ParametrisedLine();
            HurtboxOverlap currentOverlap = queuedOverlaps.Dequeue();
            newVelocity.pathStart = currentOverlap.hurtboxWorldCenter - new Vector3(animator.transform.localScale.x * shift.x, animator.transform.localScale.y * shift.y, 0);
            newVelocity.pathEnd = currentOverlap.collider.transform.position;

            //Then get the direction and negate it
            Vector3 velocityDirection = -newVelocity.direction;
            rb.AddForce(velocityDirection*knockbackIntensity);
        }
    }
}
