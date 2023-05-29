using ExtensionMethods_Bool;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace GameManagement
{
    public class Player : RuntimeSceneObject
    {
        public Component_CircleCollider2D circleCollider2DComponent;
        public Component_Overlap overlapComponent;
        const int colliderRequestAmount = 18;
        bool hasColliders = false;
        public bool grounded = true;
        bool jump => InputManager.ins.JumpDown_Input || InputManager.ins.JumpDown_BufferedInput;
        public float fallSpeedMax = -20f;
        public float ascentSpeedMax = 20f;
        public bool recalling => (InputManager.ins.LeftBumper_Input);
        public bool aimingWeapon => InputManager.ins.LeftTrigger_Input > 0.1f;
        public List<RuntimeSceneObject> equipped = new List<RuntimeSceneObject>();
        public List<RuntimeSceneObject> toBeThrown = new List<RuntimeSceneObject>();
        public List<RuntimeSceneObject> extraThrown = new List<RuntimeSceneObject>();
        public Halberd firstSlotObj;
        public float orbitParameter;
        public bool throwing;
        public void ManagedStart()
        {
            spriteRenderer.sortingOrder = 10;
            animatorStateChanged += () =>
            {
                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "StateSwap")
                    return;
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
            animator.SetBool("Grounded", grounded);
            animator.SetBool("Jump", jump);
            animator.SetBool("Run", Mathf.Abs(InputManager.ins.L_Input.x) > 0);
            animator.SetFloat("VelocityUp", upSpeed);
            animator.SetFloat("VelocityRight", rightSpeed);
            animator.SetBool("Recalling", recalling);
            animator.SetFloat("LeftTrigger_Input", InputManager.ins.LeftTrigger_Input);
            animator.SetInteger("R_Direction", Direction.Compute8WayDirection());
            animator.SetBool("RightBumper_Input", InputManager.ins.RightBumper_Input);
            if (aimingWeapon)
            {
                if (spriteRenderer.flipX && InputManager.ins.R_Input.x > 0)
                    spriteRenderer.flipX = false;
                if (!spriteRenderer.flipX && InputManager.ins.R_Input.x < 0)
                    spriteRenderer.flipX = true;
            }
            else
            {
                if (spriteRenderer.flipX && InputManager.ins.L_Input.x > 0)
                    spriteRenderer.flipX = false;
                if (!spriteRenderer.flipX && InputManager.ins.L_Input.x < 0)
                    spriteRenderer.flipX = true;
            }
            if(equipped.Count > 0)
            {
                for (int i = 0; i < equipped.Count; i++)
                {
                    Halberd halberd = equipped[i] as Halberd;
                    if (halberd != null)
                    {
                        halberd.UpdateOrbit(this, i, equipped.Count, orbitParameter);
                    }
                }
                firstSlotObj = equipped[0] as Halberd;
                if (firstSlotObj != null)
                    animator.SetInteger("InFirstSlotID", (int)firstSlotObj.weaponID);
            }
            orbitParameter += tickDelta;
            if(!aimingWeapon && !throwing)
            {
                if (toBeThrown.Count > 0)
                {
                    Debug.Log("toBeThrown Count = " + toBeThrown.Count);
                    Debug.Log("equipped Count = " + equipped.Count);
                    Halberd halberd = toBeThrown[0] as Halberd;
                    if(!halberd.throwStatus)
                    {
                        equipped.Insert(0, toBeThrown[0]);
                        toBeThrown.RemoveAt(0);
                    }
                    else
                    {
                        toBeThrown.RemoveAt(0);
                    }
                }
            }
        }
        public void ManagedFixedUpdate(float tickDelta)
        {

        }
    }
    public static class States
    {
        public static void OnStateEnter(RuntimeSceneObject obj, int stateHash)
        {
            if(obj.ID == RuntimeIdentifier.Player)
            {
                Player player = obj as Player;
                if (player == null)
                    return;
                if(Animator.StringToHash("AIM_HALBERD_DOWN") == stateHash)
                {
                    if(player.equipped.Contains(player.firstSlotObj))
                    {
                        player.toBeThrown.Add(player.firstSlotObj);
                        player.equipped.Remove(player.firstSlotObj);
                    }
                }
                if (Animator.StringToHash("AIM_HALBERD_DIAGDOWN") == stateHash)
                {
                    if (player.equipped.Contains(player.firstSlotObj))
                    {
                        player.toBeThrown.Add(player.firstSlotObj);
                        player.equipped.Remove(player.firstSlotObj);
                    }
                }
                if (Animator.StringToHash("AIM_HALBERD_HORIZONTAL") == stateHash)
                {
                    if (player.equipped.Contains(player.firstSlotObj))
                    {
                        player.toBeThrown.Add(player.firstSlotObj);
                        player.equipped.Remove(player.firstSlotObj);
                    }
                }
                if (Animator.StringToHash("AIM_HALBERD_DIAGUP") == stateHash)
                {
                    if (player.equipped.Contains(player.firstSlotObj))
                    {
                        player.toBeThrown.Add(player.firstSlotObj);
                        player.equipped.Remove(player.firstSlotObj);
                    }
                }
                if (Animator.StringToHash("AIM_HALBERD_UP") == stateHash)
                {
                    if (player.equipped.Contains(player.firstSlotObj))
                    {
                        player.toBeThrown.Add(player.firstSlotObj);
                        player.equipped.Remove(player.firstSlotObj);
                    }
                }
                if (Animator.StringToHash("THROW_HALBERD_DOWN") == stateHash)
                {
                    player.throwing = true;
                }
                if (Animator.StringToHash("THROW_HALBERD_DIAGDOWN") == stateHash)
                {
                    player.throwing = true;
                }
                if (Animator.StringToHash("THROW_HALBERD_HORIZONTAL") == stateHash)
                {
                    player.throwing = true;
                }
                if (Animator.StringToHash("THROW_HALBERD_DIAGUP") == stateHash)
                {
                    player.throwing = true;
                }
                if (Animator.StringToHash("THROW_HALBERD_UP") == stateHash)
                {
                    player.throwing = true;
                }
            }
            if(obj.ID == RuntimeIdentifier.Halberd)
            {
                Halberd halberd = obj as Halberd;
                if (halberd == null)
                    return;
                if(Animator.StringToHash("RECALL_HALBERD") == stateHash)
                {
                    halberd.embedded = false;
                }
                if(Animator.StringToHash("INVENTORY_HALBERD") == stateHash)
                {
                    halberd.embedded = false;
                }
                if (Animator.StringToHash("EQUIPPED_HALBERD") == stateHash)
                {
                    halberd.embedded = false;
                }
                if(Animator.StringToHash("LAUNCH_HALBERD") == stateHash)
                {
                    halberd.spriteRenderer.color = Color.white;
                    Debug.Log($"Halberd throw dir {halberd.throwDirection}");
                    //halberd.rb.velocity += 15f*(Vector2)halberd.throwDirection;
                    Player player = GameManagement.ins.playerObj;
                    for (int i = 0; i < player.toBeThrown.Count; i++)
                    {
                        Halberd toThrow = player.toBeThrown[i] as Halberd;
                        Debug.Log($"Halberd {i} Throw Direction = {toThrow.throwDirection}");
                        float maxCount = player.toBeThrown.Count;
                        float accuracyMaxBound = i;
                        Vector2 throwPathDeviation = toThrow.throwDirection;
                        //Flipping the dot product to add velocity in perpendicular component
                        throwPathDeviation = Random.Range(-accuracyMaxBound, accuracyMaxBound)*Vector3.Dot(player.right, toThrow.throwDirection) * player.up
                            + Random.Range(-accuracyMaxBound, accuracyMaxBound)*Vector3.Dot(player.up, toThrow.throwDirection) * player.right;
                        halberd.rb.velocity = toThrow.throwSpeedMax * (Vector2)toThrow.throwDirection + (i/maxCount) *throwPathDeviation;
                        halberd.transform.up = halberd.rb.velocity;
                    }
                }
            }
        }
        public static void OnFrame(RuntimeSceneObject obj, int stateHash)
        {
            if(obj.ID == RuntimeIdentifier.Player)
            {
                Player player = obj as Player;
                //Debug.Log(player.frame);
                if (player == null)
                    return;
                player.rb.velocity = 15f * InputManager.ins.L_Input.x * player.right + player.upVelocity;
                if (Animator.StringToHash("FALL") == stateHash)
                {
                    player.rb.AddForce(-player.up * 20f);
                    if (player.upSpeed < player.fallSpeedMax)
                    {
                        player.rb.velocity = player.rightVelocity + player.fallSpeedMax * player.up;
                    }
                }
                if (Animator.StringToHash("JUMP") == stateHash)
                {
                    player.rb.velocity += player.up * 5f;
                    if (player.upSpeed > player.ascentSpeedMax)
                    {
                        player.rb.velocity = player.rightVelocity + player.ascentSpeedMax * player.up;
                    }
                }
                if (Animator.StringToHash("ASCENTSLOW") == stateHash)
                {
                    player.rb.AddForce(-player.up * 40f);
                    if (player.upSpeed < player.fallSpeedMax)
                    {
                        player.rb.velocity = player.rightVelocity + player.fallSpeedMax * player.up;
                    }
                }
                if (Animator.StringToHash("RECALL_AIR") == stateHash)
                {
                    if (player.upSpeed > 3)
                    {
                        player.rb.AddForce(-player.up * 70f);
                    }
                    if (player.upSpeed > 0 && player.upSpeed <= 3)
                    {
                        //Low grav
                        player.rb.AddForce(-player.up * 10f);
                    }
                    if (player.upSpeed <= 0)
                    {
                        player.rb.AddForce(-player.up * 20f);
                    }
                    if (player.upSpeed < player.fallSpeedMax)
                    {
                        player.rb.velocity = player.rightVelocity + player.fallSpeedMax * player.up;
                    }
                }
                if(Animator.StringToHash("THROW_HALBERD_DOWN") == stateHash)
                {
                    //Frame 8 is where throw actually happens
                    if(player.frame == 8)
                    {
                        player.rb.velocity += 10f * player.up;
                        if(player.toBeThrown.Count > 0)
                        {
                            for (int i = 0; i < player.toBeThrown.Count; i++)
                            {
                                Halberd halberd = player.toBeThrown[i] as Halberd;
                                halberd.throwStatus = true;
                                halberd.throwDirection = -player.up;
                            }
                        }
                        player.throwing = false;
                    }
                }
                if(Animator.StringToHash("THROW_HALBERD_DIAGDOWN") == stateHash)
                {
                    if(player.frame == 8)
                    {
                        player.rb.velocity += ((InputManager.ins.L_Input.y > 0).DefinedValue(0, 1)*InputManager.ins.L_Input.y*10f + 2.5f) * player.up;
                        if (player.toBeThrown.Count > 0)
                        {
                            for (int i = 0; i < player.toBeThrown.Count; i++)
                            {
                                Halberd halberd = player.toBeThrown[i] as Halberd;
                                halberd.throwStatus = true;
                                halberd.throwDirection = player.right - player.up;
                            }
                        }
                        player.throwing = false;
                    }
                }
                if (Animator.StringToHash("THROW_HALBERD_HORIZONTAL") == stateHash)
                {
                    if (player.frame == 8)
                    {
                        //player.rb.velocity += InputManager.ins.L_Input.y * 15f * player.up;
                        if (player.toBeThrown.Count > 0)
                        {
                            for (int i = 0; i < player.toBeThrown.Count; i++)
                            {
                                Halberd halberd = player.toBeThrown[i] as Halberd;
                                halberd.throwStatus = true;
                                halberd.throwDirection = player.right;
                            }
                        }
                        player.throwing = false;
                    }
                }
                if (Animator.StringToHash("THROW_HALBERD_DIAGUP") == stateHash)
                {
                    if (player.frame == 8)
                    {
                        //player.rb.velocity += 10f * player.up;
                        if (player.toBeThrown.Count > 0)
                        {
                            for (int i = 0; i < player.toBeThrown.Count; i++)
                            {
                                Halberd halberd = player.toBeThrown[i] as Halberd;
                                halberd.throwStatus = true;
                                halberd.throwDirection = player.right + player.up;
                            }
                        }
                        player.throwing = false;
                    }
                }
                if (Animator.StringToHash("THROW_HALBERD_UP") == stateHash)
                {
                    if (player.frame == 8)
                    {
                        //player.rb.velocity += 10f * player.up;
                        if (player.toBeThrown.Count > 0)
                        {
                            for (int i = 0; i < player.toBeThrown.Count; i++)
                            {
                                Halberd halberd = player.toBeThrown[i] as Halberd;
                                halberd.throwStatus = true;
                                halberd.throwDirection = player.up;
                            }
                        }
                        player.throwing = false;
                    }
                }
            }
            if(obj.ID == RuntimeIdentifier.Halberd)
            {
                Halberd halberd = obj as Halberd;
                if (halberd == null)
                    return;
                Debug.Log($"Halberd Frame {halberd.frame}");
                if (Animator.StringToHash("FALL_HALBERD") == stateHash)
                {
                    halberd.rb.AddForce(GameManagement.ins.gravityDirection * 10f);
                    if (halberd.upSpeed < halberd.fallSpeedMax)
                    {
                        halberd.rb.velocity = halberd.rightVelocity + halberd.fallSpeedMax*halberd.up;
                    }
                    halberd.transform.up = Vector3.SmoothDamp(halberd.transform.up, halberd.rb.velocity, ref halberd.directionChange, halberd.directionChangeSmoothTime);
                }
                if (Animator.StringToHash("EMBED_HALBERD") == stateHash)
                {
                    halberd.rb.velocity = Vector3.zero;
                }
                if (Animator.StringToHash("RECALL_HALBERD") == stateHash)
                {
                    halberd.transform.up = Vector3.SmoothDamp(halberd.transform.up, -halberd.dirTo(GameManagement.ins.playerObj.rb.transform.position), ref halberd.directionChange, halberd.directionChangeSmoothTime);
                    halberd.rb.AddForce(-halberd.up * 50f);
                    if (halberd.upSpeed < -halberd.recallSpeedMax)
                    {
                        halberd.rb.velocity = halberd.rightVelocity - halberd.recallSpeedMax * halberd.up;
                    }
                }
                if (Animator.StringToHash("INVENTORY_HALBERD") == stateHash)
                {
                    halberd.spriteRenderer.color = Color.clear;
                    halberd.rb.velocity = Vector3.zero;
                    halberd.rb.transform.position = GameManagement.ins.playerObj.rb.transform.position;
                }
                if (Animator.StringToHash("EQUIPPED_HALBERD") == stateHash)
                {
                    halberd.embedded = false;
                    halberd.localTickRateMultiplier = 4f;
                    halberd.spriteRenderer.color = Color.white;
                    if (GameManagement.ins.playerObj.equipped.Contains(halberd))
                    {
                        halberd.rb.MovePosition(Vector3.SmoothDamp(halberd.rb.transform.position, halberd.orbitPos, ref halberd.posChange, halberd.posChangeSmoothTime + halberd.parallaxFactor));
                    }
                }
                if (Animator.StringToHash("READYTHROW_HALBERD") == stateHash)
                {
                    halberd.spriteRenderer.color = Color.clear;
                    halberd.rb.transform.position = GameManagement.ins.playerObj.rb.transform.position;
                }
            }
        }
    }
}