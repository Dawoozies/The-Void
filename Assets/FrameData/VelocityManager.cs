using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityManager : MonoBehaviour, Listener_FrameVelocityData, Listener_FrameVelocityDataLStick, Listener_FrameVelocityDataRStick
{
    float drag;
    Vector2 velocityBase;
    Vector2 velocityLStick;
    Vector2 velocityRStick;
    Rigidbody2D rb;
    Vector2 L_Input => InputManager.ins.L_Input;
    Vector2 R_Input => InputManager.ins.R_Input;
    public void Update_FrameVelocityData(Vector3 direction, float magnitude, float drag)
    {
        this.drag = drag;
        velocityBase = direction * magnitude;
    }
    public void Update_FrameVelocityDataLStick(Vector3 stickData)
    {
        velocityLStick = new Vector2(stickData.x,stickData.y)*stickData.z;
    }
    public void Update_FrameVelocityDataRStick(Vector3 stickData)
    {
        velocityRStick = new Vector2(stickData.x, stickData.y) * stickData.z;
    }
    void FixedUpdate()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        rb.drag = drag;
        Vector2 velocityFinal = velocityBase + Vector2.Scale(L_Input, velocityLStick) + Vector2.Scale(R_Input, velocityRStick);

        if(velocityFinal.magnitude > 0)
            rb.velocity = velocityFinal;
    }
}
