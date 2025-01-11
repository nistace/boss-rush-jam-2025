using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BossRushJam25.HexGrid {
   [CreateAssetMenu]
   public class GridHexType : ScriptableObject {
      [SerializeField] protected bool alwaysAnObstacle;
      [SerializeField] protected NeighbourProbability[] neighbours;
      [SerializeField] protected int noContentProbability = 1;
      [SerializeField] protected ContentProbability[] contents;

      public bool AlwaysAnObstacle => alwaysAnObstacle;
      public IReadOnlyList<NeighbourProbability> Neighbours => neighbours;

      public GridHexContent RollContentPrefab() {
         if (contents.Length == 0) return default;

         var random = Random.Range(0, contents.Sum(t => t.Probability) + noContentProbability);
         if (random < noContentProbability) return default;

         random -= noContentProbability;
         foreach (var content in contents) {
            random -= content.Probability;
            if (random <= 0) return content.ContentPrefab;
         }

         return default;
      }

      [Serializable] public class NeighbourProbability {
         [SerializeField] protected GridHex prefab;
         [SerializeField] protected int probability = 1;

         public GridHex Prefab => prefab;
         public int Probability => probability;
      }

      [Serializable] protected class ContentProbability {
         [SerializeField] protected GridHexContent contentPrefab;
         [SerializeField] protected int probability = 1;

         public GridHexContent ContentPrefab => contentPrefab;
         public int Probability => probability;
      }
   }
}