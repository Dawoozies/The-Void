using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityComponentApply : MonoBehaviour, Listener_FrameVelocityData
{
    Rigidbody2D rb;
    int lastFrame;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void Notify_AnimationClipChanged()
    {
        Debug.Log("Notification went through");
        lastFrame = -1;
    }

    public void Update_FrameVelocityData(int currentFrame, List<VelocityComponent> velocityComponents)
    {
        Debug.Log("Trying to update");
        for (int i = 0; i < velocityComponents.Count; i++)
        {
            VelocityComponent velocityComponent = velocityComponents[i];
            Vector3 velocityBase = velocityComponent.velocityBase;
            float maxSpeed = velocityComponent.maxSpeed;
            float multiplier = velocityComponent.multiplier;
            Vector3 finalVelocityVector = velocityBase;
            //Basis transforms
            if (velocityComponent.useLocalSpace)
                finalVelocityVector = Vector3.Scale(finalVelocityVector, rb.transform.localScale);
            if (velocityComponent.useLStick)
                finalVelocityVector = Vector3.Scale(finalVelocityVector, new Vector3(InputManager.ins.L_Input.x, InputManager.ins.L_Input.y, 0f));
            if (velocityComponent.useRStick)
                finalVelocityVector = Vector3.Scale(finalVelocityVector, new Vector3(InputManager.ins.R_Input.x, InputManager.ins.R_Input.y, 0f));
            if (velocityComponent.useGravity)
                finalVelocityVector = Vector3.Scale(finalVelocityVector, GameDataManager.ins.WorldGravityDirection);
            if (velocityComponent.useTransformUp)
                finalVelocityVector = Vector3.Scale(finalVelocityVector, rb.transform.up);
            if (velocityComponent.useTransformRight)
                finalVelocityVector = Vector3.Scale(finalVelocityVector, rb.transform.right);
            if (velocityComponent.useTransformForward)
                finalVelocityVector = Vector3.Scale(finalVelocityVector, rb.transform.forward);
            if (velocityComponent.useVelocityDirection)
                finalVelocityVector = Vector3.Scale(finalVelocityVector, rb.velocity.normalized);
            if (velocityComponent.useVelocity)
                finalVelocityVector = Vector3.Scale(finalVelocityVector, rb.velocity);
            //Velocity Application
            if (velocityComponent.isImpulse)
                Impulse(currentFrame, finalVelocityVector);
            if (velocityComponent.isGravitational)
                Gravitational(multiplier, finalVelocityVector);
            if (velocityComponent.isConstant)
                Constant(maxSpeed, finalVelocityVector);
        }

        if(lastFrame != currentFrame)
        {
            lastFrame = currentFrame;
        }
    }
    void Impulse(int currentFrame, Vector3 finalVelocity)
    {
        if (currentFrame == lastFrame)
            return;
        rb.velocity += (Vector2)finalVelocity;
        Debug.Log("Adding impulse velocity");
    }
    //Called gravitational because these will "act like" gravity
    void Gravitational(float multiplier, Vector3 finalVelocity)
    {
        rb.velocity += (Vector2)finalVelocity * Time.fixedDeltaTime * multiplier;
        Debug.Log("Adding gravity like velocity");
    }
    //Called constant because these will be constantly applied. These will act like constant vectors to the end of a vector field
    void Constant(float maxSpeed, Vector3 finalVelocity)
    {
        Vector3 finalDirection = finalVelocity.normalized;
        float finalMagnitude = finalVelocity.magnitude;
        float projectedSpeed = Vector3.Dot(rb.velocity, finalDirection);
        if (projectedSpeed + finalMagnitude > maxSpeed)
            finalMagnitude = maxSpeed - projectedSpeed;
        rb.velocity += (Vector2)finalDirection * finalMagnitude;
        Debug.Log("Adding constant velocity");
    }
}
