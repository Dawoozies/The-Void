using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager ins;
    private void Awake()
    {
        ins = this;
    }
    public int AirDodgesMax;
    public int JumpsMax;
    public bool ThrownHalberd;
}
