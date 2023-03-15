using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour, Listener_DamageReceiver
{
    public int damageCount;
    public float damageRecoverTime;
    float timer;
    public HurtboxOverlap lastOverlap;
    public void Update_DamageReceiver(HurtboxOverlap overlap)
    {
        if (damageCount > 0)
            damageCount--;
        else
            return;

        timer = 0;
        lastOverlap = overlap;
        Debug.Log($"{gameObject.name} taking damage");

        //Compute how many seconds left until the current animation state has finished
        //This will result in the first overlap instance in this animation being able to deal damage but no others
        //Hopefully this will stop damage from going through multiple times during one damage window
        damageRecoverTime = overlap.stateLength - (overlap.stateNormalizedTime * overlap.clip.length + 1) / overlap.stateSpeed;
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
