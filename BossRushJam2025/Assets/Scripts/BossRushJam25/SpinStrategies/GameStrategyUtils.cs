using BossRushJam25.HexGrid;
using BossRushJam25.Inputs;
using UnityEngine;

namespace BossRushJam25.SpinStrategies {
   public static class GameStrategyUtils {
      private static Camera CachedCamera { get; set; }
      private static Camera MainCamera => CachedCamera ? CachedCamera : CachedCamera = Camera.main;

      public static bool IsHoveringOverTile(out Vector2Int hoveredCoordinates) {
         hoveredCoordinates = default;
         if (Physics.Raycast(new Ray(MainCamera.ScreenToWorldPoint(GameInputs.Controls.Player.Aim.ReadValue<Vector2>()), MainCamera.transform.forward), out var hit)) {
            hoveredCoordinates = HexGridController.Instance.WorldToCoordinates(hit.point);
            return true;
         }
         return false;
      }
   }
}