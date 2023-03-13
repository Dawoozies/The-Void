using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods_LayerMask;
public class BasicDamage : MonoBehaviour, Listener_ColliderOverlap
{
    public int damage;
    public LayerMask layerMask;
    public void Update_ColliderOverlap(List<Collider2D> colliders)
    {
        foreach (Collider2D collider in colliders)
        {
            if (layerMask.Contains(collider.gameObject.layer))
            {
                Debug.Log($"{collider.name} is in layer mask");
            }
            else
            {
                Debug.Log($"{collider.name} is not in layer mask");
            }
        }
    }
}
