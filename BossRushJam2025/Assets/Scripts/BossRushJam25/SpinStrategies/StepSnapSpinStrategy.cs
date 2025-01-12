using BossRushJam25.HexGrid;
using BossRushJam25.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BossRushJam25.SpinStrategies {
   public class StepSnapSpinStrategy : MonoBehaviour, ISpinStrategy {
      [SerializeField] protected float snapDelay = .2f;
      [SerializeField] protected int ringRadius = 2;
      [SerializeField] protected HexHighlightType highlightType;

      private bool IsInteracting { get; set; }
      private bool IsHoveringOverTile { get; set; }
      private Vector2Int InteractionHexCoordinates { get; set; }
      private float DelayBeforeNextSnap { get; set; }

      public void Enable() {
         GameInputs.Controls.Player.Interact.performed += HandleInteractPerformed;
         GameInputs.Controls.Player.Interact.canceled += HandleInteractCancelled;
      }

      public void Disable() {
         GameInputs.Controls.Player.Interact.performed -= HandleInteractPerformed;
         GameInputs.Controls.Player.Interact.canceled -= HandleInteractCancelled;
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
                  HexGridController.Instance.TranslateRingAround(InteractionHexCoordinates, ringRadius);
                  DelayBeforeNextSnap = snapDelay;
               }
            }
         }
         else {
            IsHoveringOverTile = GameStrategyUtils.IsHoveringOverTile(out var newHexCoordinates);
            if (IsHoveringOverTile) {
               if (newHexCoordinates != InteractionHexCoordinates) {
                  HexGridController.Instance.UnHighlightAllHexes();
                  InteractionHexCoordinates = newHexCoordinates;
                  HexGridController.Instance.SetHighlightedHexAt(InteractionHexCoordinates, highlightType);
                  foreach (var neighbour in HexGridController.Instance.GetNeighbours(InteractionHexCoordinates, ringRadius)) {
                     neighbour.SetHighlighted(highlightType);
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