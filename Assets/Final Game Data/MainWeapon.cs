using DataStructures;
using ExtensionMethods_Bool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameManagement
{
    public class Halberd : RuntimeSceneObject
    {
        public Component_CircleCollider2D circleCollider2DComponent;
        public Component_Overlap overlapComponent;
        const int colliderRequestAmount = 5;
        bool hasColliders = false;
        public MainWeaponID weaponID = MainWeaponID.Halberd;
        public bool embedded = false;
        public float fallSpeedMax = -15f;
        public float throwSpeedMax = 40f;
        Player player;
        public Vector3 directionChange;
        public float directionChangeSmoothTime = 0.015f;
        public bool recalling => player.recalling;
        public bool inPlayerInventory = false;
        Vector3 playerPos => player.rb.transform.position;
        public float recallSpeedMax = 60f;

        public float orderSign;
        public bool beingCaught;
        public Vector3 posChange;
        public float posChangeSmoothTime = 0.025f;
        public float parallaxFactor = 0f;
        public float t;
        public float s;
        public float t_prevFrame;
        public float s_prevFrame;
        Vector3 orbitPos;
        Vector3 nextOrbitPos;
        public void ManagedStart()
        {
            player = GameManagement.ins.playerObj;
            animatorStateChanged += () =>
            {
                circleCollider2DComponent = GameManagement.ins.circleCollider2DCache.LoadComponent(controller.name, stateHash);
                overlapComponent = GameManagement.ins.overlapCache.LoadComponent(controller.name, stateHash);
                States.OnStateEnter(this, animatorStateInfo.shortNameHash);
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
                States.OnFrame(this, animatorStateInfo.shortNameHash);
            };
            hasColliders = GameManagement.ins.circleCollider2DBank.RequestLoanForRigidbody2D(this, colliderRequestAmount);
        }
        public void ManagedUpdate(float tickDelta)
        {
            AnimatorUpdate(tickDelta);
            animator.SetBool("Embedded", embedded);
            animator.SetBool("InPlayerInventory", player.inventory.Contains(this) || player.equipped.Contains(this));
            animator.SetBool("Recalling", recalling);
            animator.SetBool("InOrbit", player.equipped.Contains(this));
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
            if(Animator.StringToHash("EQUIPPED_HALBERD") == animatorStateInfo.shortNameHash)
            {
                if(spriteRenderer.color.a <= 0)
                    spriteRenderer.color = Color.white;
                if(player.equipped.Contains(this))
                {
                    rb.MovePosition(Vector3.SmoothDamp(rb.transform.position, orbitPos, ref posChange, posChangeSmoothTime + parallaxFactor));
                }
            }
        }
        public void ManagedFixedUpdate(float tickDelta)
        {
            //Update physics to only work frame by frame
            //Might sort out a lot of problems
        }
        public void UpdateOrbit(RuntimeSceneObject obj, float orbitIndex, float totalCount, float t)
        {
            transform.up = obj.up;
            Vector3 objPos = obj.rb.transform.position;
            float argument = (2f * Mathf.PI * orbitIndex) / totalCount;
            float xPos = Mathf.Cos(argument);
            float yPos = Mathf.Sin(argument);
            parallaxFactor = -0.01f* yPos;
            transform.localScale = Vector3.one - 0.125f * yPos *Vector3.one;
            spriteRenderer.color = new Color(1, 1, 1, 1) - new Color(1,1,1,0)*yPos*0.5f;
            spriteRenderer.sortingOrder = obj.spriteRenderer.sortingOrder + Mathf.RoundToInt((transform.localScale.y >= 1).DefinedValue(-1, 1));
            float xDist = rb.transform.position.x - obj.rb.transform.position.x;
            float shift = 0;
            Debug.Log($"Index {orbitIndex}, Distance from player = {xDist}");
            if(Mathf.Sign(InputManager.ins.L_Input.x) == Mathf.Sign(xDist))
            {
                shift = InputManager.ins.L_Input.x;
            }
            orbitPos = objPos + 1.75f*xPos * (Vector3)obj.right + shift*(Vector3)obj.right;
        }
    }
}