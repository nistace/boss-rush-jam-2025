using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace BossRushJam25.Character.Bosses {
   public class BossPatternManager : MonoBehaviour {
      private HashSet<BossAttackPattern> AttackPatterns { get; } = new HashSet<BossAttackPattern>();

      public BossAttackPattern CurrentAttack { get; set; }
      private UnityAction CurrentAttackCallback { get; set; }
      public bool IsExecutingAttack => CurrentAttack;

      public UnityEvent OnAttackStarted { get; } = new();

      private void Start() {
         AttackPatterns.Clear();
         foreach (var attackPattern in GetComponentsInChildren<BossAttackPattern>()) {
            AttackPatterns.Add(attackPattern);
         }
      }

      public void ExecuteNextAttack(UnityAction callback) {
         CurrentAttackCallback = callback;
         CurrentAttack = AttackPatterns.OrderBy(_ => Random.value).First();
         CurrentAttack.OnExecuted.AddListener(HandleAttackExecuted);
         CurrentAttack.Execute();
         OnAttackStarted.Invoke();
      }

      private void HandleAttackExecuted() {
         CurrentAttackCallback?.Invoke();
         ClearCurrentAttack();
      }

      private void ClearCurrentAttack() {
         if (!CurrentAttack) return;
         CurrentAttack.OnExecuted.RemoveListener(HandleAttackExecuted);
         CurrentAttack = default;
      }
   }
}