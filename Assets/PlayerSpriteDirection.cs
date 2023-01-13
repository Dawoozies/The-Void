using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteDirection : MonoBehaviour
{
    void Update()
    {
        //This only works with sprite sheets that have default left facing characters
        if (BasicInput.ins.InputLDirection > 0)
        {
            transform.localScale = new Vector3(-1,1,1);
        }
        if (BasicInput.ins.InputLDirection < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
