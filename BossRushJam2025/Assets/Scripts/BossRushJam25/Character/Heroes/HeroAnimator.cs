using UnityEngine;

namespace BossRushJam25.Character.Heroes {
   [RequireComponent(typeof(Animator))]
   public class HeroAnimator : MonoBehaviour {
      private static readonly int speedAnimParam = Animator.StringToHash("Speed");

      [SerializeField] protected Animator animator;
      [SerializeField] protected SpriteRenderer heroRenderer;
      [SerializeField] protected CharacterCore characterCore;

      private void Reset() {
         animator = GetComponent<Animator>();
         heroRenderer = GetComponent<SpriteRenderer>();
         characterCore = GetComponentInParent<CharacterCore>();
      }

      private void Update() {
         if (characterCore.NavMeshAgent.velocity.x > float.Epsilon) heroRenderer.flipX = false;
         if (characterCore.NavMeshAgent.velocity.x < -float.Epsilon) heroRenderer.flipX = true;

         animator.SetFloat(speedAnimParam, characterCore.NavMeshAgent.velocity.magnitude / characterCore.NavMeshAgent.speed);
      }
   }
}
