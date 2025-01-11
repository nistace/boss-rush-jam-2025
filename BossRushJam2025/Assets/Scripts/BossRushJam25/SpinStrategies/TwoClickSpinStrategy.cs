using System.Linq;
using BossRushJam25.HexGrid;
using BossRushJam25.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace BossRushJam25.SpinStrategies {
   public class TwoClickSpinStrategy : MonoBehaviour, ISpinStrategy {
      [SerializeField] protected float spinDuration = .5f;
      [SerializeField] protected HexHighlightType hoverHighlight;
      [SerializeField] protected HexHighlightType originHighlight;
      [SerializeField] protected HexHighlightType centerHighlight;
      [SerializeField] protected HexHighlightType ringHighlight;

      private enum EStep {
         SelectOrigin = 0,
         SelectDestination = 1,
         Rotation = 2
      }

      private EStep CurrentStep { get; set; } = EStep.SelectOrigin;
      private Vector2Int Origin { get; set; }
      private Vector2Int Center { get; set; }
      private int RingRadius { get; set; }
      private Vector2Int HoveringCoordinates { get; set; }
      private bool HoveringOverHex { get; set; }

      public void Initialize() {
         GameInputs.Controls.Player.Interact.performed += HandleInteractPerformed;
      }

      private void HandleInteractPerformed(InputAction.CallbackContext obj) {
         if (!HoveringOverHex) return;
         if (CurrentStep == EStep.SelectOrigin) {
            Origin = HoveringCoordinates;
            HexGridController.Instance.SetHighlightedHexAt(Origin, originHighlight);
            CurrentStep = EStep.SelectDestination;
         }
         else if (CurrentStep == EStep.SelectDestination) {
            if (HoveringCoordinates == Origin) return;
            var steps = HexCoordinates.HexDistance(Origin, HoveringCoordinates);
            HexGridController.Instance.TranslateRingAround(Center, spinDuration, RingRadius, steps, HandleTranslationDone);
            CurrentStep = EStep.Rotation;
         }
      }

      private void HandleTranslationDone() {
         HoveringOverHex = false;
         CurrentStep = EStep.SelectOrigin;
      }

      public void Tick() {
         if (CurrentStep == EStep.Rotation) {
            return;
         }

         var newHoveringOverHex = GameStrategyUtils.IsHoveringOverTile(out var newHoveringCoordinates);

         if (CurrentStep == EStep.SelectOrigin) {
            if (HoveringCoordinates == newHoveringCoordinates && HoveringOverHex == newHoveringOverHex) return;

            HoveringCoordinates = newHoveringCoordinates;
            HoveringOverHex = newHoveringOverHex;
            HexGridController.Instance.UnHighlightAllHexes();
            HexGridController.Instance.SetHighlightedHexAt(HoveringCoordinates, hoverHighlight);
         }
         else if (CurrentStep == EStep.SelectDestination) {
            if (HoveringCoordinates != newHoveringCoordinates || HoveringOverHex != newHoveringOverHex) {
               HexGridController.Instance.UnHighlightAllHexes();
               HoveringCoordinates = newHoveringCoordinates;
               HoveringOverHex = newHoveringOverHex;
               if (HoveringOverHex) {
                  var cubeOrigin = HexCoordinates.OffsetCoordinatesToCubeCoordinates(Origin);
                  var cubeHover = HexCoordinates.OffsetCoordinatesToCubeCoordinates(HoveringCoordinates);
                  RingRadius = Mathf.Max(Mathf.Abs(cubeOrigin.x - cubeHover.x), Mathf.Abs(cubeOrigin.y - cubeHover.y), Mathf.Abs(cubeOrigin.z - cubeHover.z));
                  var centersForOrigin = HexGridController.GetRingClockwiseCoordinates(Origin, RingRadius);
                  var centersForDestination = HexGridController.GetRingClockwiseCoordinates(HoveringCoordinates, RingRadius);

                  var candidateRingsPerCenter = centersForOrigin.Intersect(centersForDestination).ToDictionary(t => t, t => HexGridController.GetRingClockwiseCoordinates(t, RingRadius));
                  Center = candidateRingsPerCenter.OrderBy(t => (t.Value.IndexOf(HoveringCoordinates) + t.Value.Count - t.Value.IndexOf(Origin)) % t.Value.Count).First().Key;

                  foreach (var ringCoordinate in candidateRingsPerCenter[Center]) {
                     HexGridController.Instance.SetHighlightedHexAt(ringCoordinate, ringHighlight);
                  }
                  HexGridController.Instance.SetHighlightedHexAt(Origin, originHighlight);
               }
            }
         }
      }
   }
}