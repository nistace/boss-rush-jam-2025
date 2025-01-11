using UnityEngine;

namespace BossRushJam25.HexGrid {
   [CreateAssetMenu]
   public class GridHexContentType : ScriptableObject {
      [SerializeField] protected int maxToSpawn = 1;
      [SerializeField] protected int rotationStepsInHex = 1;

      public int MaxToSpawn => maxToSpawn;
      public int RotationStepsInHex => rotationStepsInHex;
   }
}