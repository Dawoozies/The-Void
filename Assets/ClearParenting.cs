using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearParenting : MonoBehaviour
{
    public void Clear()
    {
        transform.parent = null;
    }
}
