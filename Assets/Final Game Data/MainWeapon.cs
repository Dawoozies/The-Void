using DataStructures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameManagement
{
    public class Halberd : RuntimeSceneObject
    {
        public Component_CircleCollider2D circleCollider2DComponent;
        public Component_Overlap overlapComponent;
        const int colliderRequestAmount = 15;
        bool hasColliders = false;
        public MainWeaponID weaponID = MainWeaponID.Halberd;
        public bool embedded = false;
        float VelocityUp => Vector3.Dot(rb.velocity, transform.up);
        float VelocityRight => Vector3.Dot(rb.velocity, transform.right);
        float fallSpeedMax = -15f;
        float throwSpeedMax = 40f;
        Player player;
        Vector3 directionChange;
        const float directionChangeSmoothTime = 0.015f;
        Vector3 gravityDir => GameManagement.ins.gravityDirection;
        public bool recalling => player.recalling;
        public bool inPlayerInventory = false;
        Vector3 playerPos => player.rb.transform.position;
        float recallSpeedMax = 60f;
        Vector3 dirToPlayer => (playerPos - rb.transform.position).normalized;
        float VelocityToPlayer => Vector3.Dot(rb.velocity, dirToPlayer);
        float t = 0;
        float s = 0;
        float orderSign;
        bool beingCaught;
        Vector3 posChange;
        const float posChangeSmoothTime = 0.125f;
        public void ManagedStart()
        {
            player = GameManagement.ins.playerObj;
            spriteRenderer.sortingOrder = 10;
            orderSign = 1;
            animatorStateChanged += () =>
            {
                circleCollider2DComponent = GameManagement.ins.circleCollider2DCache.LoadComponent(controller.name, stateHash);
                overlapComponent = GameManagement.ins.overlapCache.LoadComponent(controller.name, stateHash);
            };
            animatorFrameChanged += () =>
            {
                if (circleCollider2DComponent != null && circleCollider2DComponent.componentData.Length > 0)
                {
                    Component_CircleCollider2D_Data[] circleCollider2DDataAtFrame = circleCollider2DComponent.DataWithFrame(frame);
                    UpdateLedger.CircleCollider2D(GameManagement.ins.circleCollider2DBank, this, circleCollider2DDataAtFrame);
                }
                else
                {
                    UpdateLedger.NullComponent(GameManagement.ins.circleCollider2DBank, this);
                }
                if (overlapComponent != null && overlapComponent.componentData.Length > 0)
                {
                    Component_Overlap_Data[] overlapComponentDataAtFrame = overlapComponent.DataWithFrame(frame);
                    if (overlapComponentDataAtFrame != null)
                    {
                        foreach (Component_Overlap_Data componentData in overlapComponentDataAtFrame)
                        {
                            GameManagement.ins.overlapManager.OverlapApply(this, componentData);
                        }
                    }
                }
            };
            hasColliders = GameManagement.ins.circleCollider2DBank.RequestLoanForRigidbody2D(this, colliderRequestAmount);
        }
        public void ManagedUpdate(float tickDelta)
        {
            AnimatorUpdate(tickDelta);
            animator.SetBool("Embedded", embedded);
            animator.SetBool("InPlayerInventory", inPlayerInventory);
            animator.SetBool("Recalling", recalling);
            animator.SetInteger("Equipped:MainWeaponID", (int)player.mainWeaponID);
            if(Animator.StringToHash("INVENTORY_HALBERD") == animatorStateInfo.shortNameHash)
            {
                spriteRenderer.color = Color.clear;
                rb.velocity = Vector3.zero;
                rb.transform.position = playerPos;
            }
            else
            {
                //spriteRenderer.color = Color.white;
            }
            spriteRenderer.sortingOrder = 10 + Mathf.RoundToInt(orderSign);
            beingCaught = Animator.StringToHash("CATCH_HALBERD") == player.animatorStateInfo.shortNameHash;
            if (beingCaught)
                spriteRenderer.color = Color.clear;
        }
        public void ManagedFixedUpdate(float tickDelta)
        {
            //Update physics to only work frame by frame
            //Might sort out a lot of problems
            if(Animator.StringToHash("FALL_HALBERD") == animatorStateInfo.shortNameHash)
            {
                rb.AddForce(gravityDir * 10f);
                if(VelocityUp < fallSpeedMax)
                {
                    rb.velocity = VelocityRight * transform.right + fallSpeedMax * transform.up;
                }
                transform.up = Vector3.SmoothDamp(transform.up, rb.velocity, ref directionChange, directionChangeSmoothTime);
            }
            if(Animator.StringToHash("EMBED_HALBERD") == animatorStateInfo.shortNameHash)
            {
                rb.velocity = Vector3.zero;
            }
            if(Animator.StringToHash("RECALL_HALBERD") == animatorStateInfo.shortNameHash)
            {
                transform.up = Vector3.SmoothDamp(transform.up, -dirToPlayer, ref directionChange, directionChangeSmoothTime);
                rb.AddForce(-transform.up * 200f);
                if(VelocityUp < -recallSpeedMax)
                {
                    rb.velocity = VelocityRight * transform.right - recallSpeedMax * transform.up;
                }
            }
            if (Animator.StringToHash("EQUIPPED_HALBERD") == animatorStateInfo.shortNameHash)
            {
                //orbit around player
                t += tickDelta;
                s += tickDelta;
                transform.up = player.transform.up;
                rb.transform.position = Vector3.SmoothDamp(rb.transform.position,
                    playerPos
                    + player.transform.right * 1.5f * Mathf.Cos(-Mathf.PI + t)
                    + player.transform.up * 0.5f
                    , ref posChange, posChangeSmoothTime);
                transform.localScale = new Vector3(1+0.125f*Mathf.Sin(t), 1+0.125f*Mathf.Sin(t), 1);
                if(!beingCaught)
                    spriteRenderer.color = Color.white + new Color(1f,1f,1f,0f)*0.25f*Mathf.Sin(t);
                if(s > Mathf.PI)
                {
                    orderSign *= -1;
                    s = 0;
                }
                if (t > Mathf.PI * 2f)
                    t = 0;
            }
        }
    }
}