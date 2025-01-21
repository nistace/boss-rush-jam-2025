using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BossRushJam25.Health;
using BossRushJam25.HexGrid;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BossRushJam25.Character.Bosses.GoldFist {
   public class GoldFistIndexShotAttackPattern : BossAttackPattern {
      [SerializeField] protected float castDuration = 4;
      [SerializeField] protected GridHexContentType[] obstacles;
      [SerializeField] protected DamageInfo damageInfo = new DamageInfo(1, DamageType.Laser, .1f, 5);

      private Vector2Int Origin { get; set; }
      private HexCoordinates.EDirection Direction { get; set; }
      public bool IsShooting { get; private set; }
      public Vector2Int ShotBlockingHexCoordinates { get; private set; }

      protected override IEnumerator Play() {
         IsShooting = false;
         var target = BossAttackPatternUtils.GetHeroCoordinates();
         Direction = (HexCoordinates.EDirection)Random.Range(0, Enum.GetValues(typeof(HexCoordinates.EDirection)).Length);
         Origin = BossAttackPatternUtils.GetHexOnBorder(target, Direction.Opposite());

         transform.position = HexGridController.Instance.CoordinatesToWorldPosition(Origin);
         transform.forward = HexGridController.Instance.CoordinatesToWorldPosition(target) - transform.position;

         for (var castTime = 0f; !InterruptAsap && castTime < castDuration; castTime += Time.deltaTime) {
            RefreshAreaOfEffect();
            yield return null;
         }

         if (InterruptAsap) yield break;

         IsShooting = true;
         for (var damageRunner = new DamageRunner(damageInfo); !damageRunner.Done(); damageRunner.Continue(Time.deltaTime)) {
            RefreshAreaOfEffect();
            DealDamageOnAffectedHexes(damageRunner.DamageDealtThisFrame);
            yield return null;
         }

         IsShooting = false;
      }

      private void DealDamageOnAffectedHexes(int damageDealt) {
         if (damageDealt <= 0) return;

         var hexCoordinates = Origin;
         GridHex hex;
         while (HexGridController.Instance.TryGetHex(hexCoordinates, out hex) && !hex.HexContents.Any(t => obstacles.Contains(t.Type))) {
            if (BossAttackPatternUtils.GetHeroCoordinates() == hexCoordinates) {
               BossAttackPatternUtils.DamageHero(damageInfo.DamageType, damageDealt);
            }
            hex.TryDamageContents(damageDealt, damageInfo.DamageType);

            hexCoordinates = hexCoordinates.Neighbour(Direction);
         }

         if (hex) {
            hex.TryDamageContents(damageDealt, damageInfo.DamageType);
         }
      }

      private void RefreshAreaOfEffect() {
         AffectedHexesManager.HideAllAffectedHexes();
         var hexCoordinates = Origin;
         while (HexGridController.Instance.TryGetHex(hexCoordinates, out var hex) && !hex.HexContents.Any(t => obstacles.Contains(t.Type))) {
            AffectedHexesManager.SetAffectedHex(hexCoordinates, true);
            hexCoordinates = hexCoordinates.Neighbour(Direction);
         }
         ShotBlockingHexCoordinates = hexCoordinates;
      }

      public override HashSet<Vector2Int> GetAffectedHexes()
      {
         HashSet<Vector2Int> affectedHexes = new();
         var hexCoordinates = Origin;

         while(HexGridController.Instance.TryGetHex(hexCoordinates, out var hex) && !hex.HexContents.Any(t => obstacles.Contains(t.Type)))
         {
            affectedHexes.Add(hexCoordinates);
            hexCoordinates = hexCoordinates.Neighbour(Direction);
         }

         return affectedHexes;
      }
   }
}