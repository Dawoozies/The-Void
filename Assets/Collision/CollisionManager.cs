using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using OldData;
public class CollisionManager : MonoBehaviour
{
    //First some ground rules
    //AnimationClipData should always have name [name of this gameObject]_[name of animation clip]_CollisionData
    public string nameOfEntity;
    Dictionary<string, AnimationClipCollisionData> entityCollisionData = new Dictionary<string, AnimationClipCollisionData>();
    void Awake()
    {
        //We will need to load all the collision data assets here
        //We will sort all the assets by name of clip in a dictionary
        LoadCollisionData();
    }

    public void LoadCollisionData()
    {
        AnimationClipCollisionData[] allCollisionData = Resources.LoadAll<AnimationClipCollisionData>("AnimationClipCollisionData");
        if(nameOfEntity == null || nameOfEntity.Length <= 0)
        {
            Debug.LogError("CollisionManager Error: Name of entity null or not set");
            return;
        }

        if(allCollisionData != null  && allCollisionData.Length > 0)
        {
            foreach (AnimationClipCollisionData collisionData in allCollisionData)
            {
                if(collisionData.animationClip == null)
                {
                    Debug.LogError("AnimationClipCollisionData has null animationClip, delete " + collisionData.name);
                }

                if(collisionData.animationClip != null && collisionData.animationClip.name.Contains(nameOfEntity))
                {
                    //Debug.Log(collisionData.animationClip.name + " contains substring " + nameOfEntity);
                    entityCollisionData.Add(collisionData.animationClip.name, collisionData);
                }
            }
        }
    }

    public List<FrameCollisionData> GetHitboxData(string animationClipName)
    {
        if(entityCollisionData.ContainsKey(animationClipName) == false)
        {
            //Debug.Log("CollisionManager Warning: Trying to get Hitbox Data which does not exist " + animationClipName);
            return null;
        }

        List<FrameCollisionData> newHitboxData = new List<FrameCollisionData>();
        for (int i = 0; i < entityCollisionData[animationClipName].hitboxes.Count; i++)
        {
            FrameCollisionData newFrameData = new FrameCollisionData(i);

            for (int j = 0; j < entityCollisionData[animationClipName].hitboxes[i].circles.Count; j++)
            {
                Vector2 newCenter = entityCollisionData[animationClipName].hitboxes[i].circles[j].center;
                float newRadius = entityCollisionData[animationClipName].hitboxes[i].circles[j].radius;
                Circle newCircle = new Circle(newCenter, newRadius);

                newFrameData.circles.Add(newCircle);
            }

            newHitboxData.Add(newFrameData);
        }

        return newHitboxData;
    }

    public List<FrameCollisionData> GetHurtboxData(string animationClipName)
    {
        if(entityCollisionData.ContainsKey(animationClipName) == false)
        {
            Debug.LogError("CollisionManager Error: Trying to get Hurtbox Data which does not exist");
            return null;
        }

        return entityCollisionData[animationClipName].hurtboxes;
    }

    public List<FrameCollisionData> GetGroundboxData(string animationClipName)
    {
        if(entityCollisionData.ContainsKey(animationClipName) == false)
        {
            //Debug.LogError("CollisionManager Error: Trying to get Groundbox Data which does not exist");
            return null;
        }

        List<FrameCollisionData> newGroundboxData = new List<FrameCollisionData>();
        for (int i = 0; i < entityCollisionData[animationClipName].groundboxes.Count; i++)
        {
            FrameCollisionData newFrameData = new FrameCollisionData(i);

            for (int j = 0; j < entityCollisionData[animationClipName].groundboxes[i].circles.Count; j++)
            {
                Vector2 newCenter = entityCollisionData[animationClipName].groundboxes[i].circles[j].center;
                float newRadius = entityCollisionData[animationClipName].groundboxes[i].circles[j].radius;
                Circle newCircle = new Circle(newCenter, newRadius);

                newFrameData.circles.Add(newCircle);
            }

            newGroundboxData.Add(newFrameData);
        }

        return newGroundboxData;
    }
}
