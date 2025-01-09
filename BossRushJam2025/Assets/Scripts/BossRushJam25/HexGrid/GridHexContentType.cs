using UnityEngine;

namespace BossRushJam25.HexGrid {
   [CreateAssetMenu]
   public class GridHexContentType : ScriptableObject {
      [SerializeField] protected bool walkable = true;

      public bool Walkable => walkable;
   }
}