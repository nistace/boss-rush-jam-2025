using BossRushJam25.Health;
using UnityEngine;
using UnityEngine.Events;

namespace BossRushJam25.HexGrid {
   public class GridHexContent : MonoBehaviour {
      [SerializeField] protected GridHexContentType type;

      public GridHexContentType Type => type;
      private bool Initialized { get; set; }
      public HealthSystem HealthSystem { get; private set; }
      public static UnityEvent<GridHexContent, HealthSystem, int> OnAnyContentHealthChanged { get; } = new UnityEvent<GridHexContent, HealthSystem, int>();
      public UnityEvent<DamageType, int> OnDamageAbsorbed { get; } = new UnityEvent<DamageType, int>();

      public void Start() => Initialize();

      public void Initialize() {
         if (Initialized) return;
         HealthSystem = Type.NewHealthSystem;
         HealthSystem.OnHealthChanged.AddListener(HandleHealthChanged);
         Initialized = true;
      }

      private void OnDestroy() {
         HealthSystem.OnHealthChanged.RemoveListener(HandleHealthChanged);
      }

      private void HandleHealthChanged(int newValue, int damageDelta) => OnAnyContentHealthChanged.Invoke(this, HealthSystem, damageDelta);

      public void TryDamage(int damageDealt, DamageType damageType) {
         if (type.AbsorbedDamages.Contains(damageType)) {
            OnDamageAbsorbed.Invoke(damageType, damageDealt);
            return;
         }

         HealthSystem.Damage(damageDealt, damageType);
      }

      public bool IsDamageable(DamageTypes damageTypes) => HealthSystem.Vulnerabilities.Overlaps(damageTypes);
   }
}