using BossRushJam25.HexGrid;
using BossRushJam25.Inputs;
using UnityEngine;

namespace BossRushJam25.SpinStrategies {
   public static class GameStrategyUtils {
      private static Camera CachedCamera { get; set; }
      private static Camera MainCamera => CachedCamera ? CachedCamera : CachedCamera = Camera.main;

      private static LayerMask ClickLayerMask => 1 << LayerMask.NameToLayer("InputClickLayer");

      public static bool IsHoveringOverTile(out Vector2Int hoveredCoordinates) {
         hoveredCoordinates = default;
         if (Physics.Raycast(MainCamera.ScreenPointToRay(GameInputs.Controls.Player.Aim.ReadValue<Vector2>()), out var hit, Mathf.Infinity, ClickLayerMask)) {
            hoveredCoordinates = HexGridController.Instance.WorldToCoordinates(hit.point);
            return true;
         }
         return false;
      }
   }
}