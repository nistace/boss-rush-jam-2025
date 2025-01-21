using BossRushJam25.Health;
using UnityEngine;
using UnityEngine.Events;

namespace BossRushJam25.HexGrid {
   public class GridHexContent : MonoBehaviour {
      [SerializeField] protected GridHexContentType type;

      public GridHexContentType Type => type;
      public HealthSystem HealthSystem { get; private set; }

      public static UnityEvent<GridHexContent, HealthSystem> OnAnyContentHealthChanged { get; } = new UnityEvent<GridHexContent, HealthSystem>();

      public void Start() {
         HealthSystem = Type.NewHealthSystem;
         HealthSystem.OnHealthChanged.AddListener(HandleHealthChanged);
      }

      private void OnDestroy() {
         HealthSystem.OnHealthChanged.RemoveListener(HandleHealthChanged);
      }

      private void HandleHealthChanged(int newValue, int damageDelta) => OnAnyContentHealthChanged.Invoke(this, HealthSystem);
      public void TryDamage(int damageDealt, DamageType damageType) => HealthSystem.Damage(damageDealt, damageType);
   }
}