using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityApply : MonoBehaviour, Listener_FrameVelocityData
{
    Rigidbody2D rb;
    public Vector2 velocityLimit;
    public void Update_FrameVelocityData(Vector3 direction, float magnitude, float drag)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        float xDirFactor = 1f;
        float yDirFactor = 1f;
        //If velocityLimit component is left 0 then there is no limit
        if (Mathf.Abs(rb.velocity.x) >= velocityLimit.x && velocityLimit.x > 0 && Mathf.Sign(rb.velocity.x) == Mathf.Sign(direction.x))
            xDirFactor = 0f;

        if (Mathf.Abs(rb.velocity.y) >= velocityLimit.y && velocityLimit.y > 0 && Mathf.Sign(rb.velocity.y) == Mathf.Sign(direction.y))
            yDirFactor = 0f;
        //We should check if direction is different to current velocity as adding wont go over the limit
        rb.drag = drag;
        rb.velocity += new Vector2(direction.x*xDirFactor, direction.y*yDirFactor)* magnitude;
    }
}
