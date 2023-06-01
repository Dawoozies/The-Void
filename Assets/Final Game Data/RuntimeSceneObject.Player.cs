using ExtensionMethods_Bool;
using System.Collections.Generic;
using UnityEngine;

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
        //public List<RuntimeSceneObject> equipped = new List<RuntimeSceneObject>();
        //public List<RuntimeSceneObject> toBeThrown = new List<RuntimeSceneObject>();
        //public List<RuntimeSceneObject> extraThrown = new List<RuntimeSceneObject>();
        //public Halberd firstSlotObj;
        //public bool throwing;
        public List<Weapon> orbitingWeapons = new List<Weapon>();
        public float orbitParameter;
        public List<Weapon> readyWeapons = new List<Weapon>();
        public int throwCount;
        public void ManagedStart()
        {
            spriteRenderer.sortingOrder = 10;
            spriteRenderer.sortingLayerName = "Player";
            throwCount = 1;
            animatorStateChanged += () =>
            {
                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "StateSwap")
                {
                    circleCollider2DComponent = GameManagement.ins.circleCollider2DCache.LoadComponent(controller.name, stateHash);
                    overlapComponent = GameManagement.ins.overlapCache.LoadComponent(controller.name, stateHash);
                }
                StateOnEnter.Handle(this, stateHash, previousStateHash);
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
                StateOnFrameUpdate.Handle(this, frame, stateHash, previousStateHash);
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
            orbitParameter += tickDelta;
            if(orbitingWeapons.Count > 0)
            {
                //Hard code swap orbit type here for testing
                //When prototype near done then sort out "shop" window for choosing which to use
                int orbitTypeToUse = 0;
                switch (orbitTypeToUse)
                {
                    case 1:
                        WeaponOrbit.Update_UpLookAtDir(this, InputManager.ins.R_Input, orbitingWeapons, orbitParameter);
                        break;
                    case 0:
                        WeaponOrbit.Update_UpToSetRelativeDir(this, orbitingWeapons, orbitParameter);
                        break;
                }
                
            }
            if(readyWeapons.Count > 0)
            {
                WeaponThrow.Ready_Default(this, readyWeapons);
            }
        }
        public void ManagedFixedUpdate(float tickDelta)
        {
            RigidbodyUpdate(tickDelta);
        }
    }
    public static class StateOnEnter
    {
        public static void Handle(RuntimeSceneObject obj, int stateHash, int previousStateHash)
        {
            if(obj.ID == RuntimeIdentifier.Player)
            {
                Player player = obj as Player;
                if (player == null)
                    return;
                Vector3 aimDirection =
                    (Animator.StringToHash("AIM_HALBERD_DOWN")==stateHash).DefinedValue(0, 1)*-player.up
                    +(Animator.StringToHash("AIM_HALBERD_DIAGDOWN") == stateHash).DefinedValue(0,1)*(player.right-player.up)
                    +(Animator.StringToHash("AIM_HALBERD_HORIZONTAL") == stateHash).DefinedValue(0, 1) * player.right
                    +(Animator.StringToHash("AIM_HALBERD_DIAGUP") == stateHash).DefinedValue(0, 1) * (player.right + player.up)
                    +(Animator.StringToHash("AIM_HALBERD_UP") == stateHash).DefinedValue(0, 1) * (player.up);
                bool aimingStopped =
                    ((Animator.StringToHash("AIM_HALBERD_DOWN") == previousStateHash)
                    || (Animator.StringToHash("AIM_HALBERD_DIAGDOWN") == previousStateHash)
                    || (Animator.StringToHash("AIM_HALBERD_HORIZONTAL") == previousStateHash)
                    || (Animator.StringToHash("AIM_HALBERD_DIAGUP") == previousStateHash)
                    || (Animator.StringToHash("AIM_HALBERD_UP") == previousStateHash))
                    && (player.animatorClipInfo.clip.name != "DirectionSwap");
                if(aimDirection.magnitude > 0)
                {
                    if(player.orbitingWeapons.Count > 0)
                    {
                        Debug.LogError($"Orbiting weapons stuff on frame {player.frame}");
                        for (int i = 0; i < player.throwCount; i++)
                        {
                            if (player.orbitingWeapons.Count == 0)
                                break;
                            //Debug.LogError("Adding to ready throw");
                            player.readyWeapons.Add(player.orbitingWeapons[0]);
                            player.orbitingWeapons.Remove(player.orbitingWeapons[0]);
                        }
                    }
                }
                if (aimingStopped)
                {
                    //Debug.LogError($"Ready Weapons Count {player.readyWeapons.Count}");
                    if(player.readyWeapons.Count > 0)
                    {
                        int index = 0;
                        while(player.readyWeapons.Count > 0)
                        {
                            //Debug.LogError($"REMOVING {player.readyWeapons[0].transform.gameObject.name}");
                            player.orbitingWeapons.Add(player.readyWeapons[0]);
                            player.readyWeapons.Remove(player.readyWeapons[0]);
                            if (index > 500)
                            {
                                Debug.LogError("INFINITE WHILE LOOP");
                                break;
                            }
                        }
                    }
                }
                Vector3 throwDirection = 
                    (Animator.StringToHash("THROW_HALBERD_DOWN") == stateHash).DefinedValue(0, 1) * -player.up
                    + (Animator.StringToHash("THROW_HALBERD_DIAGDOWN") == stateHash).DefinedValue(0, 1) * (player.right - player.up)
                    + (Animator.StringToHash("THROW_HALBERD_HORIZONTAL") == stateHash).DefinedValue(0, 1) * player.right
                    + (Animator.StringToHash("THROW_HALBERD_DIAGUP") == stateHash).DefinedValue(0, 1) * (player.right + player.up)
                    + (Animator.StringToHash("THROW_HALBERD_UP") == stateHash).DefinedValue(0, 1) * (player.up);
            }
            if(obj.ID == RuntimeIdentifier.Weapon)
            {
                Weapon weapon = obj as Weapon;
                if (weapon == null)
                    return;
                if(Animator.StringToHash("RECALL") == stateHash && Animator.StringToHash("RECALLSTART") == previousStateHash)
                {
                    Player player = GameManagement.ins.playerObj;
                    //weapon.rb.velocity = -40f * weapon.up;
                    weapon.animator.SetBool("Embedded", false);
                }
            }
        }
    }
    public static class StateOnFrameUpdate
    {
        public static void Handle(RuntimeSceneObject obj, int frame, int stateHash, int previousStateHash)
        {
            if (obj.ID == RuntimeIdentifier.Player)
            {
                Player player = obj as Player;
                if (player == null)
                    return;
                player.rb.velocity = 15f * InputManager.ins.L_Input.x * player.right + player.upVelocity;
                bool spriteFlipAllowed =
                    (Animator.StringToHash("RUN") == stateHash)
                    || (Animator.StringToHash("RECALL_RUN") == stateHash)
                    || (Animator.StringToHash("CATCH_HALBERD") == stateHash);
                if (spriteFlipAllowed)
                {
                    if (player.spriteRenderer.flipX && InputManager.ins.L_Input.x > 0)
                        player.spriteRenderer.flipX = false;
                    if (!player.spriteRenderer.flipX && InputManager.ins.L_Input.x < 0)
                        player.spriteRenderer.flipX = true;
                }
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
                Vector3 aimDirection =
                    (Animator.StringToHash("AIM_HALBERD_DOWN") == stateHash).DefinedValue(0, 1) * -player.up
                    + (Animator.StringToHash("AIM_HALBERD_DIAGDOWN") == stateHash).DefinedValue(0, 1) * (player.right - player.up)
                    + (Animator.StringToHash("AIM_HALBERD_HORIZONTAL") == stateHash).DefinedValue(0, 1) * player.right
                    + (Animator.StringToHash("AIM_HALBERD_DIAGUP") == stateHash).DefinedValue(0, 1) * (player.right + player.up)
                    + (Animator.StringToHash("AIM_HALBERD_UP") == stateHash).DefinedValue(0, 1) * (player.up);
                bool aiming =
                    Animator.StringToHash("AIM_HALBERD_DOWN") == stateHash
                    || Animator.StringToHash("AIM_HALBERD_DIAGDOWN") == stateHash
                    || Animator.StringToHash("AIM_HALBERD_HORIZONTAL") == stateHash
                    || Animator.StringToHash("AIM_HALBERD_DIAGUP") == stateHash
                    || Animator.StringToHash("AIM_HALBERD_UP") == stateHash;
                if(aiming)
                {
                    if (player.spriteRenderer.flipX && InputManager.ins.R_Input.x > 0)
                        player.spriteRenderer.flipX = false;
                    if (!player.spriteRenderer.flipX && InputManager.ins.R_Input.x < 0)
                        player.spriteRenderer.flipX = true;
                    if (InputManager.ins.RightBumper_Input)
                    {
                        Debug.LogError($"Right bumper input on frame {player.frame}");
                        if(player.readyWeapons.Count > 0)
                        {
                            aimDirection.x *= player.spriteRenderer.flipX.DefinedValue(1, -1);
                            int throwTypeChoice = 1;
                            switch (throwTypeChoice)
                            {
                                case 1:
                                    WeaponThrow.Throw_SemiAuto(player, aimDirection, player.readyWeapons);
                                    break;
                                case 0:
                                    WeaponThrow.Throw_Default(player, aimDirection, player.readyWeapons);
                                    break;
                            }
                            
                            if ((Animator.StringToHash("AIM_HALBERD_DOWN") == stateHash))
                                player.animator.Play("THROW_HALBERD_DOWN");
                            if ((Animator.StringToHash("AIM_HALBERD_DIAGDOWN") == stateHash))
                                player.animator.Play("THROW_HALBERD_DIAGDOWN");
                            if ((Animator.StringToHash("AIM_HALBERD_HORIZONTAL") == stateHash))
                                player.animator.Play("THROW_HALBERD_HORIZONTAL");
                            if ((Animator.StringToHash("AIM_HALBERD_DIAGUP") == stateHash))
                                player.animator.Play("THROW_HALBERD_DIAGUP");
                            if ((Animator.StringToHash("AIM_HALBERD_UP") == stateHash))
                                player.animator.Play("THROW_HALBERD_UP");
                        }
                    }
                }
                if (Animator.StringToHash("AIM_HALBERD_DOWN") == stateHash)
                {

                }
                if (Animator.StringToHash("AIM_HALBERD_DIAGDOWN") == stateHash)
                {

                }
                if (Animator.StringToHash("AIM_HALBERD_HORIZONTAL") == stateHash)
                {

                }
                if (Animator.StringToHash("AIM_HALBERD_DIAGUP") == stateHash)
                {

                }
                if (Animator.StringToHash("AIM_HALBERD_UP") == stateHash)
                {

                }
            }
            if(obj.ID == RuntimeIdentifier.Weapon)
            {
                Weapon weapon = obj as Weapon;
                if (weapon == null)
                    return;
                if (Animator.StringToHash("FALL") == stateHash)
                {
                    weapon.rb.AddForce(GameManagement.ins.gravityDirection * 80f);
                    if (weapon.upSpeed < weapon.fallSpeedMax && Animator.StringToHash("RECALL") != previousStateHash)
                    {
                        weapon.rb.velocity = weapon.rightVelocity + weapon.fallSpeedMax * weapon.up;
                    }
                    if(weapon.upSpeed < weapon.fallSpeedMax+weapon.recallSpeedMax && Animator.StringToHash("RECALL") == previousStateHash)
                    {
                        weapon.rb.velocity = weapon.rightVelocity + (weapon.fallSpeedMax + weapon.recallSpeedMax) * weapon.up;
                    }
                    weapon.transform.up = Vector3.SmoothDamp(weapon.transform.up, weapon.rb.velocity, ref weapon.dirChange, weapon.dirChangeSmoothTime);
                }
                bool embedding = 
                    Animator.StringToHash("EMBED") == stateHash 
                    || Animator.StringToHash("EMBEDDED") == stateHash;
                if(embedding)
                {
                    weapon.spriteRenderer.sortingLayerName = "EmbeddedWeapon";
                }
                else
                {
                    if (weapon.rb.transform.parent != null)
                        weapon.rb.transform.parent = null;
                    if (weapon.spriteRenderer.sortingLayerName == "EmbeddedWeapon")
                        weapon.spriteRenderer.sortingLayerName = "Player";
                }
                if (Animator.StringToHash("EMBED") == stateHash)
                {
                    weapon.rb.velocity = Vector3.zero;
                }
                if(Animator.StringToHash("RECALL") == stateHash)
                {
                    weapon.transform.up = Vector3.SmoothDamp(weapon.transform.up, -weapon.dirTo(GameManagement.ins.playerObj.rb.transform.position), ref weapon.dirChange, weapon.dirChangeSmoothTime);

                    weapon.rb.AddForce(weapon.dirTo(GameManagement.ins.playerObj.rb.transform.position) * 150f);
                    if (weapon.upSpeed < weapon.recallSpeedMax)
                    {
                        weapon.rb.velocity = weapon.rightVelocity/2f + weapon.recallSpeedMax * weapon.up;
                    }
                }
                if(Animator.StringToHash("READYTHROW") == stateHash)
                {
                    Player player = GameManagement.ins.playerObj;
                    weapon.rb.transform.position = player.transform.position;
                }
            }
        }
    }
}