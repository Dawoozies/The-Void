using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornetSecondPass : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        //When jump button down start jump squat
        //When jump squat end
    }
}
