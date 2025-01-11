using UnityEngine;

namespace BossRushJam25.HexGrid {
   [RequireComponent(typeof(Animator))]
   public class GridHexAnimator : MonoBehaviour {
      private static readonly int movingAnimParam = Animator.StringToHash("Moving");
      private GridHex GridHex { get; set; }
      private Animator Animator { get; set; }

      private void Start() {
         Animator = GetComponent<Animator>();
         GridHex = GetComponentInParent<GridHex>();

         if (GridHex) {
            Animator.SetBool(movingAnimParam, GridHex.IsMoving);
            GridHex.OnMovingChanged.AddListener(HandleMovingChanged);
         }
      }

      private void OnDestroy() {
         if (GridHex) {
            GridHex.OnMovingChanged.RemoveListener(HandleMovingChanged);
         }
      }

      private void HandleMovingChanged(bool moving) => Animator.SetBool(movingAnimParam, moving);
   }
}