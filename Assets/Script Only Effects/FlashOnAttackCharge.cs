using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashOnAttackCharge : MonoBehaviour
{
    Animator animator;
    SpriteRenderer spriteRenderer;
    public Color onColor;
    public Color offColor;
    public float onTime;
    public float timer;
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if(animator.GetBool("STATETYPE:AttackCharge"))
        {
            if (timer < onTime)
                timer += Time.deltaTime;

            if(timer >= onTime)
            {
                if (spriteRenderer.color != onColor)
                {
                    spriteRenderer.color = onColor;
                }
                else
                {
                    spriteRenderer.color = offColor;
                }

                timer = 0f;
            }
        }
        else
        {
            if (spriteRenderer.color == onColor)
                spriteRenderer.color = offColor;
        }
    }
}
