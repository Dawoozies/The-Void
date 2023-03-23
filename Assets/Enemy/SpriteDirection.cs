using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDirection : MonoBehaviour
{
    public void SetDirection(int direction)
    {
        transform.localScale = new Vector3(direction,1,1);
    }
}
