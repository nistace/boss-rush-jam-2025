using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BossRushJam25.Combat;
using BossRushJam25.Health;
using BossRushJam25.HexGrid;
using MoreMountains.FeedbacksForThirdParty;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace BossRushJam25.Character.Bosses.GoldFist {
   public class GoldFistIndexShotAttackPattern : BossAttackPattern, ILaserCaster {
      [SerializeField] protected float castDuration = 4;
      [SerializeField] protected GridHexContentType[] obstacles;
      [SerializeField] protected DamageInfo damageInfo = new DamageInfo(1, DamageType.Laser, .1f, 5);
      [SerializeField] protected SerializableDictionary<GridHexContentType, int> contentTypeCostOfShooting = new SerializableDictionary<GridHexContentType, int>();
      private Vector2Int coordinatesWhereShotIsBlocked;

      private Vector2Int Origin { get; set; }
      private HexCoordinates.EDirection Direction { get; set; }
      public bool IsShooting { get; private set; }
      public Vector2Int CoordinatesWhereShotIsBlocked => coordinatesWhereShotIsBlocked;

      protected override IEnumerator Play() {
         Animator.StartAttack(GoldFistAnimator.AttackAnimation.IndexShot);

         IsShooting = false;
         var target = CombatUtils.GetHeroCoordinates();
         Direction = EvaluateBestDirectionToShoot(target, out var originForDirection);
         Origin = originForDirection;

         transform.position = HexGridController.Instance.CoordinatesToWorldPosition(Origin);
         transform.forward = HexGridController.Instance.CoordinatesToWorldPosition(target) - HexGridController.Instance.CoordinatesToWorldPosition(Origin.Neighbour(Direction.Opposite()));

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
         feedbacksPlayer.DurationMultiplier = damageInfo.DamageDuration;
         feedbacksPlayer.GetFeedbackOfType<MMF_CinemachineImpulse>().m_ImpulseDefinition.TimeEnvelope.SustainTime = damageInfo.DamageDuration;
         feedbacksPlayer.PlayFeedbacks();

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
         AffectedHexesManager.HideAllAffectedHexes(this);

         foreach (var hexCoordinates in GetAffectedHexes()) {
            AffectedHexesManager.SetAffectedHex(this, hexCoordinates, true);
         }
      }

      public override HashSet<Vector2Int> GetAffectedHexes() => LaserUtils.Shoot(Origin, Direction, false, out coordinatesWhereShotIsBlocked);

      private HexCoordinates.EDirection EvaluateBestDirectionToShoot(Vector2Int targetCoordinates, out Vector2Int originForDirection) {
         var bestDirection = HexCoordinates.EDirection.Right;
         originForDirection = default;
         var bestDirectionScore = int.MinValue;
         foreach (var direction in ((HexCoordinates.EDirection[])Enum.GetValues(typeof(HexCoordinates.EDirection))).OrderBy(_ => Random.value)) {
            var directionScore = 0;
            var originForThisDirection = CombatUtils.GetHexOnBorder(targetCoordinates, direction.Opposite());
            var shotHexes = LaserUtils.Shoot(originForThisDirection, direction, false, out var blockingCoordinates);
            foreach (var shotHexCoordinates in shotHexes.Union(new[] { blockingCoordinates })) {
               directionScore++;
               if (HexGridController.Instance.TryGetHex(shotHexCoordinates, out var shotHex)) {
                  foreach (var shotHexContent in shotHex.HexContents) {
                     if (contentTypeCostOfShooting.TryGetValue(shotHexContent.Type, out var cost)) {
                        directionScore -= cost;
                     }
                  }
               }
            }

            if (directionScore > bestDirectionScore) {
               bestDirection = direction;
               bestDirectionScore = directionScore;
               originForDirection = originForThisDirection;
            }
         }
         return bestDirection;
      }
   }
}