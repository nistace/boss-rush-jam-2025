using System.Collections.Generic;
using System.Linq;
using BossRushJam25.HexGrid;
using BossRushJam25.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace BossRushJam25.SpinStrategies {
   public class ThreeClickSpinStrategy : MonoBehaviour, ISpinStrategy {
      [SerializeField] protected float spinDuration = .5f;
      [SerializeField] protected float delayBetweenSpins = .5f;

      private enum EStep {
         SelectOrigin = 0,
         SelectCenter = 1,
         SelectDestination = 2,
         Delay = 3
      }

      private EStep CurrentStep { get; set; } = EStep.SelectOrigin;
      private Vector2Int Origin { get; set; }
      private Vector2Int Center { get; set; }
      private Vector2Int HoveringCoordinates { get; set; }
      private IReadOnlyList<Vector2Int> RingCoordinates { get; set; }
      private bool HoveringOverHex { get; set; }

      public void Initialize() {
         GameInputs.Controls.Player.Interact.performed += HandleInteractPerformed;
      }

      private void HandleInteractPerformed(InputAction.CallbackContext obj) {
         if (!HoveringOverHex) return;
         if (CurrentStep == EStep.SelectOrigin) {
            Origin = HoveringCoordinates;
            CurrentStep = EStep.SelectCenter;
         }
         else if (CurrentStep == EStep.SelectCenter) {
            if (HoveringCoordinates == Origin) return;
            Center = HoveringCoordinates;
            CurrentStep = EStep.SelectDestination;
         }
         else if (CurrentStep == EStep.SelectDestination) {
            if (HoveringCoordinates == Origin) return;
            if (RingCoordinates == null || !RingCoordinates.Contains(HoveringCoordinates)) return;
            HexGridController.Instance.UnHighlightAllHexes();
            var steps = RingCoordinates.Count + RingCoordinates.IndexOf(HoveringCoordinates) - RingCoordinates.IndexOf(Origin);
            if (steps > RingCoordinates.Count / 2) steps -= RingCoordinates.Count;
            HexGridController.Instance.TranslateRingAround(Center, spinDuration, HexCoordinates.HexDistance(Origin, Center), steps, HandleTranslationDone);
            CurrentStep = EStep.Delay;
         }
      }

      private void HandleTranslationDone() {
         HoveringOverHex = false;
         CurrentStep = EStep.SelectOrigin;
      }

      public void Tick() {
         if (CurrentStep == EStep.Delay) {
            return;
         }

         var newHoveringOverHex = GameStrategyUtils.IsHoveringOverTile(out var newHoveringCoordinates);
         if (newHoveringCoordinates == HoveringCoordinates && newHoveringOverHex == HoveringOverHex) return;

         HoveringCoordinates = newHoveringCoordinates;
         HoveringOverHex = newHoveringOverHex;

         if (CurrentStep == EStep.SelectOrigin) {
            HexGridController.Instance.UnHighlightAllHexes();
            HexGridController.Instance.SetHighlightedHexAt(HoveringCoordinates, true);
         }
         else if (CurrentStep == EStep.SelectCenter) {
            HexGridController.Instance.UnHighlightAllHexes();
            HexGridController.Instance.SetHighlightedHexAt(HoveringCoordinates, true);
            RingCoordinates = HexGridController.GetRingClockwiseCoordinates(HoveringCoordinates, HexCoordinates.HexDistance(Origin, HoveringCoordinates));
            foreach (var ringCoordinate in RingCoordinates) {
               HexGridController.Instance.SetHighlightedHexAt(ringCoordinate, true);
            }
         }
         else if (CurrentStep == EStep.SelectDestination) {
            HexGridController.Instance.UnHighlightAllHexes();
            HexGridController.Instance.SetHighlightedHexAt(Center, true);
            foreach (var ringCoordinate in RingCoordinates) {
               HexGridController.Instance.SetHighlightedHexAt(ringCoordinate, true);
            }
         }
      }
   }
}