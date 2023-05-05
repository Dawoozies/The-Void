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
    public void Update()
    {
        if (halberdAnimator == null)
            return;
        halberdAnimator.SetBool("Recall", PlayerDataManager.ins.Recalling);
    }
    public void LaunchHalberd(Vector3 launchPos, Vector3 launchDirection)
    {
        if (halberdParent == null)
            return;
        if (halberdAnimator == null)
            return;

        halberdParent.position = launchPos;
        halberdAnimator.transform.up = launchDirection.normalized;
        halberdAnimator.transform.localScale = new Vector3(Mathf.Sign(launchDirection.x), halberdAnimator.transform.localScale.y, halberdAnimator.transform.localScale.z);
        PlayerDataManager.ins.EquippedHalberd = false; //Should be in change variable
        halberdAnimator.Play("ThrownHalberd_Launch");
    }
    public void CaughtHalberd()
    {
        if (halberdParent == null)
            return;
        if (halberdAnimator == null)
            return;
        if (halberdRigidbody == null)
            return;
        halberdParent.position = new Vector3(-1000, -1000, 0);
        halberdRigidbody.velocity = Vector3.zero;
        halberdAnimator.Play("ThrownHalberd_Idle");
    }
}
