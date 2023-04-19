using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Listener_FrameVelocityData
{
    public void Update_FrameVelocityData(Vector3 baseData, float drag, bool additive);
}
public interface Listener_FrameVelocityDataLStick
{
    public void Update_FrameVelocityDataLStick(Vector3 stickData, bool additive);
}
public interface Listener_FrameVelocityDataRStick
{
    public void Update_FrameVelocityDataRStick(Vector3 stickData, bool additive);
}