using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface Listener_FrameVelocityData
{
    public void Notify_AnimationClipChanged();
    public void Update_FrameVelocityData(int currentFrame, List<VelocityComponent> velocityComponents);
}