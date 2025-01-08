using BossRushJam25.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;
namespace BossRushJam25.SpinStrategies {
   public class StepSnapSpinStrategy : MonoBehaviour, ISpinStrategy {
      [SerializeField] protected float snapDelay = .2f;
      [SerializeField] protected float snapAnimationDuration = .1f;

      private Camera Camera { get; set; }
      private bool IsInteracting { get; set; }
      private bool IsHoveringOverTile { get; set; }
      private Vector2Int InteractionHexCoordinates { get; set; }
      private float DelayBeforeNextSnap { get; set; }

      public void Initialize() {
         Camera = Camera.main;
         GameInputs.Controls.Player.Interact.performed += HandleInteractPerformed;
         GameInputs.Controls.Player.Interact.canceled += HandleInteractCancelled;
      }

      private void HandleInteractCancelled(InputAction.CallbackContext obj) {
         IsInteracting = false;

      }

      private void HandleInteractPerformed(InputAction.CallbackContext obj) {
         IsInteracting = true;
         DelayBeforeNextSnap = snapDelay;
      }

      public void Tick() {
         if (IsInteracting) {
            if (IsHoveringOverTile) {
               DelayBeforeNextSnap -= Time.deltaTime;
               if (DelayBeforeNextSnap < 0) {
                  HexGridController.Instance.RotateRingAround(InteractionHexCoordinates, snapAnimationDuration);
                  DelayBeforeNextSnap = snapDelay;
               }
            }
         }
         else {
            IsHoveringOverTile = Physics.Raycast(new Ray(Camera.ScreenToWorldPoint(GameInputs.Controls.Player.Aim.ReadValue<Vector2>()), Camera.transform.forward), out var hit);
            if (IsHoveringOverTile) {
               var newHexCoordinates = HexGridController.Instance.WorldToCoordinates(hit.point);
               if (newHexCoordinates != InteractionHexCoordinates) {
                  HexGridController.Instance.UnHighlightAllHexes();
                  InteractionHexCoordinates = newHexCoordinates;
                  HexGridController.Instance.SetHighlightedHexAt(InteractionHexCoordinates, true);
                  foreach (var neighbour in HexGridController.Instance.GetNeighbours(InteractionHexCoordinates)) {
                     neighbour.SetHighlighted(true);
                  }
               }
            }
            else {
               HexGridController.Instance.UnHighlightAllHexes();
            }
         }
      }
   }
}
