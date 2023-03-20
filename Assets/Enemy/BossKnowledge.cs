using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LinearAlgebra;
public class BossKnowledge : MonoBehaviour
{
    Transform playerTransform;
    Animator animator;

    ParametrisedLine linearPathToPlayer;

    public float closeThreshold;
    void Update()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.Find("Player").transform;

            if (playerTransform == null)
                return;
        }

        if(animator == null)
        {
            animator = GetComponentInChildren<Animator>();

            if (animator == null)
                return;
        }

        animator.SetBool("closeToPlayer", (GetDistanceFromPlayer() < closeThreshold));
    }

    public Vector3 GetPlayerPosition ()
    {
        return playerTransform.position;
    }

    public float GetDistanceFromPlayer()
    {
        return Vector3.Distance(transform.position, GetPlayerPosition());
    }
}
