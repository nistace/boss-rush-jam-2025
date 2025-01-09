using System;
using UnityEngine;

namespace BossRushJam25.HexGrid {
   public static class HexCoordinates {
      public enum EDirection {
         Left = 0,
         UpLeft = 1,
         UpRight = 2,
         Right = 3,
         DownRight = 4,
         DownLeft = 5,
      }

      public static EDirection RotateClockwise(this EDirection direction) => direction switch {
         EDirection.Left => EDirection.UpLeft,
         EDirection.UpLeft => EDirection.UpRight,
         EDirection.UpRight => EDirection.Right,
         EDirection.Right => EDirection.DownRight,
         EDirection.DownRight => EDirection.DownLeft,
         EDirection.DownLeft => EDirection.Left,
         _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
      };

      public static EDirection RotateAntiClockwise(this EDirection direction) => direction switch {
         EDirection.Left => EDirection.DownLeft,
         EDirection.UpLeft => EDirection.Left,
         EDirection.UpRight => EDirection.UpLeft,
         EDirection.Right => EDirection.UpRight,
         EDirection.DownRight => EDirection.Right,
         EDirection.DownLeft => EDirection.DownRight,
         _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
      };

      public static Vector2Int Neighbour(this Vector2Int coordinates, EDirection direction, int steps = 1) => direction switch {
         EDirection.Left => coordinates.Left(steps),
         EDirection.UpLeft => coordinates.UpLeft(steps),
         EDirection.UpRight => coordinates.UpRight(steps),
         EDirection.Right => coordinates.Right(steps),
         EDirection.DownRight => coordinates.DownRight(steps),
         EDirection.DownLeft => coordinates.DownLeft(steps),
         _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
      };

      public static Vector2Int Left(this Vector2Int coordinates, int steps = 1) => new Vector2Int(coordinates.x - steps, coordinates.y);
      public static Vector2Int Right(this Vector2Int coordinates, int steps = 1) => new Vector2Int(coordinates.x + steps, coordinates.y);
      public static Vector2Int UpLeft(this Vector2Int coordinates, int steps = 1) => new Vector2Int(coordinates.x + Mathf.Abs(coordinates.y % 2) - (steps + 1) / 2, coordinates.y + steps);
      public static Vector2Int UpRight(this Vector2Int coordinates, int steps = 1) => new Vector2Int(coordinates.x + Mathf.Abs(coordinates.y % 2) + steps / 2, coordinates.y + steps);
      public static Vector2Int DownLeft(this Vector2Int coordinates, int steps = 1) => new Vector2Int(coordinates.x + Mathf.Abs(coordinates.y % 2) - (steps + 1) / 2, coordinates.y - steps);
      public static Vector2Int DownRight(this Vector2Int coordinates, int steps = 1) => new Vector2Int(coordinates.x + Mathf.Abs(coordinates.y % 2) + steps / 2, coordinates.y - steps);
   }
}