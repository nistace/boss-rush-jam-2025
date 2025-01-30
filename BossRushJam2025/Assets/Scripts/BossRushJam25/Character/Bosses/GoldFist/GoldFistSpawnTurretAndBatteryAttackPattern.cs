using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BossRushJam25.Combat;
using BossRushJam25.HexGrid;
using BossRushJam25.HexGrid.Glyphs;
using BossRushJam25.PowerUps;
using UnityEngine;
using UnityEngine.Events;

namespace BossRushJam25.Character.Bosses.GoldFist {
   public class GoldFistSpawnTurretAndBatteryAttackPattern : BossAttackPattern {
      [SerializeField] protected float movementSpeed = 1;
      [SerializeField] protected GridHexContent batteryPrefab;
      [SerializeField] protected Turret turretPrefab;
      [SerializeField] protected GridHexType[] spawnableHexTypes;

      public UnityEvent<GridHexContent> OnBatterySpawned { get; } = new UnityEvent<GridHexContent>();

      private enum Phase {
         MovingToBatterySpawn = 0,
         BatteryVisible = 1,
         BatteryPlaced = 2,
         MovingToTurretSpawn = 3,
         TurretVisible = 4,
         TurretPlaced = 5
      }

      private GridHexContent CurrentExecutionBattery { get; set; }
      private Turret CurrentExecutionTurret { get; set; }
      private Phase CurrentPhase { get; set; }

      protected override IEnumerator Play() {
         CurrentPhase = Phase.MovingToBatterySpawn;
         Animator.StartAttack(GoldFistAnimator.AttackAnimation.PlaceBatteryAndTurret);

         var spawns = GetSpawnPositions();
         CurrentExecutionBattery = Instantiate(batteryPrefab, transform);
         CurrentExecutionBattery.gameObject.SetActive(false);
         CurrentExecutionBattery.Initialize();

         CurrentExecutionTurret = Instantiate(turretPrefab, transform);
         CurrentExecutionTurret.gameObject.SetActive(false);
         CurrentExecutionBattery.Initialize();

         spawns.batteryHex.SetLockedInPlaceBy(this, true);
         spawns.turretHex.SetLockedInPlaceBy(this, true);

         AffectedHexesManager.SetAffectedHex(spawns.batteryHex.Coordinates, true);
         AffectedHexesManager.SetAffectedHex(spawns.turretHex.Coordinates, true);

         transform.position = spawns.batteryHex.transform.position;
         transform.forward = transform.position - Animator.transform.position;

         Animator.AnchoredObject = CurrentExecutionBattery.transform;

         while (!InterruptAsap && !Animator.IsAtTarget) {
            yield return null;
         }

         if (!InterruptAsap) {
            Animator.NextPhase();
            Animator.OnKeyPointReached.AddListener(HandleBatteryAppeared);

            while (!InterruptAsap && CurrentPhase < Phase.BatteryVisible) {
               yield return null;
            }

            Animator.OnKeyPointReached.RemoveListener(HandleBatteryAppeared);
            CurrentExecutionBattery.gameObject.SetActive(true);
            Animator.OnKeyPointReached.AddListener(HandleBatteryPlaced);

            while (!InterruptAsap && CurrentPhase < Phase.BatteryPlaced) {
               yield return null;
            }

            Animator.OnKeyPointReached.RemoveListener(HandleBatteryPlaced);
         }

         Animator.AnchoredObject = default;
         spawns.batteryHex.AddContent(CurrentExecutionBattery, true);
         CurrentExecutionBattery.gameObject.SetActive(true);
         OnBatterySpawned?.Invoke(CurrentExecutionBattery);

         transform.position = spawns.turretHex.transform.position;
         transform.forward = transform.position - Animator.transform.position;

         Animator.AnchoredObject = CurrentExecutionTurret.transform;

         CurrentPhase = Phase.MovingToTurretSpawn;
         Animator.NextPhase();

         while (!InterruptAsap && !Animator.IsAtTarget) {
            yield return null;
         }

         if (!InterruptAsap) {
            Animator.NextPhase();
            Animator.OnKeyPointReached.AddListener(HandleTurretAppeared);

            while (!InterruptAsap && CurrentPhase < Phase.TurretVisible) {
               yield return null;
            }

            Animator.OnKeyPointReached.RemoveListener(HandleTurretAppeared);
            CurrentExecutionTurret.gameObject.SetActive(true);
            Animator.OnKeyPointReached.AddListener(HandleTurretPlaced);

            while (!InterruptAsap && CurrentPhase < Phase.TurretPlaced) {
               yield return null;
            }

            Animator.OnKeyPointReached.RemoveListener(HandleTurretPlaced);
         }

         Animator.AnchoredObject = default;
         spawns.turretHex.AddContent(CurrentExecutionTurret.HexContent, true);
         CurrentExecutionTurret.gameObject.SetActive(true);
         CurrentExecutionTurret.Active = true;

         spawns.batteryHex.SetLockedInPlaceBy(this, false);
         spawns.turretHex.SetLockedInPlaceBy(this, false);
         Animator.EndAttack();
      }

      private void HandleBatteryAppeared() => CurrentPhase = Phase.BatteryVisible;
      private void HandleBatteryPlaced() => CurrentPhase = Phase.BatteryPlaced;
      private void HandleTurretAppeared() => CurrentPhase = Phase.TurretVisible;
      private void HandleTurretPlaced() => CurrentPhase = Phase.TurretPlaced;

      private (GridHex batteryHex, GridHex turretHex) GetSpawnPositions() {
         // todo improve algo to get two hexes
         var twoHexes = HexGridController.Instance.AllHexes
            .Where(t => t.HexContents.Count == 0
                        && t.Coordinates != CombatUtils.GetHeroCoordinates()
                        && spawnableHexTypes.Contains(t.Type)
                        && !t.GetComponent<GlyphChunk>()
                        && !t.GetComponentInChildren<PowerUp>()
                        && HexCoordinates.HexDistance(t.Coordinates, Vector2Int.zero) < HexGridController.Instance.GridRadius - 3)
            .OrderBy(_ => Random.value)
            .Take(2)
            .ToArray();

         return (twoHexes[0], twoHexes[1]);
      }

      public override HashSet<Vector2Int> GetAffectedHexes() => new HashSet<Vector2Int>();
   }
}