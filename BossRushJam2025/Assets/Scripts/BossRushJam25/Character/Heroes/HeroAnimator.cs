using UnityEngine;

namespace BossRushJam25.Character.Heroes {
   [RequireComponent(typeof(Animator))]
   public class HeroAnimator : MonoBehaviour {
      private static readonly int speedAnimParam = Animator.StringToHash("Speed");

      [SerializeField] protected Animator animator;
      [SerializeField] protected MeshRenderer heroRenderer;
      [SerializeField] protected CharacterCore characterCore;

      private void Reset() {
         animator = GetComponent<Animator>();
         heroRenderer = GetComponent<MeshRenderer>();
         characterCore = GetComponentInParent<CharacterCore>();
      }

      private void Update() {
         if (characterCore.NavMeshAgent.velocity.x > float.Epsilon) heroRenderer.material.SetFloat("_FlipX", 0);
         if (characterCore.NavMeshAgent.velocity.x < -float.Epsilon) heroRenderer.material.SetFloat("_FlipX", 1);

         animator.SetFloat(speedAnimParam, characterCore.NavMeshAgent.velocity.magnitude / characterCore.NavMeshAgent.speed);
      }
   }
}
