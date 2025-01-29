using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BossRushJam25.Character.Bosses {
   public class BossPatternManager : MonoBehaviour {
      private HashSet<BossAttackPattern> AttackPatterns { get; } = new HashSet<BossAttackPattern>();

      public BossAttackPattern CurrentAttack { get; private set; }
      public bool IsExecutingAttack => CurrentAttack;

      private void Start() {
         AttackPatterns.Clear();
         foreach (var attackPattern in GetComponentsInChildren<BossAttackPattern>()) {
            AttackPatterns.Add(attackPattern);
         }
      }

      public void ExecuteNextAttack() => ExecuteAttack(AttackPatterns.OrderBy(_ => Random.value).First());

      public void ExecuteAttack(BossAttackPattern attack) {
         ClearCurrentAttack();

         CurrentAttack = attack;
         CurrentAttack.OnExecuted.AddListener(HandleAttackExecuted);
         CurrentAttack.OnInterrupted.AddListener(HandleAttackInterrupted);
         CurrentAttack.Execute();
      }

      private void HandleAttackExecuted() => ClearCurrentAttack();
      private void HandleAttackInterrupted() => ClearCurrentAttack();

      private void ClearCurrentAttack() {
         if (!CurrentAttack) return;

         CurrentAttack.OnExecuted.RemoveListener(HandleAttackExecuted);
         CurrentAttack.OnInterrupted.RemoveListener(HandleAttackInterrupted);
         CurrentAttack = default;
      }

      public bool HasPatterns() {
         return AttackPatterns.Count > 0;
      }
   }
}