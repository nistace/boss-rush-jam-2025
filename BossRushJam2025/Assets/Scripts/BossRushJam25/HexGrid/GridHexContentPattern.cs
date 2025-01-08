using System.Collections.Generic;
using UnityEngine;
namespace BossRushJam25 {
   [CreateAssetMenu]
   public class GridHexContentPattern : ScriptableObject {
      [SerializeField] protected GridHexContent[] contents;
      public IReadOnlyList<GridHexContent> Contents => contents;
   }
}
