using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BossRushJam25.HexGrid;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BossRushJam25.Character.Bosses.GoldFist {
   public class GoldFistIndexShotAttackPattern : BossAttackPattern {
      [SerializeField] protected float castDuration = 4;
      [SerializeField] protected GridHexContentType[] obstacles;
      [SerializeField] protected float shotDuration = 4;
      [SerializeField] protected int damage = 1;
      [SerializeField] protected float damageTick = .1f;

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
         var lastDamageTime = -damageTick;
         for (var shotTime = 0f; !InterruptAsap && shotTime < shotDuration; shotTime += Time.deltaTime) {
            RefreshAreaOfEffect();
            if (shotTime - lastDamageTime > damageTick) {
               DealDamageOnAffectedHexes();
               lastDamageTime += damageTick;
            }
            yield return null;
         }
         IsShooting = false;
      }

      private void DealDamageOnAffectedHexes() {
         var hexCoordinates = Origin;
         while (HexGridController.Instance.TryGetHex(hexCoordinates, out var hex) && !hex.HexContents.Any(t => obstacles.Contains(t.Type))) {
            if (BossAttackPatternUtils.GetHeroCoordinates() == hexCoordinates) {
               BossAttackPatternUtils.DamageHero(damage);
            }

            hexCoordinates = hexCoordinates.Neighbour(Direction);
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