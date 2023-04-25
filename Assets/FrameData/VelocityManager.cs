using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class VelocityManager : MonoBehaviour, Listener_FrameVelocityData, Listener_FrameVelocityDataLStick, Listener_FrameVelocityDataRStick
{
    public Vector2 velocityCap;
    public float updateSpeed;
    float drag;
    public Vector2 velocityBase;
    public Vector2 velocityLStick;
    public Vector2 velocityRStick;
    public Vector2 velocityBaseTarget;
    public Vector2 velocityLStickTarget;
    public Vector2 velocityRStickTarget;
    public Vector3 lastVelocityBaseData;
    Rigidbody2D rb;
    Vector2 L_Input => InputManager.ins.L_Input;
    Vector2 R_Input => InputManager.ins.R_Input;
    public bool disableManager;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void Update_FrameVelocityData(Vector3 baseData, float drag, bool additive)
    {
        this.drag = drag;
        if(lastVelocityBaseData != baseData)
        {
            lastVelocityBaseData = baseData;
            Vector2 v = new Vector2(baseData.x, baseData.y);
            rb.velocity += v;
        }
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
        if (disableManager)
            return;

        //rb.drag = drag;
        //Vector2 velocityFinal = velocityBase + velocityLStick + velocityRStick;
        //velocityFinal.x = Mathf.Clamp(velocityFinal.x, -velocityCap.x, velocityCap.x);
        //velocityFinal.y = Mathf.Clamp(velocityFinal.y, -velocityCap.y, velocityCap.y);
        //rb.velocity = velocityFinal;
    }
}
