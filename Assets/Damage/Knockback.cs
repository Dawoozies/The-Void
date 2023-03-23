using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods_LayerMask;
using LinearAlgebra;
public class Knockback : MonoBehaviour, Listener_ColliderOverlap
{
    Rigidbody2D rb;
    public LayerMask layerMask;
    Queue<HurtboxOverlap> queuedOverlaps;
    public void Update_ColliderOverlap(List<HurtboxOverlap> overlaps)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

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
            newVelocity.pathStart = currentOverlap.hurtboxWorldCenter;
            newVelocity.pathEnd = currentOverlap.collider.transform.position;

            //Then get the direction and negate it
            Vector3 velocityDirection = -newVelocity.direction;
            rb.AddForce(velocityDirection);
        }
    }
}
