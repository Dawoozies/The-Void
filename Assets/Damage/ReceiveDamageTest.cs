using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageSystem;
public class ReceiveDamageTest : MonoBehaviour, Listener_Damage<FireDamage>
{
    public void Update_Damage(params FireDamage[] damageList)
    {
        for (int i = 0; i < damageList.Length; i++)
        {
            damageList[i].ApplyDamage();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
