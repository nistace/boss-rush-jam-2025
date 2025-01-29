using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BossRushJam25.HexGrid;
using BossRushJam25.HexGrid.Glyphs;
using BossRushJam25.PowerUps;
using UnityEngine;
using UnityEngine.Events;

namespace BossRushJam25.Character.Bosses.GoldFist {
   public class GoldFistSpawnTurretAndBatteryAttackPattern : BossAttackPattern {
      [SerializeField] protected GridHexContent batteryPrefab;
      [SerializeField] protected GridHexContent turretPrefab;
      [SerializeField] protected GridHexType[] spawnableHexTypes;

      public UnityEvent<GridHexContent> OnBatterySpawned { get; } = new UnityEvent<GridHexContent>();

      private GridHexContent CurrentExecutionBattery { get; set; }
      private GridHexContent CurrentExecutionTurret { get; set; }

      protected override IEnumerator Play() {
         var spawns = GetSpawnPositions();
         CurrentExecutionBattery = Instantiate(batteryPrefab, transform);
         CurrentExecutionTurret = Instantiate(turretPrefab, transform);

         spawns.batteryHex.SetLockedInPlaceBy(this, true);
         spawns.turretHex.SetLockedInPlaceBy(this, true);

         yield return null;

         // todo animate it

         spawns.batteryHex.AddContent(CurrentExecutionBattery, true);
         CurrentExecutionBattery.gameObject.SetActive(true);

         spawns.turretHex.AddContent(CurrentExecutionTurret, true);
         CurrentExecutionTurret.gameObject.SetActive(true);

         OnBatterySpawned?.Invoke(CurrentExecutionBattery);

         spawns.batteryHex.SetLockedInPlaceBy(this, false);
         spawns.turretHex.SetLockedInPlaceBy(this, false);
      }

      private (GridHex batteryHex, GridHex turretHex) GetSpawnPositions() {
         // todo improve algo to get two hexes
         var twoHexes = HexGridController.Instance.AllHexes
            .Where(t => t.HexContents.Count == 0
                        && t.Coordinates != BossAttackPatternUtils.GetHeroCoordinates()
                        && spawnableHexTypes.Contains(t.Type)
                        && !t.GetComponent<GlyphChunk>()
                        && !t.GetComponentInChildren<PowerUp>())
            .OrderBy(_ => Random.value)
            .Take(2)
            .ToArray();

         return (twoHexes[0], twoHexes[1]);
      }

      public override HashSet<Vector2Int> GetAffectedHexes() => new HashSet<Vector2Int>();
   }
}