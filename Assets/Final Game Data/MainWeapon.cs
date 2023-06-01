using DataStructures;
using ExtensionMethods_Bool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameManagement
{
    public class Weapon : RuntimeSceneObject
    {
        public Component_CircleCollider2D circleCollider2DComponent;
        public Component_Overlap overlapComponent;
        const int colliderRequestAmount = 5;
        public float fallSpeedMax;
        public Vector3 dirChange;
        public float dirChangeSmoothTime;
        public float recallSpeedMax;
        //
        public Vector3 orbitChange;
        public float orbitChangeSmoothTime;
        //
        public float throwSpeedMax;
        Player player => GameManagement.ins.playerObj;
        public void ManagedStart()
        {
            fallSpeedMax = -20f;
            dirChangeSmoothTime = 0.015f;
            recallSpeedMax = -60f;
            orbitChangeSmoothTime = 0.01f;
            throwSpeedMax = 80f;
            spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            spriteRenderer.sortingLayerName = "Player";
            animatorStateChanged += () =>
            {
                circleCollider2DComponent = GameManagement.ins.circleCollider2DCache.LoadComponent(controller.name, stateHash);
                overlapComponent = GameManagement.ins.overlapCache.LoadComponent(controller.name, stateHash);
                StateOnEnter.Handle(this, stateHash, previousStateHash);
            };
            animatorFrameChanged += () =>
            {
                if (circleCollider2DComponent != null && circleCollider2DComponent.componentData.Length > 0)
                    UpdateLedger.CircleCollider2D(GameManagement.ins.circleCollider2DBank, this, circleCollider2DComponent.DataWithFrame(frame));
                if (circleCollider2DComponent == null || circleCollider2DComponent.componentData.Length == 0)
                    UpdateLedger.NullComponent(GameManagement.ins.circleCollider2DBank, this);
                if(overlapComponent != null && overlapComponent.componentData.Length > 0)
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
            GameManagement.ins.circleCollider2DBank.RequestLoanForRigidbody2D(this, colliderRequestAmount);
        }
        public void ManagedUpdate(float tickDelta)
        {
            AnimatorUpdate(tickDelta);
            bool recall =
                (Animator.StringToHash("RECALL_RUN") == player.stateHash)
                || (Animator.StringToHash("RECALL_IDLE") == player.stateHash)
                || (Animator.StringToHash("RECALL_AIR") == player.stateHash)
                || (Animator.StringToHash("CATCH_HALBERD") == player.stateHash);
            animator.SetBool("Recalling", recall);
        }
        public void ManagedFixedUpdate(float tickDelta)
        {
            RigidbodyUpdate(tickDelta);
        }
    }
    public static class WeaponOrbit
    {
        public static void Update_UpToSetRelativeDir(RuntimeSceneObject target, List<Weapon> weapons, float t)
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                Weapon weapon = weapons[i];
                float orbitIndex = i;
                float total = weapons.Count;
                float argument = (2f * Mathf.PI * orbitIndex) / total;
                float xPos = Mathf.Cos(argument + t);
                float yPos = Mathf.Sin(argument + t);
                float parallaxFactor = -0.005f * yPos;
                weapon.transform.up = target.up;
                weapon.transform.localScale = Vector3.one - 0.125f * yPos * Vector3.one;
                weapon.spriteRenderer.color = new Color(1, 1, 1, 1) - new Color(1, 1, 1, 0) * yPos * 0.5f;
                weapon.spriteRenderer.sortingOrder = target.spriteRenderer.sortingOrder + Mathf.RoundToInt((weapon.transform.localScale.y >= 1).DefinedValue(-1, 1));
                float xDist = weapon.rb.transform.position.x - target.rb.transform.position.x;
                Vector3 objPos =
                    target.rb.transform.position
                    + 1.85f * xPos * (Vector3)target.right
                    + 0.375f * (Vector3)target.up;
                weapon.rb.MovePosition(Vector3.SmoothDamp(weapon.rb.transform.position, objPos, ref weapon.orbitChange, weapon.orbitChangeSmoothTime + parallaxFactor));
            }
        }
        public static void Update_UpLookAtDir(RuntimeSceneObject target, Vector3 lookDir, List<Weapon> weapons, float t)
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                Weapon weapon = weapons[i];
                float orbitIndex = i;
                float total = weapons.Count;
                float argument = (2f * Mathf.PI * orbitIndex) / total;
                float xPos = Mathf.Cos(argument + t);
                float yPos = Mathf.Sin(argument + t);
                float parallaxFactor = -0.005f * yPos;
                weapon.transform.up = lookDir;
                weapon.transform.localScale = Vector3.one - 0.125f * yPos * Vector3.one;
                weapon.spriteRenderer.color = new Color(1, 1, 1, 1) - new Color(1, 1, 1, 0) * yPos * 0.5f;
                weapon.spriteRenderer.sortingOrder = target.spriteRenderer.sortingOrder + Mathf.RoundToInt((weapon.transform.localScale.y >= 1).DefinedValue(-1, 1));
                float xDist = weapon.rb.transform.position.x - target.rb.transform.position.x;
                Vector3 objPos =
                    target.rb.transform.position
                    + 1.85f * xPos * (Vector3)target.right
                    + 0.375f * (Vector3)target.up;
                weapon.rb.MovePosition(Vector3.SmoothDamp(weapon.rb.transform.position, objPos, ref weapon.orbitChange, weapon.orbitChangeSmoothTime + parallaxFactor));
            }
        }
        //Vector2 Position(float orbitIndex, float totalCount)
        //{
        //    float argument = (2f * Mathf.PI * orbitIndex) / totalCount;
        //    Debug.Log(argument);
        //    return new Vector2(Mathf.Cos(argument + t), Mathf.Sign(argument + t));
        //}
        //public Vector3 Update_UpToSetRelativeDir(RuntimeSceneObject orbitObj, Vector2 dirToPoint, float orbitIndex, float totalCount)
        //{
        //    orbitObj.transform.up = dirToPoint.x*targetObj.right + dirToPoint.y*targetObj.up;
        //    Vector3 objPos = targetObj.rb.transform.position;
        //    Debug.Log(orbitIndex);
        //    Vector2 pos = Position(orbitIndex, totalCount);
        //    parallaxFactor = -0.01f * pos.y;
        //    orbitObj.transform.localScale = Vector3.one - 0.125f * pos.y * Vector3.one;
        //    orbitObj.spriteRenderer.color = new Color(1, 1, 1, 1) - new Color(1, 1, 1, 0) * pos.y * 0.5f;
        //    orbitObj.spriteRenderer.sortingOrder = targetObj.spriteRenderer.sortingOrder + Mathf.RoundToInt((orbitObj.transform.localScale.y >= 1).DefinedValue(-1, 1));
        //    float xDist = orbitObj.rb.transform.position.x - targetObj.rb.transform.position.x;
        //    float shift = 0;
        //    if (Mathf.Sign(InputManager.ins.L_Input.x) == Mathf.Sign(xDist))
        //    {
        //        shift = InputManager.ins.L_Input.x;
        //    }
        //    //Composition of this is pretty much all that matters
        //    return objPos + 1.75f * pos.x * (Vector3)targetObj.right + shift * (Vector3)targetObj.right + 0.35f * (Vector3)targetObj.up;
        //}
        //public Vector3 Update_UpLookAtDir(RuntimeSceneObject orbitObj, Vector2 dirToPoint, float orbitIndex, float totalCount)
        //{
        //    orbitObj.transform.up = dirToPoint;
        //    Vector3 objPos = targetObj.rb.transform.position;
        //    Vector2 pos = Position(orbitIndex, totalCount);
        //    parallaxFactor = -0.01f * pos.y;
        //    orbitObj.transform.localScale = Vector3.one - 0.125f * pos.y * Vector3.one;
        //    orbitObj.spriteRenderer.color = new Color(1, 1, 1, 1) - new Color(1, 1, 1, 0) * pos.y * 0.5f;
        //    orbitObj.spriteRenderer.sortingOrder = targetObj.spriteRenderer.sortingOrder + Mathf.RoundToInt((orbitObj.transform.localScale.y >= 1).DefinedValue(-1, 1));
        //    float xDist = orbitObj.rb.transform.position.x - targetObj.rb.transform.position.x;
        //    float shift = 0;
        //    if (Mathf.Sign(InputManager.ins.L_Input.x) == Mathf.Sign(xDist))
        //    {
        //        shift = InputManager.ins.L_Input.x;
        //    }
        //    //Composition of this is pretty much all that matters
        //    return objPos + 1.75f * pos.x * (Vector3)targetObj.right + shift * (Vector3)targetObj.right + 0.35f * (Vector3)targetObj.up;
        //}
    }
    public static class WeaponThrow
    {
        public static void Ready_Default(RuntimeSceneObject target, List<Weapon> weapons)
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                Weapon weapon = weapons[i];
                weapon.rb.MovePosition(target.rb.transform.position);
            }
        }
        public static void Throw_Default(RuntimeSceneObject target,Vector3 throwDir,List<Weapon> weapons)
        {
            int index = 0;
            float count = weapons.Count;
            while(weapons.Count > 0)
            {
                Weapon weapon = weapons[0];
                float accuracyMax = index;
                Vector2 throwPathDeviation =
                    Random.Range(-accuracyMax, accuracyMax) * Vector3.Dot(target.right, throwDir) * target.up
                    + Random.Range(-accuracyMax, accuracyMax) * Vector3.Dot(target.up, throwDir) * target.right;
                weapon.rb.velocity =
                    weapon.throwSpeedMax * (Vector2)throwDir
                    + 2f*(index / count) * throwPathDeviation;
                weapon.transform.up = weapon.rb.velocity;
                weapon.animator.Play("LAUNCH");
                weapons.RemoveAt(0);
                index++;
                if (index > 500)
                {
                    Debug.LogError("INFINITE WHILE LOOP");
                    break;
                }
            }
        }
        public static void Throw_SemiAuto(RuntimeSceneObject target, Vector3 throwDir, List<Weapon> weapons)
        {
            if (weapons.Count == 0)
                return;
            Weapon weapon = weapons[0];
            weapon.rb.velocity = weapon.throwSpeedMax * (Vector2)throwDir;
            weapon.transform.up = weapon.rb.velocity;
            weapon.animator.Play("LAUNCH");
            weapons.RemoveAt(0);
        }
    }
}