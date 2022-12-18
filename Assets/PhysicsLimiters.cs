using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsLimiters : MonoBehaviour
{
    //Makes final adjustments to the rigidbody properties e.g. clamping velocity
    public float MaxHorizontalVelocity;
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void LateUpdate()
    {
        rb.velocity = new Vector2(
            Mathf.Clamp(rb.velocity.x, -MaxHorizontalVelocity, MaxHorizontalVelocity), 
            rb.velocity.y
            );
    }
}
