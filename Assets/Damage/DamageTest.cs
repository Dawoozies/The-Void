using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageSystem;
public class DamageTest : MonoBehaviour, Listener_ColliderOverlap
{
    Damage<HurtboxOverlap> damage;
    public void Update_ColliderOverlap(List<HurtboxOverlap> overlaps)
    {
        damage = new FrostDamage();
        damage.ApplyDamage(overlaps.ToArray());
        //Right now we might as well not even have the params thing going on
        //But I want to extend eventually to having this ApplyDamage method basically collecting all damage
        //Data and then initiating the response all at once in the same place
    }
}