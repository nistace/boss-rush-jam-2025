using UnityEngine;
using UnityEngine.Events;

namespace BossRushJam25.Health {
   public class HealthSystem {
      public int MaxHealth { get; }
      public int Health { get; private set; }
      public float HealthRatio => (float)Health / MaxHealth;
      public DamageTypes Vulnerabilities { get; }

      /// <summary> Invoked when health changed. Notifies, in this order, the new Health value and the change</summary>
      public UnityEvent<int, int> OnHealthChanged { get; } = new UnityEvent<int, int>();

      public bool Empty => Health == 0;
      public bool Full => Health == MaxHealth;

      public HealthSystem(int maxHealth, DamageTypes vulnerabilities = DamageTypes.Everything) {
         MaxHealth = maxHealth;
         Health = maxHealth;
         Vulnerabilities = vulnerabilities;
      }

      public bool Damage(int amount, DamageType damageType) {
         if (!Vulnerabilities.Contains(damageType)) return false;
         return ChangeHealth(-amount);
      }

      public bool DamagePure(int amount) => ChangeHealth(-amount);

      public bool Heal(int amount, bool canRevive = false) {
         if (!canRevive && Health == 0) return false;
         return ChangeHealth(amount);
      }

      private bool ChangeHealth(int delta) {
         var actualChange = Mathf.Clamp(delta, -Health, MaxHealth - Health);
         if (actualChange == 0) return false;

         Health += actualChange;
         OnHealthChanged.Invoke(Health, actualChange);
         return true;
      }
   }
}