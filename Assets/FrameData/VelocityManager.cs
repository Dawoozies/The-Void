using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityManager : MonoBehaviour, Listener_FrameVelocityData, Listener_FrameVelocityDataLStick, Listener_FrameVelocityDataRStick
{
    public float updateSpeed;
    float drag;
    Vector2 velocityBase;
    Vector2 velocityLStick;
    Vector2 velocityRStick;
    Rigidbody2D rb;
    Vector2 L_Input => InputManager.ins.L_Input;
    Vector2 R_Input => InputManager.ins.R_Input;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void Update_FrameVelocityData(Vector3 baseData, float drag, bool additive)
    {
        this.drag = drag;
        Vector2 v = new Vector2(baseData.x, baseData.y) * baseData.z;
        if(additive)
        {
            velocityBase += v * Time.fixedDeltaTime * updateSpeed;
            return;
        }
        velocityBase = v;
    }
    public void Update_FrameVelocityDataLStick(Vector3 stickData, bool additive)
    {
        Vector2 v = Vector2.Scale(new Vector2(stickData.x, stickData.y) * stickData.z, L_Input);
        if (additive)
        {
            velocityLStick += v * Time.fixedDeltaTime * updateSpeed;
            return;
        }
        velocityLStick = v;
    }
    public void Update_FrameVelocityDataRStick(Vector3 stickData, bool additive)
    {
        Vector2 v = Vector2.Scale(new Vector2(stickData.x, stickData.y) * stickData.z, R_Input);
        if(additive)
        {
            velocityRStick += v * Time.fixedDeltaTime * updateSpeed;
            return;
        }
        velocityRStick = v;
    }
    void FixedUpdate()
    {
        rb.drag = drag;
        Vector2 velocityFinal = velocityBase + velocityLStick + velocityRStick;
        rb.velocity = velocityFinal;
    }
}
