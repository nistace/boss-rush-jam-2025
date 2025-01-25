using BossRushJam25.Character;
using UnityEngine;
using UnityEngine.Events;

namespace BossRushJam25.PowerUps {
   public class PowerUp : MonoBehaviour {
      [SerializeField] protected PowerUpType type;

      public PowerUpType Type => type;

      public static UnityEvent<PowerUp> OnAnyCollected { get; } = new UnityEvent<PowerUp>();

      public void Collect(CharacterCore collector) {
         if (type.HealAmount > 0) {
            collector.Health.Heal(type.HealAmount);
         }
         if (type.DamageUpAmount > 0) {
            collector.ChangeDamageInfo(collector.DamageInfo.WithIncreasedDamage(type.DamageUpAmount));
         }
         if (type.DamageSpeedUpAmount > float.Epsilon) {
            collector.ChangeDamageInfo(collector.DamageInfo.WithIncreasedSpeed(type.DamageSpeedUpAmount));
         }

         OnAnyCollected.Invoke(this);
         Destroy(gameObject);
      }
   }
}