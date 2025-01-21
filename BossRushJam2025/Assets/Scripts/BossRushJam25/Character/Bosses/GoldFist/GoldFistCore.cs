using System.Linq;
using BossRushJam25.HexGrid;
using UnityEngine;

namespace BossRushJam25.Character.Bosses.GoldFist {
   [RequireComponent(typeof(BossCore))]
   public class GoldFistCore : MonoBehaviour {
      [SerializeField] protected BossCore core;
      [SerializeField] protected GridHexContentType batteryType;

      private void Reset() {
         core = GetComponent<BossCore>();
      }

      private void Start() {
         foreach (var battery in FindObjectsByType<GridHexContent>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Where(t => t.Type == batteryType).ToHashSet()) {
            battery.HealthSystem.OnHealthChanged.AddListener(HandleBatteryHealthChanged);
         }
      }

      private void HandleBatteryHealthChanged(int newHealth, int healthDelta) {
         if (healthDelta >= 0) return;
         core.Health.DamagePure(-healthDelta);
      }
   }
}