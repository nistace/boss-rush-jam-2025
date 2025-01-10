using UnityEngine;

namespace BossRushJam25.HexGrid {
   [CreateAssetMenu]
   public class HexHighlightType : ScriptableObject {
      [SerializeField] protected Material hexMaterial;

      public Material HexMaterial => hexMaterial;
   }
}