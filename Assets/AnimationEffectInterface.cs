using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AnimationEffectInterface
{
    public bool TryPlayEffect(EffectData effectData, Vector3 effectOrigin);
}