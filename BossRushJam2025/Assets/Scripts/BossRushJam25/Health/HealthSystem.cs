using UnityEngine;
using UnityEngine.Events;

namespace BossRushJam25.Health {
   public class HealthSystem {
      public int Max { get; }
      public int Current { get; private set; }
      public float Ratio => (float)Current / Max;
      public DamageTypes Vulnerabilities { get; }

      /// <summary> Invoked when health changed. Notifies, in this order, the new Health value and the change</summary>
      public UnityEvent<int, int> OnHealthChanged { get; } = new UnityEvent<int, int>();

      public bool Empty => Current == 0;
      public bool Full => Current == Max;

      public HealthSystem(int max, DamageTypes vulnerabilities = DamageTypes.Everything) {
         Max = max;
         Current = max;
         Vulnerabilities = vulnerabilities;
      }

      public bool Damage(int amount, DamageType damageType) {
         if (!Vulnerabilities.Contains(damageType)) return false;
         return ChangeHealth(-amount);
      }

      public bool DamagePure(int amount) => ChangeHealth(-amount);

      public bool Heal(int amount, bool canRevive = false) {
         if (!canRevive && Current == 0) return false;
         return ChangeHealth(amount);
      }

      private bool ChangeHealth(int delta) {
         var actualChange = Mathf.Clamp(delta, -Current, Max - Current);
         if (actualChange == 0) return false;

         Current += actualChange;
         OnHealthChanged.Invoke(Current, actualChange);
         return true;
      }
   }
}