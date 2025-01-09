using UnityEngine;

namespace BossRushJam25.HexGrid {
   public class GridHexContent : MonoBehaviour {
      [SerializeField] protected GridHexContentType type;

      public GridHexContentType Type => type;
   }
}