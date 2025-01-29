using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BossRushJam25.Health;
using BossRushJam25.HexGrid;
using UnityEngine;

namespace BossRushJam25.Character.Bosses.GoldFist {
   public class GoldFistStompAttackPattern : BossAttackPattern {
      [SerializeField] protected float followSpeed = 1;
      [SerializeField] protected float castDuration = 4;
      [SerializeField] protected int damage = 8;
      [SerializeField] protected float delayAfterDamageDealt = .5f;
      [SerializeField] protected int ringRadius = 1;
      [SerializeField] protected GridHexContent fistHexContentPrefab;

      private static readonly int hitAnimParam = Animator.StringToHash("Hit");
      private static readonly int raiseAnimParam = Animator.StringToHash("Raise");

      private bool HitDone { get; set; }
      private bool FistRaised { get; set; }

      public void HitAnimEvent() => HitDone = true;
      public void RaisedAnimEvent() => FistRaised = true;
      private HashSet<GridHexContent> FistAsHexContents { get; } = new HashSet<GridHexContent>();

      protected override IEnumerator Play() {
         HitDone = false;
         FistRaised = false;
         transform.position = HexGridController.Instance.CoordinatesToWorldPosition(BossAttackPatternUtils.GetHeroCoordinates());

         for (var castTime = 0f; !InterruptAsap && castTime < castDuration; castTime += Time.deltaTime) {
            transform.position = Vector3.MoveTowards(transform.position,
               HexGridController.Instance.CoordinatesToWorldPosition(BossAttackPatternUtils.GetHeroCoordinates()),
               followSpeed * Time.deltaTime);

            RefreshAreaOfEffect();
            yield return null;
         }

         if (InterruptAsap) yield break;

         animator.SetTrigger(hitAnimParam);

         while (!InterruptAsap && !HitDone) {
            RefreshAreaOfEffect();
            yield return null;
         }

         if (InterruptAsap) yield break;

         AddFistAsHexContents();
         FistRaised = false;
         DealDamageOnAffectedHexes();
         AffectedHexesManager.HideAllAffectedHexes();

         for (var timeAfterHitDone = 0f; !InterruptAsap && timeAfterHitDone < delayAfterDamageDealt; timeAfterHitDone += Time.deltaTime) {
            yield return null;
         }

         if (InterruptAsap) {
            RemoveFistAsHexContents();
            yield break;
         }

         animator.SetTrigger(raiseAnimParam);

         while (!InterruptAsap && !FistRaised) {
            yield return null;
         }

         RemoveFistAsHexContents();
      }

      private void RemoveFistAsHexContents() {
         foreach (var fistAsHexContent in FistAsHexContents) {
            fistAsHexContent.gameObject.SetActive(false);
            var hex = fistAsHexContent.GetComponentInParent<GridHex>();
            if (hex) {
               hex.RemoveContent(fistAsHexContent);
            }
            fistAsHexContent.transform.SetParent(transform);
         }
      }

      private void AddFistAsHexContents() {
         var hexes = EvaluateTargetedHexes().Select(t => HexGridController.Instance.TryGetHex(t, out var hex) ? hex : null).Where(t => t).ToArray();

         while (FistAsHexContents.Count < hexes.Length) {
            FistAsHexContents.Add(Instantiate(fistHexContentPrefab, Vector3.zero, Quaternion.identity, transform));
         }

         var index = 0;
         foreach (var fistAsHexContent in FistAsHexContents) {
            fistAsHexContent.gameObject.SetActive(index < hexes.Length);
            if (index < hexes.Length) {
               hexes[index].AddContent(fistAsHexContent, true);
            }
            index++;
         }
      }

      private void DealDamageOnAffectedHexes() {
         if (EvaluateTargetedHexes().Contains(BossAttackPatternUtils.GetHeroCoordinates())) {
            BossAttackPatternUtils.DamageHero(DamageType.Physical, damage);
         }
      }

      private void RefreshAreaOfEffect() {
         AffectedHexesManager.HideAllAffectedHexes();
         foreach (var hex in EvaluateTargetedHexes()) {
            AffectedHexesManager.SetAffectedHex(hex, true);
         }
      }

      private HashSet<Vector2Int> EvaluateTargetedHexes() {
         var result = new HashSet<Vector2Int>();
         var centerCoordinates = HexGridController.Instance.WorldToCoordinates(transform.position);
         result.Add(centerCoordinates);
         for (var i = 1; i <= ringRadius; ++i) {
            foreach (var hex in HexGridController.GetRingClockwiseCoordinates(centerCoordinates, i)) {
               result.Add(hex);
            }
         }
         return result;
      }

      public override HashSet<Vector2Int> GetAffectedHexes() {
         return EvaluateTargetedHexes();
      }
   }
}