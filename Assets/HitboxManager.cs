using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxManager : MonoBehaviour
{
    CollisionManager collisionManager;
    Animator animator;

    AnimatorStateInfo currentStateInfo;
    AnimatorClipInfo currentClipInfo;

    List<FrameCollisionData> hitboxes = new List<FrameCollisionData>();

    public float normalizedTime;
    public float speed;

    public float clipTime;
    public int totalFrames;
    public int frame;

    Transform hitboxPool;
    int maxHitboxes = 20;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        collisionManager = GetComponent<CollisionManager>();

        if (currentClipInfo.clip == null)
        {
            currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            currentClipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];

            hitboxes = collisionManager.GetHitboxData(currentClipInfo.clip.name);
        }

        if (animator != null)
        {
            //Check for child object named HitboxPool
            if(animator.transform.childCount <= 0)
            {
                //Then no child objects exist yet so we know we going to have to create the object we want
                hitboxPool = new GameObject("Hitbox_Pool").transform;
                hitboxPool.parent = animator.transform;
            }
            else
            {
                bool hitboxPoolNotFound = true;

                for (int i = 0; i < animator.transform.childCount; i++)
                {
                    if(animator.transform.GetChild(i).name == "Hitbox_Pool")
                    {
                        hitboxPool = animator.transform.GetChild(i);
                        hitboxPoolNotFound = false;
                    }
                }

                //We are making two objects because of the new game object function
                if(hitboxPoolNotFound)
                {
                    hitboxPool = new GameObject("Hitbox_Pool").transform;
                    hitboxPool.parent = animator.transform;
                }
            }
        }

        if(hitboxPool != null)
        {
            //Create the pool
            for (int i = 0; i < maxHitboxes; i++)
            {
                Transform newHitbox = new GameObject("Hitbox" + i).transform;
                newHitbox.parent = hitboxPool;
                CircleCollider2D newCircleCollider = newHitbox.gameObject.AddComponent(typeof(CircleCollider2D)) as CircleCollider2D;
            }
        }
    }
    void Update()
    {
        if (currentClipInfo.clip != null)
        {
            if(currentClipInfo.clip.name != animator.GetCurrentAnimatorClipInfo(0)[0].clip.name)
            {
                currentClipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];

                hitboxes = collisionManager.GetHitboxData(currentClipInfo.clip.name);
            }
        }

        currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        currentClipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];

        speed = currentStateInfo.speed;
        normalizedTime = currentStateInfo.normalizedTime;

        totalFrames = Mathf.RoundToInt(currentClipInfo.clip.length);
        clipTime = normalizedTime - Mathf.FloorToInt(normalizedTime);
        frame = Mathf.FloorToInt(totalFrames * clipTime);

        SetHitboxCircles();
    }

    void SetHitboxCircles()
    {
        //From here we need to constantly be getting how many hitbox circles are in the current frame's frameData
        int maxHitboxCirclesNeeded = hitboxes[frame].circles.Count;
        for (int hitboxIndex = 0; hitboxIndex < hitboxPool.childCount; hitboxIndex++)
        {
            if(hitboxIndex < maxHitboxCirclesNeeded)
            {
                Vector2 hitboxCenter = hitboxes[frame].circles[hitboxIndex].center;
                float hitboxRadius = hitboxes[frame].circles[hitboxIndex].radius;

                hitboxPool.GetChild(hitboxIndex).gameObject.SetActive(true);
                hitboxPool.GetChild(hitboxIndex).localPosition = new Vector3(hitboxCenter.x, hitboxCenter.y, 0f);
                hitboxPool.GetChild(hitboxIndex).GetComponent<CircleCollider2D>().radius = hitboxRadius;
            }
            else
            {
                hitboxPool.GetChild(hitboxIndex).gameObject.SetActive(false);
            }
        }
    }

    public bool showHitboxGizmos = false;
    private void OnDrawGizmos()
    {
        if (hitboxPool == null)
            return;

        if(showHitboxGizmos)
        {
            for (int i = 0; i < hitboxPool.transform.childCount; i++)
            {
                if (hitboxPool.GetChild(i).gameObject.activeSelf)
                {
                    Vector3 position = hitboxPool.GetChild(i).position;
                    float radius = hitboxPool.GetChild(i).GetComponent<CircleCollider2D>().radius;
                    Gizmos.color = new Color(1f, 0, 1f, 0.65f);
                    Gizmos.DrawSphere(position, radius);
                }
            }
        }
    }
}
