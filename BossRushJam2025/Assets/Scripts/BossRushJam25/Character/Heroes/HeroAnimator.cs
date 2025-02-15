using UnityEngine;

namespace BossRushJam25.Character.Heroes {
   [RequireComponent(typeof(Animator))]
   public class HeroAnimator : MonoBehaviour {
      private static readonly int speedAnimParam = Animator.StringToHash("Speed");
      private static readonly int attackingAnimParam = Animator.StringToHash("Attacking");
      private static readonly int deadAnimParam = Animator.StringToHash("Dead");

      [SerializeField] protected Animator animator;
      [SerializeField] protected SpriteRenderer heroRenderer;
      [SerializeField] protected CharacterCore characterCore;

      public void StartAttack(bool targetIsOnLeft) {
         animator.SetBool(attackingAnimParam, true);
         heroRenderer.flipX = targetIsOnLeft;
      }

      public void StopAttack()
      {
         animator.SetBool(attackingAnimParam, false);
      }

      private void Reset() {
         animator = GetComponent<Animator>();
         heroRenderer = GetComponent<SpriteRenderer>();
         characterCore = GetComponentInParent<CharacterCore>();
      }

      private void Update() {
         if (characterCore.NavMeshAgent.velocity.x > float.Epsilon) heroRenderer.flipX = false;
         if (characterCore.NavMeshAgent.velocity.x < -float.Epsilon) heroRenderer.flipX = true;

         animator.SetFloat(speedAnimParam, characterCore.NavMeshAgent.velocity.magnitude / characterCore.NavMeshAgent.speed);
         animator.SetBool(deadAnimParam, characterCore.Health.Empty);
      }
   }
}