using UnityEngine;

namespace BossRushJam25.Combat {
   public interface ILaserCaster {
      bool IsShooting { get; }
      Vector2Int CoordinatesWhereShotIsBlocked { get; }
   }
}