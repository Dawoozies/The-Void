using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityApplyLStick : MonoBehaviour, Listener_FrameVelocityDataLStick
{
    Rigidbody2D rb;
    Vector2 L_Input => InputManager.ins.L_Input;
    public Vector2 velocityLimit;
    public void Update_FrameVelocityDataLStick(Vector3 stickData)
    {
        //X component = X axis influence
        //Y component = Y axis influence
        //Z component = Magnitude of influence
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        float xDirFactor = 1f;
        float yDirFactor = 1f;
        if (Mathf.Abs(rb.velocity.x) >= velocityLimit.x && velocityLimit.x > 0 && Mathf.Sign(rb.velocity.x) == Mathf.Sign(stickData.x*L_Input.x))
            xDirFactor = 0f;
        if (Mathf.Abs(rb.velocity.y) >= velocityLimit.y && velocityLimit.y > 0 && Mathf.Sign(rb.velocity.y) == Mathf.Sign(stickData.y*L_Input.y))
            yDirFactor = 0f;
        Vector2 velocity = new Vector2(stickData.x * L_Input.x * xDirFactor, stickData.y * L_Input.y * yDirFactor) * stickData.z;
        rb.velocity += velocity;
    }
}
