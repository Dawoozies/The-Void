using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityComponentApply : MonoBehaviour, Listener_FrameVelocityDataNew
{
    Rigidbody2D rb;
    int lastFrame;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void Notify_AnimationClipChanged()
    {
        lastFrame = -1;
    }

    public void Update_FrameVelocityData(int currentFrame, List<VelocityComponent> velocityComponents)
    {
        for (int i = 0; i < velocityComponents.Count; i++)
        {
            VelocityComponent velocityComponent = velocityComponents[i];
            if(!velocityComponent.isImpulse && !velocityComponent.isGravitational && !velocityComponent.isConstant)
            {
                Debug.LogError("A velocity component has properties: isImpulse, isGravitational, isConstant all set to false. This velocity component will do nothing!");
                continue;
            }
            Vector3 velocityBase = velocityComponent.velocityBase;
            Vector3 threshold = velocityComponent.threshold;
            List<Vector3> finalVelocitySet = new List<Vector3>();
            if (velocityComponent.useLocalSpace)
                finalVelocitySet.Add(Vector3.Scale(velocityBase, rb.transform.localScale));
            if (velocityComponent.useLStick)
                finalVelocitySet.Add(Vector3.Scale(velocityBase, new Vector3(InputManager.ins.L_Input.x, InputManager.ins.L_Input.y, 0f)));
            if (velocityComponent.useRStick)
                finalVelocitySet.Add(Vector3.Scale(velocityBase, new Vector3(InputManager.ins.R_Input.x, InputManager.ins.R_Input.y, 0f)));
            if (velocityComponent.useGravity)
                finalVelocitySet.Add(Vector3.Scale(velocityBase, GameDataManager.ins.WorldGravityDirection));
            if (velocityComponent.useTransformUp)
                finalVelocitySet.Add(Vector3.Scale(velocityBase, rb.transform.up));
            if (velocityComponent.useTransformRight)
                finalVelocitySet.Add(Vector3.Scale(velocityBase, rb.transform.right));
            if (velocityComponent.useTransformForward)
                finalVelocitySet.Add(Vector3.Scale(velocityBase, rb.transform.forward));
            if (velocityComponent.useVelocityDirection)
                finalVelocitySet.Add(Vector3.Scale(velocityBase, rb.velocity.normalized));

            //Now we've added all the shit we might use
            //If none of this shit is selected, we just do things normally
        }
    }
}
