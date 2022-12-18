using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteDirection : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(spriteRenderer != null)
        {
            if(BasicInput.ins.InputLDirection > 0)
            {
                spriteRenderer.flipX = true;
            }
            if(BasicInput.ins.InputLDirection < 0)
            {
                spriteRenderer.flipX = false;
            }
        }
    }
}
