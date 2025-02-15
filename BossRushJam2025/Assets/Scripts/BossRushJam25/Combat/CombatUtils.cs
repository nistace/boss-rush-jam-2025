﻿using System.Linq;
using BossRushJam25.BossFights;
using BossRushJam25.Health;
using BossRushJam25.HexGrid;
using UnityEngine;

namespace BossRushJam25.Combat {
   public static class CombatUtils {
      public static Vector2Int GetRandomHexGridBorderVertex() => HexGridController.Instance.GetBorderVertices().OrderBy(_ => Random.value).FirstOrDefault();

      public static Vector2Int GetHeroCoordinates() => HexGridController.Instance.WorldToCoordinates(BossFightInfo.Hero.transform.position);

      public static Vector2Int GetHexOnBorder(Vector2Int fromHex, HexCoordinates.EDirection directionTowardsBorder) {
         var hex = fromHex;
         Vector2Int next;
         while (HexGridController.Instance.IsCellInGrid(next = hex.Neighbour(directionTowardsBorder))) {
            hex = next;
         }
         return hex;
      }

      public static void DamageHero(DamageType damageType, int damage) => BossFightInfo.Hero.Health.Damage(damage, damageType);
   }
}