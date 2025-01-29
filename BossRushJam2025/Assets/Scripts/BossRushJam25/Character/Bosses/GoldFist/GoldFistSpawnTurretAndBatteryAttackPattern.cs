using System.Collections;
using System.Collections.Generic;
using BossRushJam25.HexGrid;
using UnityEngine;
using UnityEngine.Events;

namespace BossRushJam25.Character.Bosses.GoldFist {
   public class GoldFistSpawnTurretAndBatteryAttackPattern : BossAttackPattern {
      [SerializeField] protected GridHexContent batteryPrefab;
      [SerializeField] protected GridHexContent turretPrefab;

      public UnityEvent<GridHexContent> OnBatterySpawned { get; } = new UnityEvent<GridHexContent>();

      private GridHexContent CurrentExecutionBattery { get; set; }
      private GridHexContent CurrentExecutionTurret { get; set; }

      protected override IEnumerator Play() {
         while (!InterruptAsap) {
            yield return null;
         }

         CurrentExecutionBattery = Instantiate(batteryPrefab);
         CurrentExecutionBattery.gameObject.SetActive(false);
         CurrentExecutionTurret = Instantiate(turretPrefab);
         CurrentExecutionTurret.gameObject.SetActive(false);

         OnBatterySpawned?.Invoke(CurrentExecutionBattery);
      }

      public override HashSet<Vector2Int> GetAffectedHexes() => new HashSet<Vector2Int>();
   }
}