using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameManagement
{
    public class HangedFrame : RuntimeSceneObject
    {
        public HangedFrame_Torso torsoObj;
        public HangedFrame_LeftArm leftArmObj;
        public HangedFrame_RightArm rightArmObj;
        public HangedFrame_Head headObj;
        public LineRenderer lineRenderer;
        public void ManagedStart()
        {
            torsoObj.transform.position = transform.position;
            rightArmObj.transform.position = transform.position + new Vector3(-2.53f, 2.9f, 0f);
            leftArmObj.transform.position = transform.position + new Vector3(2.68f, 2.57f, 0f);
            headObj.transform.position = transform.position + new Vector3(-1.75f,2.96f,0f);
        }
        public void ManagedUpdate(float tickDelta)
        {
            torsoObj.ManagedUpdate(tickDelta);
            leftArmObj.ManagedUpdate(tickDelta);
            rightArmObj.ManagedUpdate(tickDelta);
            headObj.ManagedUpdate(tickDelta);
        }
    }
    public class HangedFrame_Head : RuntimeSceneObject
    {
        public void ManagedStart()
        {
            spriteRenderer.sortingOrder = 3;
        }
        public void ManagedUpdate(float tickDelta)
        {
            AnimatorUpdate(tickDelta);
        }
    }
    public class HangedFrame_Torso : RuntimeSceneObject
    {
        public Component_CircleCollider2D circleCollider2DComponent;
        public Component_Overlap overlapComponent;
        const int colliderRequestAmount = 5;
        public void ManagedStart()
        {
            spriteRenderer.sortingOrder = 1;
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
            GameManagement.ins.circleCollider2DBank.RequestLoanForRigidbody2D(this, colliderRequestAmount);
        }
        public void ManagedUpdate(float tickDelta)
        {
            AnimatorUpdate(tickDelta);
        }
    }
    public class HangedFrame_LeftArm : RuntimeSceneObject
    {
        public Component_CircleCollider2D circleCollider2DComponent;
        public Component_Overlap overlapComponent;
        const int colliderRequestAmount = 12;
        public void ManagedStart()
        {
            spriteRenderer.sortingOrder = 4;
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
            GameManagement.ins.circleCollider2DBank.RequestLoanForRigidbody2D(this, colliderRequestAmount);
        }
        public void ManagedUpdate(float tickDelta)
        {
            AnimatorUpdate(tickDelta);
        }
    }
    public class HangedFrame_RightArm : RuntimeSceneObject
    {
        public Component_CircleCollider2D circleCollider2DComponent;
        public Component_Overlap overlapComponent;
        const int colliderRequestAmount = 12;
        public void ManagedStart()
        {
            spriteRenderer.sortingOrder = 4;
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
            GameManagement.ins.circleCollider2DBank.RequestLoanForRigidbody2D(this, colliderRequestAmount);
        }
        public void ManagedUpdate(float tickDelta)
        {
            AnimatorUpdate(tickDelta);
        }
    }
}