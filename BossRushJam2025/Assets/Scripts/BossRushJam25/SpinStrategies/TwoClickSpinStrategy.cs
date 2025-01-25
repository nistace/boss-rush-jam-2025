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
      [SerializeField] protected float holdDurationToRotateOneHex = .3f;
      [SerializeField] protected float delayBetweenOneHexRotations = .5f;

      private enum EStep {
         SelectOrigin = 0,
         SelectDestination = 1,
         TranslatingRing = 2,
         HoldingToRotateSingleHex = 3,
         RotatingSingleHex = 4,
      }

      private EStep CurrentStep { get; set; } = EStep.SelectOrigin;
      private Vector2Int Origin { get; set; }
      private int RingRadius { get; set; }
      private Vector2Int Center { get; set; }
      private int Steps { get; set; }
      private bool CurrentCenterIsSolution { get; set; }
      private Vector2Int HoveringCoordinates { get; set; }
      private bool HoveringOverHex { get; set; }
      private Vector3 HitWorldPosition { get; set; }
      private bool IsInteracting { get; set; }
      private float CurrentInteractStartTime { get; set; }
      private float DelayBeforeNextSingleHexRotation { get; set; }

      public void Enable() {
         GameInputs.Controls.Player.Interact.performed += HandleInteractPerformed;
         GameInputs.Controls.Player.Interact.canceled += HandleInteractCanceled;
      }

      public void Disable() {
         GameInputs.Controls.Player.Interact.performed -= HandleInteractPerformed;
         GameInputs.Controls.Player.Interact.canceled -= HandleInteractCanceled;
      }

      private void HandleInteractCanceled(InputAction.CallbackContext obj) {
         if (!IsInteracting) return;
         IsInteracting = false;

         if (CurrentStep == EStep.HoldingToRotateSingleHex) {
            CurrentStep = EStep.SelectOrigin;
            if (HexGridController.Instance.TryGetHex(HoveringCoordinates, out var hex)) {
               hex.SetAsMoving(false);
            }
            HoveringOverHex = false;
            return;
         }

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
            CurrentStep = EStep.TranslatingRing;
         }
      }

      private void HandleInteractPerformed(InputAction.CallbackContext obj) {
         if (!HoveringOverHex) return;

         IsInteracting = true;
         CurrentInteractStartTime = Time.time;
         DelayBeforeNextSingleHexRotation = delayBetweenOneHexRotations;
      }

      private void HandleTranslationDone() {
         HoveringOverHex = false;
         CurrentStep = EStep.SelectOrigin;
      }

      private void HandleSingleRotationDone() {
         if (CurrentStep != EStep.RotatingSingleHex) return;
         if (IsInteracting) {
            DelayBeforeNextSingleHexRotation = delayBetweenOneHexRotations;
            CurrentStep = EStep.HoldingToRotateSingleHex;
         }
         else {
            CurrentStep = EStep.SelectOrigin;
            if (HexGridController.Instance.TryGetHex(HoveringCoordinates, out var hex)) {
               hex.SetAsMoving(false);
            }
            HoveringOverHex = false;
         }
      }

      public void Tick() {
         if (CurrentStep == EStep.TranslatingRing) {
            return;
         }

         if (CurrentStep == EStep.RotatingSingleHex) {
            return;
         }

         if (CurrentStep == EStep.HoldingToRotateSingleHex) {
            DelayBeforeNextSingleHexRotation -= Time.deltaTime;
            if (DelayBeforeNextSingleHexRotation < 0) {
               CurrentStep = EStep.RotatingSingleHex;
               HexGridController.Instance.RotateHex(HoveringCoordinates, 1, HandleSingleRotationDone);
            }
            return;
         }

         if (HoveringOverHex && IsInteracting && Time.time > CurrentInteractStartTime + holdDurationToRotateOneHex && HexGridController.Instance.TryGetHex(HoveringCoordinates, out var hex)) {
            CurrentStep = EStep.HoldingToRotateSingleHex;
            DelayBeforeNextSingleHexRotation = delayBetweenOneHexRotations;
            hex.SetAsMoving(true);
            return;
         }

         if (IsInteracting) return;

         var newHoveringOverHex = GameStrategyUtils.IsHoveringOverTile(out var newHoveringCoordinates, out var hitWorldPosition);

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
            HexGridController.Instance.UnHighlightAllHexes();
            HoveringCoordinates = newHoveringCoordinates;
            HoveringOverHex = newHoveringOverHex;
            HitWorldPosition = hitWorldPosition;

            if (HoveringOverHex) {
               CurrentCenterIsSolution = EvaluateBestRingToMoveHex(Origin, HoveringCoordinates, HitWorldPosition, out var center, out var ringRadius, out var steps, out var ring);
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

      private static bool EvaluateBestRingToMoveHex(Vector2Int origin,
         Vector2Int destination,
         Vector3 preferredWorldPosition,
         out Vector2Int center,
         out int ringRadius,
         out int steps,
         out IReadOnlyList<Vector2Int> ring) {
         var cubeOrigin = HexCoordinates.OffsetCoordinatesToCubeCoordinates(origin);
         var cubeHover = HexCoordinates.OffsetCoordinatesToCubeCoordinates(destination);

         // With radius = distance so that the path between the hexes is the shortest
         ringRadius = Mathf.Max(Mathf.Abs(cubeOrigin.x - cubeHover.x), Mathf.Abs(cubeOrigin.y - cubeHover.y), Mathf.Abs(cubeOrigin.z - cubeHover.z));
         if (EvaluateBestRingToMoveHex(origin, destination, preferredWorldPosition, ringRadius, out center, out steps, out ring)) {
            return true;
         }

         // With radius = half the distance so that the ring radius is the smallest possible
         ringRadius = Mathf.CeilToInt(ringRadius * .5f);
         if (EvaluateBestRingToMoveHex(origin, destination, preferredWorldPosition, ringRadius, out center, out steps, out ring)) {
            return true;
         }

         return false;
      }

      private static bool EvaluateBestRingToMoveHex(Vector2Int origin,
         Vector2Int destination,
         Vector3 preferredWorldPosition,
         int ringRadius,
         out Vector2Int center,
         out int steps,
         out IReadOnlyList<Vector2Int> ring) {
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
            .Where(t => t.ring.All(r => HexGridController.Instance.TryGetHex(r, out var hex) && !hex.LockedInPlace))
            .Select(t => (t.center, t.ring, t.direction, absoluteStepCount: (t.ring.IndexOf(destination) + t.ring.Count - t.ring.IndexOf(origin)) % t.ring.Count))
            .OrderBy(t => t.absoluteStepCount)
            .ThenBy(t => (HexGridController.Instance.CoordinatesToWorldPosition(t.center) - preferredWorldPosition).sqrMagnitude)
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