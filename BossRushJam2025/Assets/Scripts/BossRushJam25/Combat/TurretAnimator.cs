using UnityEngine;

namespace BossRushJam25.Combat {
   [RequireComponent(typeof(Animator))]
   public class TurretAnimator : MonoBehaviour {
      private static readonly int activeAnimParam = Animator.StringToHash("Active");
      private static readonly int shootingAnimParam = Animator.StringToHash("Shooting");

      [SerializeField] protected Animator animator;
      [SerializeField] protected Turret turret;

      private void Reset() {
         animator = GetComponent<Animator>();
         turret = GetComponentInParent<Turret>();
      }

      private void Update() {
         animator.SetBool(activeAnimParam, turret.Active);
         animator.SetBool(shootingAnimParam, turret.IsShooting);
      }
   }
}