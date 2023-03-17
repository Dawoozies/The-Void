using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageSystem;
public class BasicEnemy : MonoBehaviour, Listener_DamageReceiver
{
    int damageCount;
    float damageRecoverTime;
    float timer;
    void Start()
    {
        //This should only ever be 1
        damageCount = 1;
    }
    public void Update_DamageReceiver(HurtboxOverlap overlap)
    {
        if (damageCount > 0)
            damageCount--;
        else
            return;

        timer = 0;

        damageRecoverTime = DamageUtilities.DamageRecoveryTime(overlap);
    }

    void Update()
    {
        if(damageCount < 1)
        {
            if (timer < damageRecoverTime)
                timer += Time.deltaTime;
            else
                damageCount++;
        }
    }
}
