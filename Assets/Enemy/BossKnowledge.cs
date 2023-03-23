using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LinearAlgebra;
public class BossKnowledge : MonoBehaviour
{
    Transform playerTransform;
    Animator animator;

    ParametrisedLine linearPathToPlayer = new ParametrisedLine();

    public float closeThreshold;
    public float closeThresholdX;
    public float closeThresholdY;
    Vector3 direction;
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

        linearPathToPlayer.pathStart = transform.position;
        linearPathToPlayer.pathEnd = playerTransform.position;
        direction = linearPathToPlayer.direction;

        animator.SetBool("closeToPlayer", (GetDistanceFromPlayer() < closeThreshold));
        animator.SetBool("closeToPlayerX", (GetXDistanceFromPlayer() < closeThresholdX));
        animator.SetBool("closeToPlayerY", (GetYDistanceFromPlayer() < closeThresholdY));
        animator.SetBool("facingPlayerX", (GetFaceDirectionX() == GetDirectionX()));
    }

    public Vector3 GetPlayerPosition ()
    {
        return playerTransform.position;
    }

    public float GetDistanceFromPlayer()
    {
        return Vector3.Distance(transform.position, GetPlayerPosition());
    }

    public float GetXDistanceFromPlayer()
    {
        return Mathf.Abs(transform.position.x - GetPlayerPosition().x);
    }

    public float GetYDistanceFromPlayer()
    {
        return Mathf.Abs(transform.position.y - GetPlayerPosition().y);
    }

    public int GetDirectionX ()
    {
        return Mathf.RoundToInt(Mathf.Sign(direction.x));
    }
    public int GetDirectionY ()
    {
        return Mathf.RoundToInt(Mathf.Sign(direction.y));
    }

    public int GetFaceDirectionX ()
    {
        return Mathf.RoundToInt(Mathf.Sign(animator.transform.localScale.x));
    }

    private void OnDrawGizmos()
    {
        if (playerTransform == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + direction);
    }
}
