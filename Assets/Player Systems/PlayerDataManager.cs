using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager ins;
    private void Awake()
    {
        ins = this;
        //Just for now
        EquippedHalberd = true; //make this false when we start doing weapon swapping system
    }
    public int AirDodgesMax;
    public int JumpsMax;
    public bool ThrownHalberd;
    public bool EquippedHalberd;
}
