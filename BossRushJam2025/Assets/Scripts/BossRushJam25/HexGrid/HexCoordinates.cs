﻿using System;
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

      public static EDirection Opposite(this EDirection direction) => direction switch {
         EDirection.Left => EDirection.Right,
         EDirection.UpLeft => EDirection.DownRight,
         EDirection.UpRight => EDirection.DownLeft,
         EDirection.Right => EDirection.Left,
         EDirection.DownRight => EDirection.UpLeft,
         EDirection.DownLeft => EDirection.UpRight,
         _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
      };

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

      public static EDirection RotationToDirection(Transform transform) => RotationToDirection(Vector3.SignedAngle(Vector3.forward, transform.forward, Vector3.up));

      public static EDirection RotationToDirection(float signedAngleWithForward) {
         while (signedAngleWithForward < 0) signedAngleWithForward += 360f;
         signedAngleWithForward %= 360;
         return signedAngleWithForward switch {
            < 60 => EDirection.UpRight,
            < 120 => EDirection.Right,
            < 180 => EDirection.DownRight,
            < 240 => EDirection.DownLeft,
            < 300 => EDirection.Left,
            _ => EDirection.UpLeft
         };
      }

      public static float ToYawDegrees(this EDirection direction) => direction switch {
         EDirection.Left => -90,
         EDirection.UpLeft => -30,
         EDirection.UpRight => 30,
         EDirection.Right => 90,
         EDirection.DownRight => 150,
         EDirection.DownLeft => -150,
         _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
      };

      public static Vector2Int Left(this Vector2Int coordinates, int steps = 1) => new Vector2Int(coordinates.x - steps, coordinates.y);
      public static Vector2Int Right(this Vector2Int coordinates, int steps = 1) => new Vector2Int(coordinates.x + steps, coordinates.y);
      public static Vector2Int UpLeft(this Vector2Int coordinates, int steps = 1) => new Vector2Int(coordinates.x + Mathf.Abs(coordinates.y % 2) - (steps + 1) / 2, coordinates.y + steps);
      public static Vector2Int UpRight(this Vector2Int coordinates, int steps = 1) => new Vector2Int(coordinates.x + Mathf.Abs(coordinates.y % 2) + steps / 2, coordinates.y + steps);
      public static Vector2Int DownLeft(this Vector2Int coordinates, int steps = 1) => new Vector2Int(coordinates.x + Mathf.Abs(coordinates.y % 2) - (steps + 1) / 2, coordinates.y - steps);
      public static Vector2Int DownRight(this Vector2Int coordinates, int steps = 1) => new Vector2Int(coordinates.x + Mathf.Abs(coordinates.y % 2) + steps / 2, coordinates.y - steps);

      public static int HexDistance(Vector2Int origin, Vector2Int center) {
         var cubeOrigin = OffsetCoordinatesToCubeCoordinates(origin);
         var cubeCenter = OffsetCoordinatesToCubeCoordinates(center);

         var cubeDistance = cubeOrigin - cubeCenter;

         return (Mathf.Abs(cubeDistance.x) + Mathf.Abs(cubeDistance.y) + Mathf.Abs(cubeDistance.z)) / 2;
      }

      public static Vector2Int OffsetCoordinatesToAxialCoordinates(Vector2Int o) => new Vector2Int(o.x + (o.y + (o.y & 1)) / 2, -o.y);
      public static Vector3Int AxialCoordinatesToCubeCoordinates(Vector2Int a) => new Vector3Int(a.x, a.y, -a.x - a.y);
      public static Vector3Int OffsetCoordinatesToCubeCoordinates(Vector2Int o) => AxialCoordinatesToCubeCoordinates(OffsetCoordinatesToAxialCoordinates(o));
   }
}