using System;
using System.Collections;
using System.Collections.Generic;
using BossRushJam25.Combat;
using BossRushJam25.Health;
using BossRushJam25.HexGrid;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BossRushJam25.Character.Bosses.GoldFist {
   public class GoldFistIndexShotAttackPattern : BossAttackPattern, ILaserCaster {
      [SerializeField] protected float castDuration = 4;
      [SerializeField] protected GridHexContentType[] obstacles;
      [SerializeField] protected DamageInfo damageInfo = new DamageInfo(1, DamageType.Laser, .1f, 5);
      private Vector2Int coordinatesWhereShotIsBlocked;

      private Vector2Int Origin { get; set; }
      private HexCoordinates.EDirection Direction { get; set; }
      public bool IsShooting { get; private set; }
      public Vector2Int CoordinatesWhereShotIsBlocked => coordinatesWhereShotIsBlocked;

      protected override IEnumerator Play() {
         Animator.StartAttack(GoldFistAnimator.AttackAnimation.IndexShot);

         IsShooting = false;
         var target = CombatUtils.GetHeroCoordinates();
         Direction = (HexCoordinates.EDirection)Random.Range(0, Enum.GetValues(typeof(HexCoordinates.EDirection)).Length);
         Origin = CombatUtils.GetHexOnBorder(target, Direction.Opposite());

         transform.position = HexGridController.Instance.CoordinatesToWorldPosition(Origin);
         transform.forward = HexGridController.Instance.CoordinatesToWorldPosition(target) - HexGridController.Instance.CoordinatesToWorldPosition(Origin.Neighbour(Direction));

         while (!InterruptAsap && !Animator.IsAtTarget) {
            yield return null;
         }

         Animator.NextPhase();

         for (var castTime = 0f; !InterruptAsap && castTime < castDuration; castTime += Time.deltaTime) {
            RefreshAreaOfEffect();
            yield return null;
         }

         if (InterruptAsap) yield break;

         IsShooting = true;
         Animator.NextPhase();

         for (var damageRunner = new DamageRunner(damageInfo); !damageRunner.Done(); damageRunner.Continue(Time.deltaTime)) {
            RefreshAreaOfEffect();
            DealDamageOnAffectedHexes(damageRunner.DamageDealtThisFrame);
            yield return null;
         }

         IsShooting = false;
         Animator.EndAttack();
      }

      private void DealDamageOnAffectedHexes(int damageDealt) {
         if (damageDealt <= 0) return;

         foreach (var affectedHex in GetAffectedHexes()) {
            if (CombatUtils.GetHeroCoordinates() == affectedHex) {
               CombatUtils.DamageHero(damageInfo.DamageType, damageDealt);
            }
            if (HexGridController.Instance.TryGetHex(affectedHex, out var hex)) {
               hex.TryDamageContents(damageDealt, damageInfo.DamageType);
            }
         }

         if (HexGridController.Instance.TryGetHex(coordinatesWhereShotIsBlocked, out var lastHex)) {
            lastHex.TryDamageContents(damageDealt, damageInfo.DamageType);
         }
      }

      private void RefreshAreaOfEffect() {
         AffectedHexesManager.HideAllAffectedHexes();

         foreach (var hexCoordinates in GetAffectedHexes()) {
            AffectedHexesManager.SetAffectedHex(hexCoordinates, true);
         }
      }

      public override HashSet<Vector2Int> GetAffectedHexes() => LaserUtils.Shoot(Origin, Direction, false, out coordinatesWhereShotIsBlocked);
   }
}