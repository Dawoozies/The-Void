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
                if(animationEffect.clip.name.Contains(nameOfEntity))
                {
                    animationEffectData.Add(animationEffect.clip.name, animationEffect);
                }
            }
        }
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
    public void PlayEffect(Animator animator, AnimationClip clip, AnimatorStateInfo stateInfo)
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
            if (effectAnimators[i].TryPlayEffect(effectData, transform.position))
            {
                break;
            }
        }

        Debug.Log("Trying to play effects for clip: " + clip.name);
    }
}