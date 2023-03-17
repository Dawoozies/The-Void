using UnityEngine;
namespace DamageSystem
{
    public static class DamageUtilities
    {
        //Computes how much time in seconds is left in the animation state
        //We only want another instance of damage being allowed if another animation state is causing damage
        public static float DamageRecoveryTime(HurtboxOverlap overlap)
        {
            return overlap.stateLength - (overlap.stateNormalizedTime * overlap.clip.length) / overlap.stateSpeed;
        }
    }

    public interface Damage<T>
    {
        public void ApplyDamage(params T[] list);
    }

    public class Test : Damage<int>
    {
        public void ApplyDamage(params int[] list)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface GeneralDamage
    {
        public void Apply();
    }

    public class DamageDealer : GeneralDamage
    {
        public void Apply()
        {
            throw new System.NotImplementedException();
        }
    }

    public class FrostDamage : Damage<HurtboxOverlap>
    {
        public Rigidbody2D rb;
        public void ApplyDamage(params HurtboxOverlap[] overlaps)
        {
            for (int i = 0; i < overlaps.Length; i++)
            {
                Debug.Log($"Applying Frost Damage to {overlaps[i].collider.name}");
            }
        }
    }
    public class FireDamage : Damage<Transform>
    {
        public void ApplyDamage(params Transform[] transforms)
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                Debug.Log($"Applying Fire Damage to {transforms[i].name}");
            }
        }
    }
    
    public interface Listener_Damage<T>
    {
        public void Update_Damage(params T[] damageList);
    }
}