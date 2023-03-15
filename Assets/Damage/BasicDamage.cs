using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods_LayerMask;
public class BasicDamage : MonoBehaviour, Listener_ColliderOverlap
{
    public int damage;
    public LayerMask layerMask;
    public void Update_ColliderOverlap(List<HurtboxOverlap> overlaps)
    {

    }
}
