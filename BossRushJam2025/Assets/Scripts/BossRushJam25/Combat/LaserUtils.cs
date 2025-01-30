using System.Collections.Generic;
using System.Linq;
using BossRushJam25.Health;
using BossRushJam25.HexGrid;
using UnityEngine;

namespace BossRushJam25.Combat {
   public static class LaserUtils {
      public static HashSet<Vector2Int> Shoot(Vector2Int origin, HexCoordinates.EDirection direction, bool skipOrigin, out Vector2Int blockingCoordinates) {
         var damagedCoordinates = new HashSet<Vector2Int>();
         var hexCoordinates = origin;
         if (skipOrigin) hexCoordinates = hexCoordinates.Neighbour(direction);
         while (HexGridController.Instance.TryGetHex(hexCoordinates, out var hex) && !hex.HexContents.Any(t => t.Type.ObstacleForDamageTypes.Contains(DamageType.Laser))) {
            damagedCoordinates.Add(hexCoordinates);
            hexCoordinates = hexCoordinates.Neighbour(direction);
         }
         blockingCoordinates = hexCoordinates;
         return damagedCoordinates;
      }
   }
}