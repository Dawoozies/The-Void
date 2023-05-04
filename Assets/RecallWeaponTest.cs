using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecallWeaponTest : StateMachineBehaviour
{
    public Vector3 recallLocalPosition;
    Rigidbody2D playerRigidbody;
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerRigidbody = animator.GetComponentInParent<Rigidbody2D>();
        if (playerRigidbody == null)
            return;
        Vector3 playerPos = playerRigidbody.transform.position;
        PlayerDataManager.ins.RecallPosition = new Vector3
            (
            playerPos.x + recallLocalPosition.x * animator.transform.localScale.x,
            playerPos.y + recallLocalPosition.y * animator.transform.localScale.y,
            playerPos.z + recallLocalPosition.z * animator.transform.localScale.z
            );
        PlayerDataManager.ins.Recalling = true;
    }
}
