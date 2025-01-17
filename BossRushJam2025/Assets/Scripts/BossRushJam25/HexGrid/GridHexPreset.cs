using System;
using UnityEngine;

namespace BossRushJam25.HexGrid {
   [Serializable]
   public class GridHexPreset {
      [SerializeField] protected GridHex hexPrefab;
      [SerializeField] protected GridHexContent contentPrefab;

      public GridHex HexPrefab => hexPrefab;
      public GridHexContent ContentPrefab => contentPrefab;
   }
}