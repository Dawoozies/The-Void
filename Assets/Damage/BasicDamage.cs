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
                //Debug.Log($"{collider.name} is in layer mask");
                Listener_DamageReceiver[] listener_DamageReceivers = collider.GetComponents<Listener_DamageReceiver>();
                if(listener_DamageReceivers != null && listener_DamageReceivers.Length > 0)
                {
                    for (int i = 0; i < listener_DamageReceivers.Length; i++)
                    {
                        listener_DamageReceivers[i].Update_DamageReceiver();
                    }
                }
            }
        }
    }
}
