using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
public class AnimationEffects : MonoBehaviour
{
    public string nameOfEntity;
    Dictionary<string, AnimationEffect> animationEffectData = new Dictionary<string, AnimationEffect>();
    //Will need to get current animation clip to know what kind of effects we require
    public AnimatorController animatorController;
    Transform effectAnimatorPool;
    public int maxPoolSize = 20;
    List<AnimationEffectInterface> effectAnimators = new List<AnimationEffectInterface>();
    private void Awake()
    {
        LoadAnimationEffectData();
        GenerateAnimatorPool();
    }
    void LoadAnimationEffectData()
    {
        AnimationEffect[] data = Resources.LoadAll<AnimationEffect>("AnimationEffectData");
        if(nameOfEntity == null || nameOfEntity.Length <= 0)
        {
            Debug.LogError("AnimationEffect Error: Name of entity null or not set");
            return;
        }

        if(data != null && data.Length > 0)
        {
            foreach (AnimationEffect animationEffect in data)
            {
                //Ensure the AnimatorController is updated with all states
                if(animationEffect.effectsList.Count > 0)
                {
                    //Check through each frame seeing if there are non zero effects assigned
                    for (int frame = 0; frame < animationEffect.effectsList.Count; frame++)
                    {
                        if (animationEffect.effectsList[frame].effects != null && animationEffect.effectsList[frame].effects.Count > 0)
                        {
                            //Then there are effects here check that each effect has a state reflected in the controller
                            for (int effectIndex = 0; effectIndex < animationEffect.effectsList[frame].effects.Count; effectIndex++)
                            {
                                InitEffectInController(animationEffect.effectsList[frame].effects[effectIndex]);
                            }
                        }
                    }
                }


                if(animationEffect.clip.name.Contains(nameOfEntity))
                {
                    animationEffectData.Add(animationEffect.clip.name, animationEffect);
                }
            }
        }
    }
    void InitEffectInController (EffectData effectData)
    {
        AnimationClip[] controllerClips = animatorController.animationClips;
        foreach (AnimationClip controllerClip in controllerClips)
        {
            if(effectData.clip.name == controllerClip.name)
            {
                return;
            }
        }

        animatorController.AddMotion(effectData.clip);
    }
    void GenerateAnimatorPool()
    {
        effectAnimatorPool = new GameObject($"{nameOfEntity}_EffectAnimatorPool").transform;

        for (int i = 0; i < maxPoolSize; i++)
        {
            Transform newTransform = new GameObject("EffectAnimator_" + i).transform;
            newTransform.parent = effectAnimatorPool;
            SpriteRenderer newSpriteRenderer = newTransform.gameObject.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            newSpriteRenderer.sortingOrder = 100;
            Animator newAnimator = newTransform.gameObject.AddComponent(typeof(Animator)) as Animator;
            newAnimator.runtimeAnimatorController = animatorController;
            EffectAnimator effectAnimator = newTransform.gameObject.AddComponent(typeof(EffectAnimator)) as EffectAnimator;
            effectAnimator.InitialiseEffectAnimator();

            effectAnimators.Add(effectAnimator);
        }
    }
    public bool PlayEffect(AnimationClip clip, AnimatorStateInfo stateInfo, Vector3 entityLocalScale, bool copyEntityDirection)
    {
        if(!animationEffectData.ContainsKey(clip.name))
        {
            Debug.LogError($"Trying to play effect for {clip.name} but no effect data exists for this clip.");
            return false;
        }

        float normalizedTime = stateInfo.normalizedTime;
        int totalFrames = Mathf.RoundToInt(clip.length);
        float clipTime = normalizedTime - Mathf.FloorToInt(normalizedTime);
        int frame = Mathf.FloorToInt(totalFrames * clipTime);

        if (animationEffectData[clip.name].effectsList == null || animationEffectData[clip.name].effectsList.Count <= 0)
            return false;

        if (animationEffectData[clip.name].effectsList[frame].effects == null || animationEffectData[clip.name].effectsList[frame].effects.Count <= 0)
            return false;

        int randomEffectIndex = Random.Range(0, animationEffectData[clip.name].effectsList[frame].effects.Count);
        EffectData effectData = animationEffectData[clip.name].effectsList[frame].effects[randomEffectIndex];

        for (int i = 0; i < effectAnimators.Count; i++)
        {
            if (effectAnimators[i].TryPlayEffect(effectData, transform.position, entityLocalScale, copyEntityDirection))
            {
                return true;
            }
        }

        return false;
    }

    public void PlayEffectAtPosition(AnimationClip clip, AnimatorStateInfo stateInfo, Vector3 worldPositionToPlayAt, Vector3 entityLocalScale, bool copyEntityDirection)
    {
        if(!animationEffectData.ContainsKey(clip.name))
        {
            Debug.LogError($"Trying to play effect for {clip.name} but no effect data exists for this clip.");
            return;
        }

        float normalizedTime = stateInfo.normalizedTime;
        int totalFrames = Mathf.RoundToInt(clip.length);
        float clipTime = normalizedTime - Mathf.FloorToInt(normalizedTime);
        int frame = Mathf.FloorToInt(totalFrames * clipTime);

        int randomEffectIndex = Random.Range(0, animationEffectData[clip.name].effectsList[frame].effects.Count);
        EffectData effectData = animationEffectData[clip.name].effectsList[frame].effects[randomEffectIndex];

        for (int i = 0; i < effectAnimators.Count; i++)
        {
            if (effectAnimators[i].TryPlayEffectAtPosition(effectData, worldPositionToPlayAt, entityLocalScale, copyEntityDirection))
            {
                break;
            }
        }

        Debug.Log($"Trying to play effect at position {worldPositionToPlayAt} for clip: {clip.name}");
    }
}