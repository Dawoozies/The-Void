using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ThrownWeaponTest : MonoBehaviour
{
    public static ThrownWeaponTest ins;
    private void Awake()
    {
        ins = this;
    }
    public Transform halberdParent;
    public Rigidbody2D halberdRigidbody;
    public Animator halberdAnimator;
    public float velocityMultiplier;
    public void LaunchHalberd(Vector3 launchPos, Vector3 launchVelocity)
    {
        if (halberdParent == null)
            return;
        if (halberdAnimator == null)
            return;

        halberdParent.position = launchPos;
        halberdRigidbody.velocity = launchVelocity*velocityMultiplier;
        halberdAnimator.transform.up = launchVelocity.normalized;
        halberdAnimator.transform.localScale = new Vector3(Mathf.Sign(launchVelocity.x), halberdAnimator.transform.localScale.y, halberdAnimator.transform.localScale.z);
        Debug.Log(halberdRigidbody.velocity);
        halberdAnimator.Play("Launch");
    }
}
