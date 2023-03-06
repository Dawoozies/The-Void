using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods_Animator;
public class HurtboxManager : MonoBehaviour
{
    CollisionManager collisionManager;
    Animator animator;

    AnimatorClipInfo currentClipInfo;
    List<FrameCollisionData> hurtboxes = new List<FrameCollisionData>();

    int frame;
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        collisionManager = GetComponent<CollisionManager>();

        if (animator == null)
        {
            Debug.LogError("Hurtbox Manager Error: No Animator component found");
            return;
        }

        if (currentClipInfo.clip == null)
        {
            currentClipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];

            hurtboxes = collisionManager.GetHurtboxData(currentClipInfo.clip.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(currentClipInfo.clip != null)
        {
            if(currentClipInfo.clip.name != animator.GetCurrentAnimatorClipInfo(0)[0].clip.name)
            {
                currentClipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];

                hurtboxes = collisionManager.GetHurtboxData(currentClipInfo.clip.name);
            }
        }

        currentClipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];

        frame = animator.CurrentFrame();
    }

    public bool showHurtboxGizmos = false;
    private void OnDrawGizmos()
    {
        if (hurtboxes == null)
            return;

        if(showHurtboxGizmos)
        {
            if(hurtboxes.Count > 0)
            {
                for (int i = 0; i < hurtboxes[frame].circles.Count; i++)
                {
                    Gizmos.color = new Color(1, 0, 0, 0.65f);
                    Vector3 circlePos = transform.position + new Vector3(
                        hurtboxes[frame].circles[i].center.x * animator.transform.localScale.x,
                        hurtboxes[frame].circles[i].center.y,
                        0f);

                    Gizmos.DrawSphere(circlePos, hurtboxes[frame].circles[i].radius);
                }
            }
        }
    }
}
