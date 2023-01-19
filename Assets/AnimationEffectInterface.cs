using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AnimationEffectInterface
{
    public bool TryPlayEffect(EffectData effectData, Vector3 effectOrigin, Vector3 entityLocalScale, bool copyEntityDirection);
    public bool TryPlayEffectAtPosition(EffectData effectData, Vector3 worldPositionToPlayAt, Vector3 entityLocalScale, bool copyEntityDirection);
}