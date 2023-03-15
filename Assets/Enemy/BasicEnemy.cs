using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour, Listener_DamageReceiver
{
    int damageCount;
    float damageRecoverTime;
    float timer;
    HurtboxOverlap lastOverlap;
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
        lastOverlap = overlap;
        Debug.Log($"{gameObject.name} taking damage");

        //Compute how many seconds left until the current animation state has finished
        //This will result in the first overlap instance in this animation being able to deal damage but no others
        //Hopefully this will stop damage from going through multiple times during one damage window
        damageRecoverTime = overlap.stateLength - (overlap.stateNormalizedTime * overlap.clip.length + 1) / overlap.stateSpeed;

        if (damageRecoverTime < 0)
            Debug.LogError($"Damage Recover Time is negative: This is not fucking good, please increase frames of animation that is causing this. Anim Clip Name = {overlap.clip.name}");
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
