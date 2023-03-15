using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods_LayerMask;
using ExtensionMethods_List;
public class BasicDamage : MonoBehaviour, Listener_ColliderOverlap
{
    public int damage;
    public LayerMask layerMask;
    public List<HurtboxOverlap> queuedOverlaps = new List<HurtboxOverlap>();
    public void Update_ColliderOverlap(List<HurtboxOverlap> overlaps)
    {
        for (int i = 0; i < overlaps.Count; i++)
        {
            HurtboxOverlap overlap = overlaps[i];
            if (layerMask.Contains(overlap.collider.gameObject.layer))
                queuedOverlaps.AddFirstInstance(overlap);
        }
    }

    void Update()
    {
        if (queuedOverlaps == null)
            return;
        //We need to handle all the queuedOverlaps
        if (queuedOverlaps.Count > 0)
        { Handle_DamageReceivers(queuedOverlaps[0]); queuedOverlaps.RemoveAt(0); }
    }

    void Handle_DamageReceivers(HurtboxOverlap overlap)
    {
        //We might want to change this to getting components in parent as we might want logic to exist on the parent entity to the hitboxes
        //But for now we'll just use it like this
        Listener_DamageReceiver[] listener_DamageReceivers = overlap.collider.GetComponents<Listener_DamageReceiver>();
        if (listener_DamageReceivers == null)
            return;

        for (int i = 0; i < listener_DamageReceivers.Length; i++)
        {
            listener_DamageReceivers[i].Update_DamageReceiver(overlap);
        }
    }
}
