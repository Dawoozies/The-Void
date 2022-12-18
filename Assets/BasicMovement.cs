using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    public float speedMultiplier = 2f;
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        rb.AddForce(new Vector2(BasicInput.ins.InputLHorizontal*speedMultiplier, 0f), ForceMode2D.Impulse);
    }
}
