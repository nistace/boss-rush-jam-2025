using System.Collections.Generic;
using System.Linq;
using BossRushJam25.HexGrid;
using BossRushJam25.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace BossRushJam25.SpinStrategies {
   public class TwoClickSpinStrategy : MonoBehaviour, ISpinStrategy {
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
      private int RingRadius { get; set; }
      private Vector2Int Center { get; set; }
      private int Steps { get; set; }
      private bool CurrentCenterIsSolution { get; set; }
      private Vector2Int HoveringCoordinates { get; set; }
      private bool HoveringOverHex { get; set; }

      public void Enable() {
         GameInputs.Controls.Player.Interact.performed += HandleInteractPerformed;
      }

      public void Disable() {
         GameInputs.Controls.Player.Interact.performed += HandleInteractPerformed;
      }

      private void HandleInteractPerformed(InputAction.CallbackContext obj) {
         if (!HoveringOverHex) return;
         if (CurrentStep == EStep.SelectOrigin) {
            if (!HexGridController.Instance.IsCellInGrid(HoveringCoordinates)) return;
            Origin = HoveringCoordinates;
            HexGridController.Instance.SetHighlightedHexAt(Origin, originHighlight);
            CurrentStep = EStep.SelectDestination;
         }
         else if (CurrentStep == EStep.SelectDestination) {
            if (HoveringCoordinates == Origin) return;
            if (!CurrentCenterIsSolution) return;

            HexGridController.Instance.SetHighlightedHexAt(HoveringCoordinates, ringHighlight);
            HexGridController.Instance.TranslateRingAround(Center, RingRadius, Steps, HandleTranslationDone);
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
            if (HoveringOverHex) {
               HexGridController.Instance.SetHighlightedHexAt(HoveringCoordinates, hoverHighlight);
            }
         }
         else if (CurrentStep == EStep.SelectDestination) {
            if (HoveringCoordinates != newHoveringCoordinates || HoveringOverHex != newHoveringOverHex) {
               HexGridController.Instance.UnHighlightAllHexes();
               HoveringCoordinates = newHoveringCoordinates;
               HoveringOverHex = newHoveringOverHex;

               if (HoveringOverHex) {
                  CurrentCenterIsSolution = EvaluateBestRingToMoveHex(Origin, HoveringCoordinates, out var center, out var ringRadius, out var steps, out var ring);
                  if (CurrentCenterIsSolution) {
                     Center = center;
                     RingRadius = ringRadius;
                     Steps = steps;

                     foreach (var ringCoordinate in ring) {
                        HexGridController.Instance.SetHighlightedHexAt(ringCoordinate, ringHighlight);
                     }
                  }
               }

               HexGridController.Instance.SetHighlightedHexAt(Origin, originHighlight);

               if (HoveringOverHex) {
                  HexGridController.Instance.SetHighlightedHexAt(HoveringCoordinates, hoverHighlight);
               }
            }
         }
      }

      public static bool EvaluateBestRingToMoveHex(Vector2Int origin, Vector2Int destination, out Vector2Int center, out int ringRadius, out int steps, out IReadOnlyList<Vector2Int> ring) {
         var cubeOrigin = HexCoordinates.OffsetCoordinatesToCubeCoordinates(origin);
         var cubeHover = HexCoordinates.OffsetCoordinatesToCubeCoordinates(destination);

         // With radius = distance so that the path between the hexes is the shortest
         ringRadius = Mathf.Max(Mathf.Abs(cubeOrigin.x - cubeHover.x), Mathf.Abs(cubeOrigin.y - cubeHover.y), Mathf.Abs(cubeOrigin.z - cubeHover.z));
         if (EvaluateBestRingToMoveHex(origin, destination, ringRadius, out center, out steps, out ring)) {
            return true;
         }

         // With radius = half the distance so that the ring radius is the smallest possible
         ringRadius = Mathf.CeilToInt(ringRadius * .5f);
         if (EvaluateBestRingToMoveHex(origin, destination, ringRadius, out center, out steps, out ring)) {
            return true;
         }

         return false;
      }

      public static bool EvaluateBestRingToMoveHex(Vector2Int origin, Vector2Int destination, int ringRadius, out Vector2Int center, out int steps, out IReadOnlyList<Vector2Int> ring) {
         center = default;
         steps = default;
         ring = default;

         var centersForOrigin = HexGridController.GetRingClockwiseCoordinates(origin, ringRadius);
         var centersForDestination = HexGridController.GetRingClockwiseCoordinates(destination, ringRadius);

         var candidateRingsPerCenter = centersForOrigin.Intersect(centersForDestination)
            .SelectMany(t => new[] {
               (center: t, ring: HexGridController.GetRingClockwiseCoordinates(t, ringRadius), direction: 1),
               (center: t, ring: HexGridController.GetRingAntiClockwiseCoordinates(t, ringRadius), direction: -1),
            })
            .Where(t => t.ring.All(r => HexGridController.Instance.IsCellInGrid(r)))
            .Select(t => (t.center, t.ring, t.direction, absoluteStepCount: (t.ring.IndexOf(destination) + t.ring.Count - t.ring.IndexOf(origin)) % t.ring.Count))
            .OrderBy(t => t.absoluteStepCount)
            .ThenByDescending(t => t.direction)
            .ToArray();

         if (candidateRingsPerCenter.Length > 0) {
            var solution = candidateRingsPerCenter[0];
            center = solution.center;
            steps = solution.direction * solution.absoluteStepCount;
            ring = solution.ring;

            return true;
         }

         return false;
      }
   }
}