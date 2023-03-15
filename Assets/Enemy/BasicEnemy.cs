using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour, Listener_DamageReceiver
{
    public int damageCount;
    public float damageRecoverTime;
    float timer;
    public void Update_DamageReceiver()
    {
        if (damageCount > 0)
            damageCount--;
        else
            return;

        timer = 0;
        Debug.Log($"{gameObject.name} taking damage");
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
